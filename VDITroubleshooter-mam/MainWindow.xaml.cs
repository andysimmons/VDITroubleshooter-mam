using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VDITroubleshooter_mam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            textboxUserSearch.TextChanged += textboxUserSearch_TextChanged;
        }

        /// <summary>
        ///     Search AD for possible user matches.
        /// </summary>
        /// <param name="AmbiguousName">Search string</param>
        /// <param name="MaxResults">Max results</param>
        /// <returns>
        ///     Returns a list of potential AD user matches.
        /// </returns>
        private static List<string> GetUserSuggestions(string AmbiguousName, int MaxResults)
        {
            var suggestions = new List<string>();
            suggestions.Clear();

            var search = new DirectorySearcher("(anr=" + AmbiguousName + ")");
            search.SizeLimit = MaxResults;
            search.Asynchronous = true;

            foreach (SearchResult result in search?.FindAll())
            {
                string item = result.Properties["samAccountName"][0] + " (" + result.Properties["cn"][0] + ")";
                suggestions.Add(item);
            }

            return suggestions;
        }



        void textboxUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string partialUserName = textboxUserSearch.Text;
            var autoList = new List<string>();

            if (partialUserName.Length >= 3)
            {
                autoList = GetUserSuggestions(partialUserName, 5);
            }

            if (autoList.Count > 0)
            {
                listboxSuggestions.ItemsSource = autoList;
                listboxSuggestions.Visibility = Visibility.Visible;
            }
            // TODO Delete this?
            else if (textboxUserSearch.Text.Equals(""))
            {
                listboxSuggestions.Visibility = Visibility.Collapsed;
                listboxSuggestions.ItemsSource = null;
            }
            else
            {
                listboxSuggestions.Visibility = Visibility.Collapsed;
                listboxSuggestions.ItemsSource = null;
            }
        }

        void listboxSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listboxSuggestions.ItemsSource != null)
            {
                listboxSuggestions.KeyDown += listboxSuggestions_KeyDown;
            }
        }

        private void textboxUserSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                listboxSuggestions.Focus();
            }
            else if (e.Key == Key.Enter)
            {

            }

        }

        /// <summary>
        ///     Accept selected auto-complete suggestion and close the auto-complete box.
        /// </summary>
        private void AcceptSuggestion()
        {
            if (listboxSuggestions.SelectedItem != null)
            {
                textboxUserSearch.Text = listboxSuggestions.SelectedItem.ToString().Split()[0];
            }

            CloseAutoCompleteBox();
        }

        /// <summary>
        ///     Close the auto-complete box (without changing search text).
        /// </summary>
        private void CloseAutoCompleteBox()
        {
            listboxSuggestions.Visibility = Visibility.Collapsed;
            textboxUserSearch.Focus();
        }

        private void GetUserData()
        {

        }

        private void listboxSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (ReferenceEquals(sender, listboxSuggestions))
            {
                if (e.Key == Key.Enter)
                {
                    textboxUserSearch.Text = listboxSuggestions.SelectedItem.ToString().Split()[0];
                    listboxSuggestions.Visibility = Visibility.Collapsed;
                }

                if (e.Key == Key.Down)
                {
                    e.Handled = true;
                    listboxSuggestions.Items.MoveCurrentToNext();
                }
                if (e.Key == Key.Up)
                {
                    e.Handled = true;
                    listboxSuggestions.Items.MoveCurrentToPrevious();
                }
            }
        }

        // Explicitly searches AD for a given username and returns the object
        private SearchResult GetADUser(string samAccountName)
        {
            try
            {
                var search = new DirectorySearcher("(samAccountName=" + samAccountName + ")");
                search.Asynchronous = false;
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        // Updates form controls that display search result details
        private void UpdateResultControls(SearchResult user)
        {

        }

        // Clears form controls that display search result details
        private void ClearResultControls()
        {

        }

        private void listboxSuggestions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ReferenceEquals(sender, listboxSuggestions) && (listboxSuggestions.SelectedItem != null))
            {
                textboxUserSearch.Text = listboxSuggestions.SelectedItem.ToString().Split()[0];
                listboxSuggestions.Visibility = Visibility.Collapsed;
            }
        }
    }
}
