using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Gma.System.MouseKeyHook;
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
        private IKeyboardMouseEvents globalHook;

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
            globalHook = Hook.GlobalEvents();
            globalHook.MouseDown += GlobalHook_MouseDown;

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

#if (!DEBUG)
            CheckUpdate();

#endif
        }

        private void GlobalHook_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!Settings.Default.MinimizeWhenClickOutside)
                return;
            
            if (e.X < Left || e.X>Left+Width || e.Y<Top || e.Y>Top+Height)
            {
                Hide();
                WindowState = WindowState.Minimized;
            }

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
                        if (words[i] == _filterString)
                        {
                            return true;
                        }
                    }

                    break;

                case true:
                    for (int i = 0; i < words.Count; i++)
                    {
                        if (words[i].Contains(_filterString))
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

            var words = obj as string;

            
                if (words.RemoveDiacritics().Contains(_filterString))
                {
                    return true;
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
        
        private async Task ShowMessage(string message)
        {
            AutoClosemessage.Text = message;
            AutoCloseMessageContainer.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            AutoCloseMessageContainer.Visibility = Visibility.Collapsed;

        }

        private void TxtSearch_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterString = txtSearch.Text;
                txtSearch.SelectAll();
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
            txtSearch.Focus();
            txtSearch.SelectAll();
        }

        private void TxtSearch_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txtSearch.SelectedText.Length == 0)
            {
                txtSearch.SelectAll();
            }
        }

        private void PartSearch_OnChecked(object sender, RoutedEventArgs e)
        {
            FilterCollection();
        }

        private void TxtSearch_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher?.BeginInvoke((ThreadStart)(() =>
            {
                Keyboard.Focus(txtSearch);
            }));
        }
    }
}
