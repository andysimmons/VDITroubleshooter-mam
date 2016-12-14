using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDITroubleshooter.BL
{
    public class UserSuggestion
    {
        public string CN { get; set; }

        public string SAMAccountName { get; set; }

        public string Department { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return SAMAccountName;
        }

        /// <summary>
        /// Search AD for possible user matches.
        /// </summary>
        /// <param name="AmbiguousName">Search string</param>
        /// <param name="MaxResults">Max results</param>
        /// <returns>
        /// Returns a list of potential AD user matches.
        /// </returns>
        public static List<UserSuggestion> Retrieve(string AmbiguousName, int MaxResults)
        {
            var suggestions = new List<UserSuggestion>();

            var search = new DirectorySearcher($"(&(anr={AmbiguousName})(ObjectCategory=Person))");

            var searchProps = new string[] { "samAccountName", "cn", "department", "title" };

            search.PropertiesToLoad.AddRange(searchProps);
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
    }
}
