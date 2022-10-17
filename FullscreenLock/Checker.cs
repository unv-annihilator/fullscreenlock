using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace FullscreenLock
{
    public class OffsetParameters
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int W { get; set; } = 0;
        public int H { get; set; } = 0;
    }

    public class IniFile   // revision 11
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }

    internal static class Extensions
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            try
            {
                var imageName = QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                    fileNameBuilder.ToString() :
                    null;
                return imageName;
            }
            catch
            {
            }
            return null;
        }
    }

    public class Checker
    {
        System.Timers.Timer t = new System.Timers.Timer();
        System.Timers.Timer t2 = new System.Timers.Timer();

        // Import a bunch of win32 API calls.
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);
        [DllImport("user32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(ref RECT rcClip);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        Label l; // One day I'll figure out how to set the label without sending a pointer into the constructor.
        ListBox procBox;
        ListBox selectedBox;
        NotifyIcon notifyIcon;
        List<ProcessModel> savedSelectedApps = new List<ProcessModel>();
        List<ProcessModel> runningApps = new List<ProcessModel>();

        object cObj = new object();
        object lockObj = new object();
        object iniObj = new object();

        public Dispatcher dispatch = Dispatcher.CurrentDispatcher;

        public bool StartMinimized { get; set; }
        public bool MinimizeToSystemTray { get; set; }
        public bool ShiftOverrideEnabled { get; set; }

        public class ProcessModel
        {
            public string ProcessName { get; set; }
            public string ProcessTitle { get; set; }
            public bool CreatedFromExe { get; set; }
        }

        public Checker(Label ll, FullscreenLock f, ListBox procBox, ListBox selectedBox, NotifyIcon notifyIcon)
        {
            l = ll;
            this.procBox = procBox;
            this.selectedBox = selectedBox;
            this.notifyIcon = notifyIcon;

            LoadSettings();
            LoadAppSettings();

            // notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Icon = Properties.Resources.gaben;

            if (StartMinimized)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
                f.ShowInTaskbar = false;
                f.WindowState = FormWindowState.Minimized;
                f.Hide();
            }
            else
            {
                f.Show();
                f.WindowState = FormWindowState.Normal;
                f.ShowInTaskbar = true;
            }

            Task.Factory.StartNew(() =>
            {
                ReadSelectedApps();
                ReadRunningApps();

                t2.Elapsed += new ElapsedEventHandler(ListProcesses);
                t2.Interval = 5000;
                t2.Start();

                t.Elapsed += new ElapsedEventHandler(CheckForFullscreenApps);
                t.Interval = 100;
                t.Start();
            });
        }

        public void toggle(Button b, Label l)
        {
            if (t.Enabled)
            {
                t.Stop();
                t2.Stop();
                dispatch.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    l.Text = "Paused";
                    b.Text = "Start";
                }));
            }
            else
            {
                t2.Start();
                t.Start();
                dispatch.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    l.Text = "Waiting for focus";
                    b.Text = "Stop";
                }));
            }
        }

        public void moveRight()
        {
            lock (lockObj)
            {
                if (procBox.SelectedIndex != -1)
                {
                    var selected = (ProcessModel)procBox.SelectedItem;

                    if (selected != null)
                    {
                        if (!selectedBox.Items.Contains(selected))
                        {
                            selectedBox.Items.Add(selected);
                        }
                        this.savedSelectedApps.Add(selected);

                        if (selected.CreatedFromExe)
                        {
                            this.runningApps.Remove(selected);
                            procBox.Items.Remove(selected);
                        }
                    }
                }
                SaveSelectedApps();
                SaveRunningApps();
            }
            LoadProcesses();
        }

        public void delSelected()
        {
            lock (lockObj)
            {
                if (procBox.SelectedIndex != -1)
                {
                    var selected = (ProcessModel)procBox.SelectedItem;

                    procBox.Items.Remove(selected);
                    this.runningApps.Remove(selected);
                    SaveRunningApps();
                }
                else if (selectedBox.SelectedIndex != -1)
                {
                    var selected = (ProcessModel)selectedBox.SelectedItem;

                    selectedBox.Items.Remove(selected);
                    this.savedSelectedApps.Remove(selected);
                    SaveSelectedApps();
                }
            }
        }

        public void moveLeft()
        {
            ProcessModel savedExe = null;

            lock (lockObj)
            {
                if (selectedBox.SelectedIndex != -1)
                {
                    var selected = (ProcessModel)selectedBox.SelectedItem;

                    if (selected != null)
                    {
                        selectedBox.Items.Remove(selected);
                    }
                    this.savedSelectedApps.Remove(selected);

                    if (selected.CreatedFromExe)
                    {
                        savedExe = selected;
                    }
                }
                SaveSelectedApps();
            }
            LoadProcesses(savedExe);
        }

        public void AddExe(string exeFileName)
        {
            if (!string.IsNullOrEmpty(exeFileName))
            {
                lock (lockObj)
                {
                    var selected = new ProcessModel
                    {
                        ProcessName = exeFileName,
                        ProcessTitle = exeFileName,
                        CreatedFromExe = true
                    };

                    if (selected != null)
                    {
                        if (!selectedBox.Items.Contains(selected))
                        {
                            selectedBox.Items.Add(selected);
                        }
                        this.savedSelectedApps.Add(selected);
                    }
                }
                SaveSelectedApps();
                LoadProcesses();
            }
        }

        private void LoadSettings()
        {
            lock (iniObj)
            {
                var iniFile = new IniFile();

                StartMinimized = iniFile.Read("StartMinimized") == "true";
                MinimizeToSystemTray = iniFile.Read("MinimizeToTray") == "true";
                ShiftOverrideEnabled = iniFile.Read("ShiftOverride") == "true";
            }
        }

        public void SaveSettings()
        {
            lock (iniObj)
            {
                var iniFile = new IniFile();

                iniFile.Write("StartMinimized", StartMinimized ? "true" : "false");
                iniFile.Write("MinimizeToTray", MinimizeToSystemTray ? "true" : "false");
                iniFile.Write("ShiftOverride", ShiftOverrideEnabled ? "true" : "false");
            }
        }

        private void ReadSelectedApps()
        {
            lock (iniObj)
            {
                this.savedSelectedApps = new List<ProcessModel>();
                var iniFile = new IniFile();
                var procs = iniFile.Read("SelectedProcesses");
                if (!string.IsNullOrWhiteSpace(procs))
                {
                    var savedProcs = procs.Split(',').ToArray();
                    foreach (var savedProc in savedProcs)
                    {
                        var split = savedProc.Split(';').ToArray();

                        var proc = new ProcessModel
                        {
                            ProcessName = split.Length > 0 ? split[0] : savedProc,
                            ProcessTitle = split.Length > 1 ? split[1] : savedProc,
                            CreatedFromExe = split.Length > 2 ? split[2] == "Y" : false
                        };

                        this.savedSelectedApps.Add(proc);
                    }
                }

                dispatch.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    lock (lockObj)
                    {
                        var objCol = new ListBox.ObjectCollection(selectedBox);
                        objCol.AddRange(this.savedSelectedApps.ToArray());
                        selectedBox.Items.Clear();
                        selectedBox.Items.AddRange(objCol);
                    }
                }));
            }
        }

        private void ReadRunningApps()
        {
            lock (iniObj)
            {
                runningApps = new List<ProcessModel>();
                var iniFile = new IniFile();
                var procs = iniFile.Read("RunningProcesses");
                if (!string.IsNullOrWhiteSpace(procs))
                {
                    var savedProcs = procs.Split(',').ToArray();
                    foreach (var savedProc in savedProcs)
                    {
                        var split = savedProc.Split(';').ToArray();

                        var proc = new ProcessModel
                        {
                            ProcessName = split.Length > 0 ? split[0] : savedProc,
                            ProcessTitle = split.Length > 1 ? split[1] : savedProc,
                            CreatedFromExe = split.Length > 2 ? split[2] == "Y" : false
                        };

                        if (!savedSelectedApps.Any(x => x.ProcessName == proc.ProcessName || x.ProcessTitle == proc.ProcessTitle))
                        {
                            runningApps.Add(proc);
                        }
                    }
                }

                dispatch.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    lock (lockObj)
                    {
                        var objCol = new ListBox.ObjectCollection(procBox);
                        objCol.AddRange(runningApps.ToArray());
                        procBox.Items.Clear();
                        procBox.Items.AddRange(objCol);
                    }
                }));
            }
        }

        public void LoadAppSettings()
        {
            lock (iniObj)
            {
                var iniFile = new IniFile();
                var procs = iniFile.Read("AppOffsets");

                if (!string.IsNullOrWhiteSpace(procs))
                {
                    var savedApps = procs.Split(',').ToArray();

                    foreach (var savedApp in savedApps)
                    {
                        var split = savedApp.Split(';').ToArray();

                        string app = split.Length > 0 ? split[0] : null;
                        int x = 0;
                        int y = 0;
                        int w = 0;
                        int h = 0;

                        if (split.Length > 1)
                        {
                            int.TryParse(split[1], out x);
                        }
                        if (split.Length > 2)
                        {
                            int.TryParse(split[2], out y);
                        }
                        if (split.Length > 3)
                        {
                            int.TryParse(split[3], out w);
                        }
                        if (split.Length > 4)
                        {
                            int.TryParse(split[4], out h);
                        }

                        if (!string.IsNullOrWhiteSpace(app))
                        {
                            var offsets = new OffsetParameters
                            {
                                X = x,
                                Y = y,
                                W = w,
                                H = h
                            };

                            if (!appOffsets.ContainsKey(app))
                            {
                                appOffsets.Add(app, offsets);
                            }
                            else
                            {
                                appOffsets[app] = offsets;
                            }
                        }
                    }
                }
            }
        }

        public void SaveAppSettings()
        {
            lock (iniObj)
            {
                var iniFile = new IniFile();
                var strBuilder = new StringBuilder();

                foreach (var offsetKey in this.appOffsets.Keys)
                {
                    var val = this.appOffsets[offsetKey];
                    if (val.X != 0 || val.Y != 0 || val.W != 0 || val.H != 0)
                    {
                        if (strBuilder.Length != 0) strBuilder.Append(",");
                        strBuilder.Append(offsetKey);
                        strBuilder.Append(";");
                        strBuilder.Append(val.X);
                        strBuilder.Append(";");
                        strBuilder.Append(val.Y);
                        strBuilder.Append(";");
                        strBuilder.Append(val.W);
                        strBuilder.Append(";");
                        strBuilder.Append(val.H);
                    }
                }
                iniFile.Write("AppOffsets", strBuilder.ToString());
            }
        }
        private void SaveSelectedApps()
        {
            lock (iniObj)
            {
                var iniFile = new IniFile();
                var strBuilder = new StringBuilder();
                foreach (ProcessModel proc in this.savedSelectedApps)
                {
                    if (strBuilder.Length != 0) strBuilder.Append(",");
                    strBuilder.Append(proc.ProcessName + ";" + proc.ProcessTitle + (proc.CreatedFromExe ? ";Y" : ""));
                }
                iniFile.Write("SelectedProcesses", strBuilder.ToString());
            }
        }
        private void SaveRunningApps()
        {
            lock (iniObj)
            {
                var iniFile = new IniFile();
                var strBuilder = new StringBuilder();
                foreach (ProcessModel proc in this.runningApps)
                {
                    if (strBuilder.Length != 0) strBuilder.Append(",");
                    strBuilder.Append(proc.ProcessName + ";" + proc.ProcessTitle + (proc.CreatedFromExe ? ";Y" : ""));
                }
                iniFile.Write("RunningProcesses", strBuilder.ToString());
            }
        }

        private static string GetProcessFileName(Process proc)
        {
            var filePath = proc.GetMainModuleFileName();
            var path = !string.IsNullOrEmpty(filePath) ? Path.GetFileNameWithoutExtension(filePath) : proc.ProcessName;

            path = path.Length > 1 ? path.Substring(0, 1).ToUpperInvariant() + path.Substring(1) : path;
            return path;
        }

        private void LoadProcesses(ProcessModel savedExe = null)
        {
            lock (cObj)
            {
                var procs = Process.GetProcesses();
                RECT appBounds;
                Rectangle screenBounds;
                var shellWindow = GetShellWindow();
                var desktopWindow = GetDesktopWindow();
                var curProcessName = Process.GetCurrentProcess().ProcessName;

                var fullScreenProcs = new List<ProcessModel>();
                var normalProcs = new List<ProcessModel>();
                var running = new List<ProcessModel>();

                foreach (var proc in procs)
                {
                    var hWnd = proc.MainWindowHandle;

                    if (desktopWindow == hWnd) continue;
                    if (shellWindow == hWnd) continue;
                    if (!IsWindowVisible(hWnd)) continue;
                    if (GetWindowTextLength(hWnd) == 0) continue;

                    GetWindowRect(hWnd, out appBounds);

                    if (!savedSelectedApps.Any(x => x.ProcessName == proc.ProcessName) &&
                        proc.ProcessName != curProcessName &&
                        appBounds.Size.Width > 0 && appBounds.Size.Height > 0)
                    {
                        screenBounds = Screen.FromHandle(hWnd).Bounds;
                        var model = new ProcessModel
                        {
                            ProcessName = proc.ProcessName,
                            ProcessTitle = GetProcessFileName(proc)
                        };
                        //if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                        //{

                        //}
                        if (!running.Any(x => x.ProcessName == model.ProcessName || x.ProcessTitle == model.ProcessTitle))
                        {
                            running.Add(model);
                        }
                    }
                }

                lock (lockObj)
                {
                    if (savedExe != null)
                    {
                        var runningExe2 = running.FirstOrDefault(x => x.ProcessName == savedExe.ProcessName || x.ProcessTitle == savedExe.ProcessTitle);
                        if (runningExe2 == null)
                        {
                            running.Add(savedExe);
                        }
                        else
                        {
                            runningExe2.CreatedFromExe = true;
                        }
                    }

                    foreach (var addedExe in this.runningApps.Where(x => x.CreatedFromExe))
                    {
                        var runningExe = running.FirstOrDefault(x => x.ProcessName == addedExe.ProcessName || x.ProcessTitle == addedExe.ProcessTitle);
                        if (runningExe == null)
                        {
                            running.Add(addedExe);
                        }
                        else
                        {
                            runningExe.CreatedFromExe = true;
                        }
                    }

                    if (running.Any(x => !this.runningApps.Any(y => (y.ProcessName == x.ProcessName || y.ProcessTitle == x.ProcessTitle) && x.CreatedFromExe == y.CreatedFromExe)) ||
                                         this.runningApps.Any(x => !running.Any(y => (y.ProcessName == x.ProcessName || y.ProcessTitle == x.ProcessTitle) && x.CreatedFromExe == y.CreatedFromExe)))
                    {
                        this.runningApps = running;
                        SaveRunningApps();

                        dispatch.BeginInvoke(
                        DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            lock (lockObj)
                            {
                                var objCol = new ListBox.ObjectCollection(procBox);

                                objCol.AddRange(running.ToArray());

                                procBox.Items.Clear();
                                procBox.Items.AddRange(objCol);
                            }
                        }));
                    }
                }
            }
        }

        private void ListProcesses(object sender, ElapsedEventArgs e)
        {
            if (IsWindowVisible(Process.GetCurrentProcess().MainWindowHandle))
            {
                LoadProcesses();
            }
        }

        private void CheckForFullscreenApps(object sender, ElapsedEventArgs e)
        {
            dispatch.BeginInvoke(
            DispatcherPriority.Normal,
            (Action)(() =>
            {
                if (IsForegroundFullScreen(this))
                {

                    l.Text = "Fullscreen app in focus";
                }
                else
                {
                    l.Text = "Waiting for focus";

                }
            }));
        }

        public bool shiftInterception = false;

        public Dictionary<string, OffsetParameters> appOffsets = new Dictionary<string, OffsetParameters>();

        public static bool IsForegroundFullScreen(Checker c)
        {
            //Get the handles for the desktop and shell now.
            IntPtr desktopHandle;
            IntPtr shellHandle;
            desktopHandle = GetDesktopWindow();
            shellHandle = GetShellWindow();
            RECT appBounds;
            Rectangle screenBounds;
            IntPtr hWnd;

            if (c.ShiftOverrideEnabled && Keyboard.IsKeyDown(Key.LeftShift))
            {
                System.Windows.Forms.Cursor.Clip = Rectangle.Empty;
                c.shiftInterception = true;
                return false;
            }

            hWnd = GetForegroundWindow();
            if (hWnd != null && !hWnd.Equals(IntPtr.Zero))
            {
                //Check we haven't picked up the desktop or the shell
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle)))
                {
                    GetWindowRect(hWnd, out appBounds);
                    //determine if window is fullscreen
                    screenBounds = Screen.FromHandle(hWnd).Bounds;
                    uint procid = 0;
                    GetWindowThreadProcessId(hWnd, out procid);
                    var proc = Process.GetProcessById((int)procid);
                    var procFileName = GetProcessFileName(proc);
                    if (c.savedSelectedApps.Any(x => x.ProcessName == proc.ProcessName || x.ProcessName == procFileName))
                    {
                        if ((appBounds.Bottom - appBounds.Top) + 100 >= screenBounds.Height && (appBounds.Right - appBounds.Left) + 50 >= screenBounds.Width
                             &&
                             //If shift interception is done, then the cursor must be within the app to re-active the mouselock behavior 
                             (!c.shiftInterception ||
                              (System.Windows.Forms.Cursor.Position.X >= appBounds.Left &&
                               System.Windows.Forms.Cursor.Position.X <= appBounds.Right) &&
                               System.Windows.Forms.Cursor.Position.Y >= appBounds.Top) &&
                               System.Windows.Forms.Cursor.Position.Y <= appBounds.Bottom)
                        {
                            //Console.WriteLine(proc.ProcessName);
                            var offsets = c.appOffsets.ContainsKey(proc.ProcessName) ? c.appOffsets[proc.ProcessName] : null;

                            int x = 1;
                            int y = 1;
                            int w = 0;
                            int h = 0;

                            if (offsets != null)
                            {
                                x = offsets.X;
                                y = offsets.Y;
                                w = offsets.W;
                                h = offsets.H;
                            }

                            System.Windows.Forms.Cursor.Clip = new Rectangle(appBounds.Left + x, appBounds.Top + y, appBounds.Width + w, appBounds.Height + h);
                            return true;
                        }
                        else
                        {
                            System.Windows.Forms.Cursor.Clip = Rectangle.Empty;
                            return false;
                        }
                    }
                    else
                    {
                        System.Windows.Forms.Cursor.Clip = Rectangle.Empty;
                        return false;
                    }
                }
            }
            else
            {
                System.Windows.Forms.Cursor.Clip = Rectangle.Empty;
            }
            return false;
        }
    }
}

