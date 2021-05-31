using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GlobalHotKey;
using Vajehdan.Views;


namespace Vajehdan
{
    [SupportedOSPlatform("windows7.0")]
    public partial class App
    {
        private DateTime _lastKeyPressedTime;

        public App()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                var uncaughtException = ((Exception)e.ExceptionObject);
                uncaughtException.Log();
                MessageBox.Show($"{uncaughtException.Message}\n-----------\nSaved in the log file");
                Environment.Exit(-1);
            };
#endif
            SetupHook();
            Database.LoadData();
            Helper.CheckNewVersion();
        }

        private void SetupHook()
        {

            var hotKeyManager = new HotKeyManager();
            hotKeyManager.Register(Key.None, ModifierKeys.Alt);
            hotKeyManager.KeyPressed += (_, _) =>
            {
                if (!Helper.GetSettings().OpenByHotKey)
                    return;

                if (DateTime.Now - _lastKeyPressedTime > TimeSpan.FromMilliseconds(400))
                {
                    _lastKeyPressedTime = DateTime.Now;
                    return;
                }

                foreach (var mainWindow in Helper.GetWindow<MainWindow>())
                {
                    mainWindow.ShowMainWindow();
                }
            };
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            if (Helper.GetSettings().ClearHistoryOnClose)
            {
                Helper.GetSettings().AutoCompleteList.Clear();
                Helper.GetSettings().Save();
            }

            Process.GetCurrentProcess().Kill();
        }
    }
}
