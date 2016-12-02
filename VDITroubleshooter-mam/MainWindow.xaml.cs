using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VDITroubleshooter
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

            PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        /// <summary>
        ///     Searches AD for an explicit username and returns the result.
        /// </summary>
        /// <param name="samAccountName">Active Directory username</param>
        protected SearchResult GetADUser(string samAccountName)
        {
            var search = new DirectorySearcher("(samAccountName=" + samAccountName + ")");
            search.Asynchronous = false;
            return search?.FindOne();
        }


        /// <summary>
        ///     Search AD for possible user matches.
        /// </summary>
        /// <param name="AmbiguousName">Search string</param>
        /// <param name="MaxResults">Max results</param>
        /// <returns>
        ///     Returns a list of potential AD user matches as strings.
        /// </returns>
        private static List<string> GetUserSuggestions(string AmbiguousName, int MaxResults)
        {
            var suggestions = new List<string>();

            var search = new DirectorySearcher("(anr=" + AmbiguousName + ")");
            search.PropertiesToLoad.Add("samAccountName");
            search.PropertiesToLoad.Add("cn");
            search.PropertiesToLoad.Add("department");
            search.PropertiesToLoad.Add("title");
            search.SizeLimit = MaxResults;
            search.Asynchronous = true;

            try
            {
                foreach (SearchResult result in search?.FindAll())
                {
                    string samAccountName = result.Properties["samAccountName"][0].ToString();
                    string cn = result.Properties["cn"][0].ToString();

                    try
                    {
                        string title = result.Properties["title"]?[0]?.ToString();
                        string department = result.Properties["department"]?[0]?.ToString();
                                                
                        suggestions.Add($"{samAccountName} ({cn}  ::  {title}  ::  {department}) ");
                    }

                    catch
                    {
                        suggestions.Add($"{samAccountName} ({cn}) ");
                    }
                }
            }

            catch
            {
                suggestions.Clear();
            }

            search.Dispose();

            return suggestions;
        }

        /// <summary>
        ///     Displays the auto-complete dropdown under the user search textbox.
        /// </summary>
        /// <param name="Suggestions">List of suggested matches.</param>
        private void ShowUserSuggestions(List<string> Suggestions)
        {
            if (Suggestions.Count > 0)
            {
                listboxSuggestions.ItemsSource = Suggestions;
                listboxSuggestions.Visibility = Visibility.Visible;
            }
            else
            {
                HideUserSuggestions();
            }
        }

        /// <summary>
        ///     Accept selected auto-complete suggestion and close the auto-complete box.
        /// </summary>
        private void AcceptSuggestion()
        {
            if (listboxSuggestions.SelectedItem != null)
            {
                // set the search text to just the SAM account name from the suggestion string
                textboxUserSearch.Text = listboxSuggestions.SelectedItem.ToString().Split()[0];
            }

            HideUserSuggestions();
        }

        /// <summary>
        ///     Close the auto-complete box (without changing search text).
        /// </summary>
        private void HideUserSuggestions()
        {
            listboxSuggestions.Visibility = Visibility.Collapsed;
            listboxSuggestions.ItemsSource = null;
            textboxUserSearch.Focus();
            textboxUserSearch.CaretIndex = textboxUserSearch.Text.Length;
        }

        private void GetUserData()
        {
            // TODO
        }

        /// <summary>
        ///     Updates form controls that display search result details
        /// </summary>
        /// <param name="Username">SAM account name of the AD user.</param>
        private void UpdateResultControls(SearchResult Username)
        {
            // TODO
        }

        /// <summary>
        ///     Clears form controls that display search result details
        /// </summary>
        private void ClearResultControls()
        {
            // TODO
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HideUserSuggestions();
            }
        }

        private void textboxUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string partialUserName = textboxUserSearch.Text;

            // Try ambigious name resolution if we have at least 3 letters to search on
            if (partialUserName.Length >= 3)
            {
                List<string> anrHits = GetUserSuggestions(partialUserName, 8);

                ShowUserSuggestions(anrHits);                
            }

            else
            {
                HideUserSuggestions();
            }
        }

        private void textboxUserSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Down))
            {
                listboxSuggestions.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                HideUserSuggestions();
            }
        }

        private void listboxSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listboxSuggestions.ItemsSource != null)
            {
                listboxSuggestions.KeyDown += listboxSuggestions_KeyDown;
            }
        }

        private void listboxSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if (ReferenceEquals(sender, listboxSuggestions))
            {

                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    AcceptSuggestion();
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

                if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    HideUserSuggestions();
                }
            }
        }


        private void listboxSuggestions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ReferenceEquals(sender, listboxSuggestions) && (listboxSuggestions.SelectedItem != null))
            {
                e.Handled = true;
                textboxUserSearch.Text = listboxSuggestions.SelectedItem.ToString().Split()[0];
                listboxSuggestions.Visibility = Visibility.Collapsed;
            }
        }


        private void listboxSuggestions_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(sender, listboxSuggestions))
            {
            }

            else
            {
                HideUserSuggestions();
            }
        }

        private void listviewUsersDesktops_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listviewUsersOtherVMs.SelectedItem != null && listviewUsersDesktops.SelectedItem != null)
            {
                listviewUsersOtherVMs.UnselectAll();
            }
        }

        private void listviewUsersOtherVMs_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listviewUsersDesktops.SelectedItem != null && listviewUsersOtherVMs.SelectedItem != null)
            {
               listviewUsersDesktops.UnselectAll();
            }
        }
    }
}
