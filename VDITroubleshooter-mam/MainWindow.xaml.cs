using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VDITroubleshooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            textboxUserSearch.TextChanged += textboxUserSearch_TextChanged;

            PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private CancellationTokenSource ctsGetSuggestions;

        private CancellationTokenSource ctsSuppressor;

        private string acceptedText = "";

        /// <summary>
        ///     Searches AD for an explicit username and returns the result.
        /// </summary>
        /// <param name="samAccountName">Active Directory username</param>
        protected SearchResult GetADUser(string samAccountName)
        {
            var search = new DirectorySearcher($"(samAccountName={samAccountName})");
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
        private static List<UserSuggestion> GetUserSuggestions(string AmbiguousName, int MaxResults)
        {
            var suggestions = new List<UserSuggestion>();

            var search = new DirectorySearcher($"(&(anr={AmbiguousName})(ObjectCategory=Person))");
            search.PropertiesToLoad.Add("samAccountName");
            search.PropertiesToLoad.Add("cn");
            search.PropertiesToLoad.Add("department");
            search.PropertiesToLoad.Add("title");
            search.SizeLimit = MaxResults;
            search.ClientTimeout = TimeSpan.FromSeconds(1);
            search.Asynchronous = true;

            try
            {
                foreach (SearchResult result in search?.FindAll())
                {
                    string samAccountName = result.Properties["samAccountName"][0].ToString();
                    string cn = result.Properties["cn"][0].ToString();
                    string title;
                    string department;

                    try
                    {
                        title = result.Properties["title"]?[0]?.ToString();
                        department = result.Properties["department"]?[0]?.ToString();
                    }

                    catch
                    {
                        title = null;
                        department = null;
                    }

                    var suggestion = new UserSuggestion
                    {
                        SAMAccountName = samAccountName,
                        CN = cn,
                        Department = department,
                        Title = title
                    };

                    suggestions.Add(suggestion);
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
        private void ShowUserSuggestions(List<UserSuggestion> Suggestions)
        {
            bool isAlreadyAccepted = string.Equals(acceptedText, textboxUserSearch.Text, StringComparison.OrdinalIgnoreCase);

            if (isAlreadyAccepted || Suggestions.Count == 0)
            {
                HideUserSuggestions();
            }
            else
            {
                listboxSuggestions.ItemsSource = Suggestions;
                listboxSuggestions.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     Accept selected auto-complete suggestion and close the auto-complete box.
        /// </summary>
        private void AcceptSuggestion()
        {
            if (listboxSuggestions.SelectedItem != null)
            {
                // Stop any threads that generate new suggestions
                try
                {
                    ctsGetSuggestions?.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }

                // Accept the suggestion
                acceptedText = listboxSuggestions.SelectedItem.ToString().Split()[0].ToLower();

                textboxUserSearch.TextChanged -= textboxUserSearch_TextChanged;
                textboxUserSearch.Text = acceptedText;
                textboxUserSearch.TextChanged += textboxUserSearch_TextChanged;

                // Hold off on more suggestions for a couple seconds (for the current string)
                SuppressRedundantSuggestions_Async();
            }

            HideUserSuggestions();
        }

        /// <summary>
        ///     Temporarily suppress automatic background searching for a given search string.
        /// </summary>
        /// <param name="Seconds">Duration (sec) before resuming normal behavior.</param>
        private async void SuppressRedundantSuggestions_Async(int Seconds = 2)
        {
            try
            {
                ctsSuppressor?.Cancel();
            }
            catch (ObjectDisposedException) { }

            string previouslyAcceptedText = acceptedText;

            using (ctsSuppressor = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(Seconds), ctsSuppressor.Token);

                    // If the most recently accepted text hasn't changed since we've started, quit suppressing
                    // suggestions for it.
                    if (string.Equals(previouslyAcceptedText, acceptedText, StringComparison.OrdinalIgnoreCase))
                    {
                        acceptedText = "";
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///     Close the auto-complete box (without changing search text).
        /// </summary>
        private void HideUserSuggestions()
        {
            listboxSuggestions.ItemsSource = null;
            listboxSuggestions.Visibility = Visibility.Collapsed;

            if (!textboxUserSearch.IsFocused)
            {
                textboxUserSearch.Focus();
                textboxUserSearch.CaretIndex = textboxUserSearch.Text.Length;
            }
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

        private async void textboxUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Cancel previous searches
            try
            {
                ctsGetSuggestions?.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }

            string partialUserName = textboxUserSearch.Text;

            // Try ambigious name resolution if we have at least 3 letters to search on
            using (ctsGetSuggestions = new CancellationTokenSource())
            {
                try
                {
                    if (partialUserName.Length >= 3)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(250), ctsGetSuggestions.Token);

                        if (partialUserName == textboxUserSearch.Text)
                        {
                            List<UserSuggestion> anrHits = GetUserSuggestions(partialUserName, 8);

                            ShowUserSuggestions(anrHits);
                        }
                    }
                    else
                    {
                        HideUserSuggestions();
                    }
                }
                catch
                {
                }
            }
        }

        private void textboxUserSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Down) && listboxSuggestions.ItemsSource != null)
            {
                if (listboxSuggestions.SelectedItem == null)
                {
                    listboxSuggestions.SelectedIndex = 0;
                }
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
                AcceptSuggestion();
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

        // Don't show the clear search button if it's disabled
        private void buttonClearSearch_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (buttonClearSearch.IsEnabled)
            {
                buttonClearSearch.Visibility = Visibility.Visible;
            }
            else
            {
                buttonClearSearch.Visibility = Visibility.Hidden;
            }
        }

        private void buttonClearSearch_Click(object sender, RoutedEventArgs e)
        {
            textboxUserSearch.Clear();
            textboxUserSearch.Focus();
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            HideUserSuggestions();
        }
    }
}
