using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Vajehdan.Properties;

namespace Vajehdan.Windows
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            Close();
        }


        private void SettingWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
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
            Settings.Default.Save();
        }

        
    }
}
