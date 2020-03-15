using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using Vajehdan.Properties;
using ContextMenu = System.Windows.Controls.ContextMenu;
using FileDialog = Microsoft.Win32.FileDialog;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Vajehdan
{
    public partial class App
    {
        #region Fields
        private NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;
        private IKeyboardMouseEvents _hook;
        private string _appName;
        private ContextMenu _contextMenu;
        int triggerThreshold = 500; //This would be equivalent to .5 seconds
        int lastCtrlTick = 0;
        #endregion

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjIzMDA2QDMxMzcyZTM0MmUzMEdXcEJJVDRCTnFCbWhRbDRabWVkMU1DdURxYmpwWnZMSHgzYnVsN3N5VHM9");
            AppDomain currentDomain=AppDomain.CurrentDomain;
#if !DEBUG
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            string error =
                $"Source: {e.Source}\n\n" +
                $"Message: {e.Message}\n\n" +
                $"Target: {e.TargetSite}\n\n" +
                $"StackTrace: {e.Demystify()}\n\n";

            File.WriteAllText("log.txt", error);
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(-1);
        }

        
        #region App Startup
        private void App_Startup(object sender, StartupEventArgs e)
        {
            //Get application name: Vajehdan
            _appName = Assembly.GetExecutingAssembly().GetName().Name;
            _contextMenu = FindResource("NotifierContextMenu") as ContextMenu;

            //Run only one instance of the app
            var mutex = new Mutex(true, _appName, out var createdNew);
            if (!createdNew) Current.Shutdown();

            SetKeyboardHook();
            SetNotifyIcon();

          
            _mainWindow = new MainWindow(Database.Instance);
            var hasStartByWindowsArg = e.Args.Any(s => s.Contains(Settings.Default.StartupArgument));
            
            //Start minimized if specified
            if (hasStartByWindowsArg)
                HideMainWindow();
            else
                ShowMainWindow();
        }

        #endregion

        #region System tray
        private void SetNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();
            var ico = GetResourceStream(new Uri("pack://application:,,,/Resources/Vajehdan.ico"))?.Stream;
            _notifyIcon.Icon = new Icon(ico);
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "واژه‌دان";

            /*if (Settings.Default.FirstRun)
            {
                _notifyIcon.ShowBalloonTip(30000, "واژه‌دان", "باز کردن برنامه: Alt + Shift + V\nبستن برنامه: Esc\nتنظیمات را می‌توانید تغییر دهید.", ToolTipIcon.Info);
                Settings.Default.FirstRun = false;
            }*/

            _notifyIcon.MouseDown += NotifyIcon_MouseDown;
        }

        private void NotifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) _contextMenu.IsOpen = !_contextMenu.IsOpen;

            if (e.Button == MouseButtons.Left)
            {
                if (_mainWindow.WindowState == WindowState.Normal)
                    HideMainWindow();
                else if (_mainWindow.WindowState == WindowState.Minimized)
                    ShowMainWindow();
            }
        }

        private void Menu_Setting(object sender, RoutedEventArgs e)
        {
            Window settingsWindow = new SettingWindow();
            settingsWindow.Show();
        }

        private void Menu_About(object sender, RoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }
        #endregion

        #region Keyboard hook and shortcut key
        private void SetKeyboardHook()
        {
            _hook = Hook.GlobalEvents();
            _hook.KeyDown += _hook_KeyDown;
        }

        private void _hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (Current.Windows.OfType<SettingWindow>().Any()) return;

            if (e.KeyData == Keys.Escape)
            {
                _contextMenu.IsOpen = false;
                HideMainWindow();
                return;
            }

            if (!Settings.Default.OpenByDoubleAlt)
                return;

            if (e.Modifiers != Keys.Alt)
                return;

            int thisCtrlTick = Environment.TickCount;
            int elapsed = thisCtrlTick - lastCtrlTick;

            if (elapsed <= triggerThreshold)
            {
                ShowMainWindow();
            }
            lastCtrlTick = thisCtrlTick;
        }
        #endregion

        #region Show and hide windows
        public void HideMainWindow()
        {
            _mainWindow.Hide();
            _mainWindow.WindowState = WindowState.Minimized;
        }

        public void ShowMainWindow()
        {
            _contextMenu.IsOpen = false;
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Show();
            _mainWindow.txtSearch.SelectAll();
            _mainWindow.txtSearch.Focus();
        }
        #endregion

        #region Exit tasks
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
            _notifyIcon.Dispose();
            ConfigRegistry();
        }

        private void ConfigRegistry()
        {
            var key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!Settings.Default.StartByWindows)
            {
                key?.DeleteValue(_appName, false);
                return;
            }

            var value = Assembly.GetExecutingAssembly().Location + " " + Settings.Default.StartupArgument;
            key?.SetValue(_appName, value);
        }
        #endregion

       
    }
}