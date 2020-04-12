using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Vajehdan.Properties;

namespace Vajehdan.Views
{
    /// <summary>
    /// Interaction logic for ChangelogWindow.xaml
    /// </summary>
    public partial class ChangelogWindow : INotifyPropertyChanged
    {
        private string _changelog;

        public string Changelog
        {
            get => _changelog;
            set
            {
                _changelog = value;
                NotifyPropertyChanged("Changelog");
            }
        }

        public ChangelogWindow(string changelog)
        {
            InitializeComponent();
            Changelog = changelog;
        }

      

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void GoToDownloadPage_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
            Process.Start(Settings.Default.UpdateUrl);
            e.Handled = true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
