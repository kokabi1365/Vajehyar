using System.Windows.Controls;

namespace Vajehdan.Views
{
    /// <summary>
    /// Interaction logic for FavoritesWindow.xaml
    /// </summary>
    public partial class FavoritesWindow
    {

        public FavoritesWindow()
        {
            InitializeComponent();
        }

        private void ContentTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Helper.GetSettings().Save();
        }
    }
}
