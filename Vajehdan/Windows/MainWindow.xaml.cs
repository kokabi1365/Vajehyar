using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Vajehdan.Properties;
using Vajehdan.Utility;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;


namespace Vajehdan.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private List<string> _list1;

        private ICollectionView _motaradefMotazadList;
        public ICollectionView MotaradefMotazadList
        {
            get => _motaradefMotazadList;
            set { _motaradefMotazadList = value; NotifyPropertyChanged("MotaradefMotazadList"); }
        }

        private ICollectionView _TeyfiList;
        public ICollectionView TeyfiList
        {
            get => _TeyfiList;
            set { _TeyfiList = value; NotifyPropertyChanged("TeyfiList"); }
        }

        private ICollectionView _EmlaeiList;
        public ICollectionView EmlaeiList
        {
            get => _EmlaeiList;
            set { _EmlaeiList = value; NotifyPropertyChanged("EmlaeiList"); }
        }

        public MainWindow(Database database)
        {
            InitializeComponent();

            MotaradefMotazadList = CollectionViewSource.GetDefaultView(database.words_motaradef);
            MotaradefMotazadList.Filter = FilterResult;
            var motaradefCollectionView = MotaradefMotazadList as ListCollectionView;
            motaradefCollectionView.CustomSort = new CustomSorter(this);

            TeyfiList = CollectionViewSource.GetDefaultView(database.words_teyfi);
            TeyfiList.Filter = FilterResult;
            var teyfiCollectionView = TeyfiList as ListCollectionView;
            teyfiCollectionView.CustomSort = new CustomSorter(this);

            EmlaeiList = CollectionViewSource.GetDefaultView(database.words_emlaei);
            EmlaeiList.Filter = EmlaeiFilterResult;
            var emlaeiCollectionView = EmlaeiList as ListCollectionView;
            emlaeiCollectionView.CustomSort = new CustomSorter(this);

            hint.Text = $"جستجوی بین {database.GetCount().Round().Format()} واژۀ فارسی";

            

#if (!DEBUG)
            CheckUpdate();
#endif
        }

        private async void CheckUpdate()
        {
            await GithubHelper.CheckUpdate();
        }

        private string _filterString;

        public string FilterString
        {
            get => _filterString;
            set
            {
                _filterString = value;
                NotifyPropertyChanged("FilterString");
                FilterCollection();
            }
        }

        private void FilterCollection()
        {
            _motaradefMotazadList?.Refresh();
            _TeyfiList?.Refresh();
            _EmlaeiList?.Refresh();
            txtSearch.SelectAll();
        }

        public bool FilterResult(object obj)
        {
            if (string.IsNullOrEmpty(_filterString))
                return false;

            var words = obj as List<string>;

            switch (PartSearch.IsChecked)
            {
                case false:
                    for (int i = 0; i < words.Count; i++)
                    {
                        if (words[i].RemoveDiacritics() == _filterString.RemoveDiacritics())
                        {
                            return true;
                        }
                    }

                    break;

                case true:
                    for (int i = 0; i < words.Count; i++)
                    {
                        if (words[i].RemoveDiacritics().Contains(_filterString.RemoveDiacritics()))
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        private bool EmlaeiFilterResult(object obj)
        {
            if (string.IsNullOrEmpty(_filterString))
                return false;

            var words = obj as List<string>;

            for (int i = 0; i < words.Count; i++)
            {
                if (words[i].RemoveDiacritics().Contains(_filterString.RemoveDiacritics()))
                {
                    return true;
                }
            }

            return false;
        }

        #region Events

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void TxtSearch_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            bool space = Keyboard.IsKeyDown(Key.Space);
            bool two = Keyboard.IsKeyDown(Key.D2);

            if (shift && space || ctrl && shift && two)
            {
                e.Handled = true;
                txtSearch.Text += "\u200c";
                txtSearch.SelectionStart = txtSearch.Text.Length;
                txtSearch.SelectionLength = 0;
            }
        }

        private void TxtSearch_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\u200C\u200F]+$"))
            {
                e.Handled = true;
            }
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            if (!Settings.Default.MinimizeWhenClickOutside)
                return;

            if (!Utility.Helper.IsWindowOpen<SettingWindow>())
            {
                if (!Settings.Default.ShowInTaskbar)
                    Hide();

                WindowState = WindowState.Minimized;
            }
        }


        private async Task ShowMessage(string message)
        {
            AutoClosemessage.Text = message;
            AutoCloseMessageContainer.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            AutoCloseMessageContainer.Visibility = Visibility.Collapsed;

        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void TxtSearch_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterString = txtSearch.Text;
            }
        }

        private async void Word_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            string word = (sender as Button).Content.ToString();
            Clipboard.SetText(word);
            string message = $".واژۀ «{word}» کپی شد";
            await ShowMessage(message);
        }

        private void Word_OnClick(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = FilterString = (sender as Button).Content.ToString();
        }

        private void TxtSearch_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txtSearch.SelectedText.Length == 0)
            {
                txtSearch.SelectAll();
            }
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            txtSearch.Undo();
            FilterString = txtSearch.Text;
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            txtSearch.Redo();
            FilterString = txtSearch.Text;
        }

        private void PartSearch_OnChecked(object sender, RoutedEventArgs e)
        {
            FilterCollection();
        }
    }

    internal class CustomSorter : IComparer
    {
        private MainWindow mainWindow;

        public CustomSorter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public int Compare(object x, object y)
        {
            string item1 = string.Join("", (x as List<string>)?.ToArray());
            string item2 = string.Join("", (y as List<string>)?.ToArray());
            string filter = mainWindow.FilterString;

            int a = item1?.IndexOf(filter) ?? -1;
            int b = item2?.IndexOf(filter) ?? -1;

            return a.CompareTo(b);
        }

    }

    public class DefaultFontConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value.ToString().Contains("فونت پیش‌فرض") || string.IsNullOrEmpty(value.ToString()) ? parameter : value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }



}
