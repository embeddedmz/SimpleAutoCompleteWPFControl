using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleAutoCompleteWPFControl
{
    /// <summary>
    /// Logique d'interaction pour TextBoxAutoComplete.xaml
    /// </summary>
    public partial class TextBoxAutoComplete : UserControl, INotifyPropertyChanged
    {
        public string TextBoxContent
        {
            get { return (string)GetValue(_txtBoxContentProperty); }
            set { SetValue(_txtBoxContentProperty, value); }
        }
        public static readonly DependencyProperty _txtBoxContentProperty =
            DependencyProperty.Register("TextBoxContent", typeof(string), typeof(TextBoxAutoComplete), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<string> SuggestionResults
        {
            get { return _suggestionResults; }
        }
        private readonly ObservableCollection<string> _suggestionResults = new ObservableCollection<string>();

        private ISuggestionProvider _suggestionProvider;

        private bool _userHasPressedAKey = false;

        public TextBoxAutoComplete()
        {
            InitializeComponent();

            grid.DataContext = this;
            listBox.SelectionMode = SelectionMode.Single;
        }

        public object SelectedListBoxItem
        {
            get { return (object)GetValue(_selectedListBoxItemProperty); }
            set { SetValue(_selectedListBoxItemProperty, value); }
        }

        public static readonly DependencyProperty _selectedListBoxItemProperty =
            DependencyProperty.Register("SelectedListBoxItem", typeof(object),
                typeof(TextBoxAutoComplete), new UIPropertyMetadata(null));

        public ISuggestionProvider SuggestionProvider
        {
            get { return (ISuggestionProvider)GetValue(SuggestionProviderProperty); }
            set { SetValue(SuggestionProviderProperty, value); }
        }

        public static readonly DependencyProperty SuggestionProviderProperty =
            DependencyProperty.Register("SuggestionProvider", typeof(ISuggestionProvider),
                typeof(TextBoxAutoComplete), new UIPropertyMetadata(null, new PropertyChangedCallback(SuggestionProviderProvider_Changed)));

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotifyPropertyChanged("SelectedItem");

            // Verification.  
            if (this.listBox.SelectedIndex <= -1)
            {
                // Disable.  
                popup.IsOpen = false;

                // Info.  
                return;
            }

            if (popup.IsKeyboardFocusWithin)
            {
                SetTextAndHide();
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtSearch.TextChanged -= txtSearch_TextChanged;
            SetTextAndHide();
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Verification.  
            if (string.IsNullOrEmpty(this.txtSearch.Text))
            {
                popup.IsOpen = false;
                IsBusy = false;

                return;
            }

            //TextBoxContent = txtSearch.Text;

            if (_suggestionProvider != null)
            {
                IsBusy = true;
            }

            SuggestionResults.Clear();
            if (_suggestionProvider != null)
            {
                var results = _suggestionProvider.GetSuggestions(txtSearch.Text).ToList();                
                if (results.Count <= 0)
                {
                    popup.IsOpen = false;
                    return;
                }
                results.ForEach(item => SuggestionResults.Add(item));

                if (SuggestionResults.Count == 1)
                {
                    listBox.SelectedItem = listBox.Items[0];
                    //SetTextAndHide();
                }

                popup.VerticalOffset = txtSearch.ActualHeight;
                IsBusy = false;
                
                if (_userHasPressedAKey)
                {
                    popup.IsOpen = true;
                    _userHasPressedAKey = false;
                }
            }
            else
            {
                popup.IsOpen = false;
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!popup.IsKeyboardFocusWithin)
                popup.IsOpen = false;

            IsBusy = false;
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SelectTextOnFocus)
                txtSearch.SelectAll();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex--;
                }
            }

            if (e.Key == Key.Down)
            {
                if (listBox.SelectedIndex < listBox.Items.Count - 1)
                {
                    listBox.SelectedIndex++;
                }
            }
            
            if (e.Key == Key.Enter)
            {
                if (popup.IsOpen && listBox.Items.Count > 0)
                    SetTextAndHide();
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _userHasPressedAKey = true;

            if (e.Key == Key.Tab && listBox.SelectedItem != null)
            {
                this.popup.IsOpen = false;
                e.Handled = true;
            }
        }

        private void txtSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                popup.IsOpen = false;
                listBox.SelectedItem = null;
                e.Handled = true;
            }
        }

        private void SetTextAndHide()
        {
            popup.IsOpen = false;
            if (listBox.SelectedItem != null)
            {
                txtSearch.Text = listBox.SelectedItem.ToString();
                txtSearch.CaretIndex = txtSearch.Text.Length;
                //txtSearch.Focus();
            }
        }

        private static void SuggestionProviderProvider_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxAutoComplete me = d as TextBoxAutoComplete;
            me._suggestionProvider = e.NewValue as ISuggestionProvider;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region not important stuff for now
        public int PopupWidth
        {
            get { return (int)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }

        public static readonly DependencyProperty PopupWidthProperty =
            DependencyProperty.Register("PopupWidth", typeof(int), typeof(TextBoxAutoComplete), new UIPropertyMetadata(0));

        public PopupAnimation PopupAnimation
        {
            get { return popup.PopupAnimation; }
            set { popup.PopupAnimation = value; }
        }

        public bool IsBusy
        {
            get { return (bool)GetValue(_isBusyProperty); }
            set { SetValue(_isBusyProperty, value); }
        }
        public static readonly DependencyProperty _isBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(TextBoxAutoComplete), new UIPropertyMetadata(false));

        public bool SelectTextOnFocus
        {
            get { return (bool)GetValue(_selectTextOnFocusProperty); }
            set { SetValue(_selectTextOnFocusProperty, value); }
        }
        public static readonly DependencyProperty _selectTextOnFocusProperty =
            DependencyProperty.Register("SelectTextOnFocus", typeof(bool), typeof(TextBoxAutoComplete), new UIPropertyMetadata(false));

        public DataTemplate ItemTemplate
        {
            get { return this.listBox.ItemTemplate; }
            set { this.listBox.ItemTemplate = value; }
        }

        public ItemsPanelTemplate ItemsPanel
        {
            get { return this.listBox.ItemsPanel; }
            set { this.listBox.ItemsPanel = value; }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return this.listBox.ItemTemplateSelector; }
            set { this.listBox.ItemTemplateSelector = value; }
        }
        #endregion
    }
}
