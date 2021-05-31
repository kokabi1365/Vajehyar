using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Vajehdan.Views;
using Clipboard = System.Windows.Clipboard;

namespace Vajehdan.Controls
{
    [SupportedOSPlatform("windows7.0")]
    public partial class CustomContextMenu
    {
        
        public CustomContextMenu()
        {
            InitializeComponent();
        }

        private async void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedText="";

            if (PlacementTarget is FlowDocumentPageViewer)
            {
                var flowDocumentReader = (PlacementTarget as FlowDocumentPageViewer);
                TextPointer potStart = flowDocumentReader?.Selection?.Start;
                TextPointer potEnd = flowDocumentReader?.Selection?.End;
                var range = new TextRange(potStart, potEnd);
                selectedText = range.Text;
            }

            if (string.IsNullOrWhiteSpace(selectedText))
                return;

            switch ((sender as MenuItem)?.Name)
            {
                case "FindItem":
                    Helper.GetActiveWindow<MainWindow>().SearchTextBox.Text = selectedText;
                    await Helper.GetActiveWindow<MainWindow>().FilterResult();
                    break;

                case "FindItemInNewWindow":
                    
                    var window = new MainWindow
                    {
                        NotifyIcon = { Visibility = Visibility.Collapsed },
                        WindowState = WindowState.Normal,
                        SearchTextBox = {Text = selectedText},
                        CloseButton = { Content = "x", Foreground = new SolidColorBrush(Colors.Red) }
                    };
                    await window.FilterResult();
                    window.Show();
                    break;

                case "CopyItem":
                    Clipboard.SetText(selectedText);
                    break;

                case "SearchItem":
                    Helper.OpenAppOrWebsite("http://google.com/search?q=" + selectedText);
                    break;

                case "FavoriteItem":
                    string favorites=Helper.GetSettings().Favorites;
                    selectedText = selectedText.Trim();
                    if (favorites.Contains(selectedText))
                        return;
                    
                    Helper.GetSettings().Favorites = (favorites.Trim()+"\n"+selectedText).Trim();
                    Helper.GetSettings().Save();
                    Zoom(Helper.GetActiveWindow<MainWindow>().FavoriteButton);
                    break;
            }
        }

        private void Zoom(Button target)
        {
            ScaleTransform trans = new ScaleTransform();
            target.RenderTransform = trans;
            target.RenderTransformOrigin = new Point(0.5, 0.5);
            

            // if you use the same animation for X & Y you don't need anim1, anim2 
            DoubleAnimation anim = new DoubleAnimation
            {
                From = 1,
                To = 1.3,
                EasingFunction = new QuadraticEase(),
                SpeedRatio = 10,
                AutoReverse = true
            };
            trans.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            trans.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

    }
}
