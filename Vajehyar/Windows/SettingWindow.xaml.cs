using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using Vajehyar.Properties;

namespace Vajehyar.Windows
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


        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Escape)
            {
                return;
            }
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
           
            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                                     || key == Key.LeftCtrl || key == Key.RightCtrl
                                     || key == Key.LeftAlt || key == Key.RightAlt
                                     || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl + ");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift + ");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt + ");
            }

            shortcutText.Append(key.ToString());
            textBox.Text = shortcutText.ToString();
        }

        private void SettingWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }

        }
       
    }
}
