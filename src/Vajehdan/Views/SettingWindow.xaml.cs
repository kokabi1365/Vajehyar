using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;
using Microsoft.Win32;
using Vajehdan.Controls;

namespace Vajehdan.Views
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        [SupportedOSPlatform("windows")]
        private void SettingWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Helper.GetSettings().Save();

            try
            {
                var appName = Assembly.GetEntryAssembly()?.GetName().Name;

                var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (!Helper.GetSettings().StartByWindows)
                {
                    key?.DeleteValue(appName ?? string.Empty, false);
                    return;
                }

                var value = Process.GetCurrentProcess().MainModule?.FileName;
                key?.SetValue(appName, value ?? string.Empty);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartByWindowsCheckBox.SetValue(CheckBoxControl.IsCheckedProperty, true);
            HotkeyCheckBox.SetValue(CheckBoxControl.IsCheckedProperty, true);
        }
    }
}
