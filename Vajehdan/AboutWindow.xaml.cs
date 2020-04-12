using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Vajehdan.Properties;

namespace Vajehdan
{
    /// <summary>
    /// Interaction logic for AboutWindow
    /// </summary>
    public partial class AboutWindow : INotifyPropertyChanged
    {
        private string _currentVersion;

        public string CurrentVersion
        {
            get => _currentVersion;
            set
            {
                _currentVersion = value;
                NotifyPropertyChanged("CurrentVersion");
            }
        }

        public AboutWindow()
        {
            InitializeComponent();

            Version version = Assembly.GetEntryAssembly()?.GetName().Version;
            
            CurrentVersion = version.IsBeta() ? $"{version.ToSemVersion()} (آزمایشی)" : version?.ToSemVersion();

            if (Settings.Default.AboutLeftPos == 0 && Settings.Default.AboutTopPos == 0)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Version_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(Settings.Default.UpdateUrl);
            e.Handled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MinimizeWindow();
            Process.Start(Settings.Default.Donate);
            e.Handled = true;
        }

        private void MinimizeWindow()
        {
            Close();
            App app = ((App)Application.Current);
            //app.HideMainWindow();
        }

    }
}
