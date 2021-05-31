using System.Windows;

namespace Vajehdan.Controls
{
    /// <summary>
    /// Interaction logic for CheckBoxControl.xaml
    /// </summary>
    public partial class CheckBoxControl
    {
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Lyric", typeof(string), typeof(CheckBoxControl), new FrameworkPropertyMetadata("توضیح",
                FrameworkPropertyMetadataOptions.AffectsRender));

        public string IsChecked
        {
            get => (string)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckBoxControl), new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender));

        public CheckBoxControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
