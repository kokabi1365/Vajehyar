using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using Vajehdan.Properties;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;


namespace Vajehdan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private List<string> _list1;
        private IKeyboardMouseEvents globalHook;
        private BackgroundWorker bw;

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

        private ICollectionView _didYouMeanList;
        public ICollectionView DidYouMeanList
        {
            get => _didYouMeanList;
            set { _didYouMeanList = value; NotifyPropertyChanged("DidYouMeanList"); }
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
            await Helper.CheckUpdate();
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
            //txtSearch.SelectAll();
        }

        public bool FilterResult(object obj)
        {

            if (string.IsNullOrEmpty(_filterString))
                return false;

            var words = obj as List<string>;

            return words.AsParallel().Any(t => t.Contains(_filterString));

        }



        private bool EmlaeiFilterResult(object obj)
        {
            if (string.IsNullOrEmpty(_filterString))
                return false;

            var words = obj as string;
            
            return words.RemoveDiacritics().Contains(_filterString);
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

        private async void TxtSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text.Length==1)
                return;
            
            if (await txtSearch.IsIdle())
            {
                FilterString = txtSearch.Text;
                DidYouMeanList = CollectionViewSource.GetDefaultView(new List<string>() {"test1", "test2"});
            }
        }

        private void TxtSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterString = txtSearch.Text;
            }
        }
    }
}
