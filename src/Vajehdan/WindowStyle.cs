using System.Windows;
using System.Windows.Input;
using Vajehdan.Annotations;
using Vajehdan.Views;

namespace Vajehdan
{
    [UsedImplicitly]
    public partial class WindowStyle
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

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            
            if (window.GetType()==typeof(FavoritesWindow))
                return;

            Helper.MakeWindowCenterScreen(window);
            Helper.SlideUpAnimation(window);
        }
    }
}
