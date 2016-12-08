using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDITroubleshooter
{
    internal class VirtualDesktop
    {
        public string HostedMachineName { get; set; }

        public string SessionState { get; set; }

        public string DesktopGroupName { get; set; }
    }
}
