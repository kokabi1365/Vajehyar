﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using Syncfusion.UI.Xaml.Grid;
using Vajehdan.Properties;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Clipboard = System.Windows.Clipboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;


namespace Vajehdan.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly int _triggerThreshold = 500;
        private int _lastCtrlTick;

        private ObservableCollection<string> _words;
        public ObservableCollection<string> Words
        {
            get => _words;
            set
            {
                _words = value;
                NotifyPropertyChanged("Words");
            }
        }

        private GridVirtualizingCollectionView _motaradefMotazadList;

        public GridVirtualizingCollectionView MotaradefMotazadList
        {
            get => _motaradefMotazadList;
            set { _motaradefMotazadList = value; NotifyPropertyChanged("MotaradefMotazadList"); }
        }

        private GridVirtualizingCollectionView _TeyfiList;
        public GridVirtualizingCollectionView TeyfiList
        {
            get => _TeyfiList;
            set { _TeyfiList = value; NotifyPropertyChanged("TeyfiList"); }
        }

        private GridVirtualizingCollectionView _EmlaeiList;
        public GridVirtualizingCollectionView EmlaeiList
        {
            get => _EmlaeiList;
            set { _EmlaeiList = value; NotifyPropertyChanged("EmlaeiList"); }
        }

        private string _filterString;

        public string FilterString
        {
            get => _filterString;
            set
            {
                if (value.Length < 2 || string.IsNullOrWhiteSpace(value))
                    _filterString = null;
                else
                    _filterString = value.ToPlainText();
                
                NotifyPropertyChanged("FilterString");
                FilterCollection();
            }
        }


        private void FilterCollection()
        {
            _motaradefMotazadList?.Refresh();
            _TeyfiList?.Refresh();
            _EmlaeiList?.Refresh();
        }

        public MainWindow()
        {


            InitializeComponent();
            MotaradefMotazadList = new GridVirtualizingCollectionView(Database.GetWords(DatabaseType.Motaradef));
            MotaradefMotazadList.Filter = FilterResult;
            MotaradefMotazadList.CollectionChanged += MotaradefMotazadList_CollectionChanged;

            TeyfiList = new GridVirtualizingCollectionView(Database.GetWords(DatabaseType.Teyfi));
            TeyfiList.Filter = FilterResult;

            EmlaeiList = new GridVirtualizingCollectionView(Database.GetWords(DatabaseType.Emlaei));
            EmlaeiList.Filter = FilterResult;

            Words = new ObservableCollection<string>(Database.GetAllWords());


            var globalMouseHook = Hook.GlobalEvents();
            globalMouseHook.MouseDown += GlobalMouseHook_MouseDown;

            _keyboardHook = new KeyboardHook();
            _keyboardHook.SetHook();
            _keyboardHook.OnKeyDownEvent += (o, arg) =>
            {
                //if (Windows.OfType<SettingWindow>().Any()) return;

                if (arg.KeyData == Keys.Escape)
                {
                    HideMainWindow();
                    return;
                }

                if (!Settings.Default.OpenByDoubleAlt)
                    return;

                if (arg.Modifiers != Keys.Alt)
                    return;

                int thisCtrlTick = Environment.TickCount;
                int elapsed = thisCtrlTick - _lastCtrlTick;

                if (elapsed <= _triggerThreshold)
                {
                    ShowMainWindow();
                }

                _lastCtrlTick = thisCtrlTick;
            };

            HideMainWindow();




#if (!DEBUG)
            CheckUpdate();

#endif
        }

        private void MotaradefMotazadList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    if (MotaradefMotazadList.Count == 0 && TeyfiList.Count==0 && EmlaeiList.Count==0)
                    {
                        ResultPanel.Visibility = Visibility.Collapsed;
                        return;
                    }

                    ResultPanel.Visibility = Visibility.Visible;

                   
                    MotaradefColumn.Width = MotaradefMotazadList.Count == 0 ? new GridLength(0) : new GridLength(3, GridUnitType.Star);
                    TeyfiColumn.Width = TeyfiList.Count == 0 ? new GridLength(0) : new GridLength(3, GridUnitType.Star);
                    EmlaeiColumn.Width = EmlaeiList.Count == 0 ? new GridLength(0) : new GridLength(2, GridUnitType.Star);
                }));
        }

        private async void CheckUpdate()
        {
            await Helper.CheckUpdate();
        }

        private KeyboardHook _keyboardHook;


        public bool FilterResult(object obj)
        {
            if (FilterString==null)
                return false;

            if ((bool) PartSearch.IsChecked)
            {
                return ((string[])obj).Any(s => s.Contains(FilterString));
            }

            return ((string[]) obj).Any(s => s == FilterString);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void TxtSearch_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtSearch.Text) && e.Key==Key.Space)
            {
                e.Handled = true;
                txtSearch.Clear();
            }
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

        private void Word_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            string word = (sender as Button).Content.ToString();
            Clipboard.SetText(word);
            myToast.Show();
        }

        private void Word_OnClick(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = FilterString = ((sender as Button).Content.ToString()).Trim();
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

        private void TxtSearch_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.IsKeyboardFocusWithin)
            {
               return;
            }
            Dispatcher?.BeginInvoke((ThreadStart)(() =>
            {
                Keyboard.Focus(txtSearch);
            }));
        }

        private void TxtSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterString = txtSearch.Text;
            }
        }

        private void Menu_RunCommand(object sender, RoutedEventArgs e)
        {
            switch (((FrameworkElement)sender).Name)
            {
                case "ItemSetting":
                    new SettingWindow().Show();
                    break;

                case "ItemAbout":
                    new AboutWindow().Show();
                    break;

                case "ItemExit":
                    Application.Current.Shutdown();
                    break;
            }
        }

        public void ShowMainWindow()
        {
            WindowState = WindowState.Normal;
            Show();
            txtSearch.Focus();
            txtSearch.SelectAll();
        }

        public void HideMainWindow()
        {
            Hide();
            WindowState = WindowState.Minimized;
        }

        private void GlobalMouseHook_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (txtSearch.IsSuggestionOpen)
                return;
            

            if (!Settings.Default.MinimizeWhenClickOutside)
                return;

            if (e.X < Left || e.X > Left + Width || e.Y < Top || e.Y > Top + Height)
            {
                HideMainWindow();
            }
        }


        private void NotifyIconClickCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ShowMainWindow();
        }

        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();
        double autoHeight;
        

        private void Datagrid_OnQueryRowHeight(object sender, QueryRowHeightEventArgs e)
        {
            if ((sender as SfDataGrid).GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out autoHeight))
            {
                if (autoHeight > 24)
                {
                    e.Height = autoHeight;
                    e.Handled = true;
                }
            }
        }


        private void PartSearch_OnChecked(object sender, RoutedEventArgs e)
        {
            FilterCollection();
        }
    }

}