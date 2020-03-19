using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Vajehdan.Properties;
using ContextMenu = System.Windows.Controls.ContextMenu;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Keys = System.Windows.Forms.Keys;
using MessageBox = System.Windows.MessageBox;
using MouseButtons = System.Windows.Forms.MouseButtons;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Vajehdan
{
    public partial class App
    {
        private ContextMenu _contextMenu;
        private NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;
        private readonly int _triggerThreshold = 500;
        private int _lastCtrlTick;
        private KeyboardHook _keyboardHook;

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense("MjIzMDA2QDMxMzcyZTM0MmUzMEdXcEJJVDRCTnFCbWhRbDRabWVkMU1DdURxYmpwWnZMSHgzYnVsN3N5VHM9");

#if !DEBUG
            AppDomain currentDomain=AppDomain.CurrentDomain;
            currentDomain.UnhandledException += (o, args) =>
            {
                Exception e = (Exception) args.ExceptionObject;
                string error =
                    $"Source: {e.Source}\n\n" +
                    $"Message: {e.Message}\n\n" +
                    $"Target: {e.TargetSite}\n\n" +
                    $"StackTrace: {e.Demystify()}\n\n";

                File.WriteAllText("log.txt", error);
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            };
#endif
            Current.Startup += (obj, argrument) =>
            {
                //Set keyboard hook
                _keyboardHook = new KeyboardHook();
                _keyboardHook.SetHook();
                _keyboardHook.OnKeyDownEvent += (o, arg) =>
                {
                    if (Current.Windows.OfType<SettingWindow>().Any()) return;

                    if (arg.KeyData == Keys.Escape)
                    {
                        _contextMenu.IsOpen = false;
                        HideMainWindow();
                        return;
                    }

                    if (!Settings.Default.OpenByDoubleAlt)
                        return;

                    if (arg.Modifiers != Keys.Alt)
                        return;

                    int thisCtrlTick = Environment.TickCount;
                    int elapsed = thisCtrlTick - _lastCtrlTick;

                    if (elapsed <= _triggerThreshold)
                    {
                        ShowMainWindow();
                    }

                    _lastCtrlTick = thisCtrlTick;
                };

                //Set notify icon
                _notifyIcon = new NotifyIcon();
                _notifyIcon.Icon = Helper.GetIconFromResource("Vajehdan.ico");
                _notifyIcon.Visible = true;
                _notifyIcon.Text = "واژه‌دان";
                _notifyIcon.MouseDown += (o, arg) =>
                {

                    if (arg.Button == MouseButtons.Right) 
                        _contextMenu.IsOpen = !_contextMenu.IsOpen;

                    if (arg.Button == MouseButtons.Left)
                    {
                        ShowMainWindow();
                    }
                };

                _contextMenu = FindResource("NotifierContextMenu") as ContextMenu;
                _mainWindow = new MainWindow(Database.Instance);
            };

            Current.Exit += (obj, argrument) =>
            {
                _notifyIcon.Dispose();
                Process.GetCurrentProcess().Kill();
            };
        }

        private void Menu_RunCommand(object sender, RoutedEventArgs e)
        {
            switch (((FrameworkElement)sender).Name)
            {
                case "ItemSetting":
                    new SettingWindow().Show();
                    break;

                case "ItemAbout":
                    new AboutWindow().Show();
                    break;

                case "ItemExit":
                    Current.Shutdown();
                    break;
            }
        }

        public void ShowMainWindow()
        {
            _contextMenu.IsOpen = false;
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Show();
            _mainWindow.txtSearch.SelectAll();
            _mainWindow.txtSearch.Focus();
        }

        public void HideMainWindow()
        {
            _mainWindow.Hide();
            _mainWindow.WindowState = WindowState.Minimized;
        }
    }
}