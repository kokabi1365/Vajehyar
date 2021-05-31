using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Vajehdan.Models;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Vajehdan.Views
{
    [SupportedOSPlatform("windows7.0")]
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Properties and fields
        public string SearchQuery { get; set; }
        private ObservableCollection<SearchResult> _resultCollection;
        public ObservableCollection<SearchResult> ResultCollection
        {
            get => _resultCollection;
            set
            {
                _resultCollection = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> OtherFormsCollection { get; set; } = new();
        private LinkedList<string> _autoCompleteList;
        public LinkedList<string> AutoCompleteList
        {
            get => _autoCompleteList;
            set
            {
                _autoCompleteList = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Events

        #region Top Right Buttons
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void PinButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Topmost == false)
            {
                PinButton.Foreground = new SolidColorBrush(Colors.Red);
                PinButton.FontSize = 14;
                Topmost = true;
            }
            else
            {
                PinButton.Foreground = new SolidColorBrush(Colors.Black);
                PinButton.FontSize = 12;
                Topmost = false;
            }
        }
        private void NewWindowButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow
            {
                NotifyIcon = { Visibility = Visibility.Collapsed },
                WindowState = WindowState.Normal,
                CloseButton = { Content = "x", Foreground = new SolidColorBrush(Colors.Red) }
            };
            window.Show();
        }
        #endregion

        #region SearchBox Buttons
        private async void BackwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            var list = GetLatestHistory();
            var item = list.Find(SearchQuery)?.Previous;

            if (item == null)
                return;

            SearchTextBox.Text = item.Value;

            PreviousButton.IsEnabled = item.Previous != null;

            await FilterResult();
        }
        private async void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            var list = GetLatestHistory();
            var item = list.Find(SearchQuery)?.Next;

            if (item == null)
                return;

            SearchTextBox.Text = item.Value;

            NextButton.IsEnabled = item.Next != null;
            await FilterResult();
        }
        #endregion

        #region MainWindow Events
        public MainWindow()
        {
            InitializeComponent();
            NotifyIconCommandBinding.Executed += (_, _) => ShowMainWindow();
        }
        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (CloseButton.Content.ToString() == "_")
            {
                Helper.MakeWindowCenterScreen(this);
                
                if (Helper.GetSettings().FirstRun)
                {
                    Helper.GetSettings().FirstRun = false;
                    Helper.GetSettings().Save();
                    return;
                }

                HideMainWindow();
            }

            var item = GetLatestHistory()?.Last;
            if (item != null)
            {
                SearchTextBox.Text = item.Value;
                await FilterResult();
            }
            else
            {
                PreviousButton.IsEnabled = NextButton.IsEnabled = false;
            }

        }
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Helper.OpenAppOrWebsite(Helper.GetSettings().Website);

        }
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            switch (CloseButton.Content.ToString())
            {
                case "_":
                    HideMainWindow();
                    e.Cancel = true;
                    break;
                case "x":
                    HideMainWindow();
                    break;
            }
        }
        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();

        }
        #endregion

        #region SearchBox Events
        private void SearchTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text) && e.Key == Key.Space)
            {
                SearchTextBox.Clear();
                e.Handled = true;
            }

            //Allow typing half space
            var shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            var ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            var space = Keyboard.IsKeyDown(Key.Space);
            var two = Keyboard.IsKeyDown(Key.D2);

            // First condition:  Half space in standard persian keyboard
            // Second condition: Half space in non-standard persian keyboard
            if (shift && space || ctrl && shift && two)
            {
                e.Handled = true;
                SearchTextBox.Text += "\u200c";
                SearchTextBox.SelectionStart = SearchTextBox.Text.Length;
                SearchTextBox.SelectionLength = 0;
            }
        }
        private async void SearchTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await FilterResult();
            }
        }
        private async void SearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await SuggestCorrectSpell();
        }
        #endregion

        private void Menu_RunCommand(object sender, RoutedEventArgs e)
        {
            switch (((FrameworkElement)sender).Name)
            {
                case "ItemDisableHotkey":
                    Helper.GetSettings().OpenByHotKey = !Helper.GetSettings().OpenByHotKey;
                    Helper.GetSettings().Save();
                    break;

                case "ItemSetting":
                    Helper.ShowWindow<SettingWindow>();
                    break;

                case "ItemAbout":
                    Helper.ShowWindow<AboutWindow>();
                    break;

                case "ItemExit":
                    NotifyIcon.Visibility = Visibility.Collapsed;
                    NotifyIcon.Icon = null;
                    Application.Current.Shutdown();
                    break;
            }
        }
        private void FlowDocumentReader_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var flowDocumentReader = sender as FlowDocumentReader;
            var potStart = flowDocumentReader?.Selection?.Start;
            var potEnd = flowDocumentReader?.Selection?.End;
            var range = new TextRange(potStart, potEnd);

            if (range.Text.Trim().Length == 0)
            {
                e.Handled = true;
            }

        }
        private async void OtherFormsButton_OnClick(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = ((Button)sender).Content.ToString() ?? string.Empty;
            await FilterResult();
        }
        void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
            => Helper.ShowWindow<FavoritesWindow>();
        private void UpdatedButton_OnClick(object sender, RoutedEventArgs e)
            => Helper.OpenAppOrWebsite(Helper.GetSettings().Website);

        #endregion

        #region Helper Methods

        private void DisableOrEnablePreviousNextButtons()
        {
            var list = GetLatestHistory();
            if (list.Count > 1)
            {
                if (list.Find(SearchQuery) == list.Last)
                {
                    NextButton.IsEnabled = false;
                    PreviousButton.IsEnabled = true;
                }

                else if (list.Find(SearchQuery) == list.First)
                {
                    PreviousButton.IsEnabled = false;
                    NextButton.IsEnabled = true;
                }
                else
                {
                    NextButton.IsEnabled = PreviousButton.IsEnabled = true;
                }
            }
            else
            {
                PreviousButton.IsEnabled = NextButton.IsEnabled = false;
            }
        }

        private LinkedList<string> GetLatestHistory()
        {
            return new(Helper.GetSettings().AutoCompleteList ?? new List<string>());
        }

        public async Task SuggestCorrectSpell()
        {
            var searchTerm = SearchTextBox.Text.Trim().RemoveDiacritics().ToLower();
            OtherFormsCollection.Clear();
            await Task.Run(() =>
            {
                string str = Regex.Replace(searchTerm.Trim(), @"\s+", " "); //Convert more than one space to one space
                var normalized = str.Replace(' ', '|').Replace('\u200c', '|');

                if (normalized.Contains('|'))
                {

                    List<string> combinations = Helper.Combinations(normalized, '|').Distinct().ToList();
                    combinations.RemoveAll(s => s == str);

                    foreach (var word in combinations.Where(word => Database.EmlaeiEntries.Any(s => s == word)))
                    {
                        Dispatcher.InvokeAsync(() =>
                        {
                            OtherFormsCollection.Add(word);
                        });
                    }
                }
                else
                {
                    for (int i = 1; i < normalized.Length; i++)
                    {
                        var part1 = normalized.Substring(0, i);
                        var part2 = normalized.Substring(i);
                        var withSpace = part1 + ' ' + part2;
                        var withHalfSpace = part1 + '\u200c' + part2;

                        if (Database.EmlaeiEntries.Any(s => s == withSpace))
                            Dispatcher.InvokeAsync(() =>
                            {
                                OtherFormsCollection.Add(withSpace);
                            });

                        if (Database.EmlaeiEntries.Any(s => s == withHalfSpace))
                            Dispatcher.InvokeAsync(() =>
                            {
                                OtherFormsCollection.Add(withHalfSpace);
                            });
                    }
                }
            });
        }

        public async Task FilterResult()
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                return;

            SearchQuery = SearchTextBox.Text.Trim().RemoveDiacritics().ToLower();

            SearchTextBox.IsEnabled = false;
            ResultCollection = new ObservableCollection<SearchResult>();
            TabControl.SelectedIndex = 0;

            foreach (var dic in Database.Dictionaries)
            {
                if (dic.Name == "فرهنگستان" && !SearchQuery.IsAllCharEnglish())
                    continue;

                await Task.Run(() =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        LoadingTextBlock.Text = $"جستجو در {dic.Name} ...";
                    });

                    var result = SearchQuery.Contains('|')
                        ? Database.SearchGanjvar(dic, SearchQuery)
                        : Database.Search(dic, SearchQuery);

                    Dispatcher.InvokeAsync(() =>
                    {
                        if (result.ResultDoc.Blocks.Count == 0)
                        {
                            ResultCollection.Remove(result);
                        }
                        else
                        {
                            ResultCollection.Add(result);
                        }

                    });
                }).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        t.Exception.Log();
                        //Continue execution
                    }
                });
            }


            SearchTextBox.IsEnabled = true;
            SearchTextBox.SelectAll();
            SearchTextBox.Focus();

            AutoCompleteList = GetLatestHistory();
            if (!AutoCompleteList.Contains(SearchQuery))
            {
                AutoCompleteList.AddLast(SearchQuery);
                Helper.GetSettings().AutoCompleteList =
                    new List<string>(AutoCompleteList);
                Helper.GetSettings().Save();
            }

            foreach (Window window in Application.Current.Windows)
            {
                (window as MainWindow)?.DisableOrEnablePreviousNextButtons();
            }
        }

        public void ShowMainWindow()
        {
            Helper.SlideUpAnimation(this);

            Show();

            if (WindowState != WindowState.Normal)
                WindowState = WindowState.Normal;

            SearchTextBox.SelectAll();
            Keyboard.Focus(SearchTextBox);
        }

        public void HideMainWindow()
        {
            Hide();
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region AutoGenerated codes
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
