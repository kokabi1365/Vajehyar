using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vajehyar.Properties;
using Vajehyar.Utility;

namespace Vajehyar.Windows
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
