using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Vajehdan.Views;

namespace Vajehdan
{
    public partial class App
    {

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider
                .RegisterLicense("MjMzODU2QDMxMzgyZTMxMmUzMGVmVFhFRzV2RWo0bWxrWGF5aFpjc0JWNHpXRXlVTzV3RG9IZTVWbFIrdXc9");

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

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Process.GetCurrentProcess().Kill();
        }
    }
}