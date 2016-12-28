using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDITroubleshooter.BL
{
    class ADUser
    {
        public string Name { get; set; }

        public string UserPrincipalName { get; set; }

        public DateTime AccountExpirationDate { get; set; }

        public DateTime LastBadPasswordAttempt { get; set; }

        public DateTime ActualLastBadPasswordAttempt { get; set; }

        public DateTime LastLogon { get; set; }

        public DateTime ActualLastLogon { get; set; }

        public string XDSitePreference { get; set; }

        public bool LockedOut { get; set; }

        public bool Enabled { get; set; }

        // TODO 
        public bool HomeDirPresent { get; set; }
    }
}