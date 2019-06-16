using System;
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
using Vajehyar.Properties;
using Vajehyar.Utility;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;


namespace Vajehyar.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _typedNonPersian;
        public bool TypedNonPersian
        {
            get => _typedNonPersian;
            set
            {
                _typedNonPersian = value;
                NotifyPropertyChanged("TypedNonPersian");
            }
        }

        private ICollectionView _words;
        public ICollectionView Words
        {
            get => _words;
            set { _words = value; NotifyPropertyChanged("Words"); }
        }

        private string _hint;

        public string Hint
        {
            get => _hint;
            set { _hint = value; NotifyPropertyChanged("Hint"); }
        }
        
        public MainWindow(Database database)
        {
            InitializeComponent(); 
            Words = CollectionViewSource.GetDefaultView(database.Words);
            Words.Filter = FilterResult;
            Hint = $"جستجوی فارسی بین {database.GetCount().Round().Format()} واژه";

#if (!DEBUG)
            CheckUpdate();
#endif
        }

        private async void CheckUpdate()
        {
            if (Settings.Default.CheckUpdate)
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
            _words?.Refresh();
        }

       
        public bool FilterResult(object obj)
        {
            if (string.IsNullOrEmpty(_filterString))
                return false;
            
            string pattern = _filterString;

            if (WholeWord.IsChecked==true)
            {
                pattern = @"\b" + _filterString + @"\b";
            }

            return Regex.IsMatch(JoinWords(obj as Word), pattern);
        }

        private string JoinWords(Word word)
        {
            string result;
            IEnumerable<string> list=new List<string>();
            foreach (var groups in word.SynAcros)
            {
                list = groups.Syns.Concat(groups.Acros);
            }

            return string.Join(",", list.ToArray());
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

        private async void TxtSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (await txtSearch.GetIdle(Settings.Default.SearchDelay))
            {
                FilterString = txtSearch.Text;
            }
        }

        private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            FilterString = txtSearch.Text;
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

            if (!Settings.Default.ShowInTaskbar)
                Hide();

            WindowState = WindowState.Minimized;
        }
       

        private async Task ShowMessage(string message)
        {
            AutoClosemessage.Text = message;
            AutoCloseMessageContainer.Visibility = Visibility.Visible;
            await Task.Delay(2000);
           AutoCloseMessageContainer.Visibility = Visibility.Collapsed;
           
        }

        private async void Word_OnClick(object sender, RoutedEventArgs e)
        {
            string word = (sender as Button).Content.ToString();
            Clipboard.SetText(word);
            string message = $"واژۀ «{word}» کپی شد.";
            await ShowMessage(message);
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
