using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Vajehdan.Resources
{
    public partial class WindowStyle: ResourceDictionary
    {
        public WindowStyle()
        {
            InitializeComponent();
        }


        private void TitleBar_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            if (e.ChangedButton == MouseButton.Left)
                window.DragMove();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            Properties.Settings.Default.Save();
            window.Close();
        }
    }
}
