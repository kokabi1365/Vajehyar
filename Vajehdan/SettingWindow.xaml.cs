using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Vajehdan.Properties;

namespace Vajehdan
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow
    {
        public SettingWindow()
        {
            InitializeComponent();

            if (Settings.Default.SettingLeftPos == 0 && Settings.Default.SettingTopPos == 0)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void SettingWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            Fonts.SelectedIndex = 0;
            Settings.Default.OpenByDoubleAlt = true;
            Settings.Default.FontSize = 13;
            Settings.Default.MinimizeWhenClickOutside = false;
            Settings.Default.StartByWindows = false;
        }

        private void SettingWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            Settings.Default.Save();

            //Update startup entry in registry
            var appName = Assembly.GetExecutingAssembly().GetName().Name;

            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (!Settings.Default.StartByWindows)
            {
                key?.DeleteValue(appName, false);
                return;
            }
            var value = Assembly.GetExecutingAssembly().Location + " " + Settings.Default.StartupArgument;
            key?.SetValue(appName, value);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
