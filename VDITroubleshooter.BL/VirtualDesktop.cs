using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Citrix.Broker.Admin.SDK;

namespace VDITroubleshooter.BL
{
    public class VirtualDesktop
    {
        public VirtualDesktop()
        {
        }

        public VirtualDesktop(string hostedMachineName, string adminAddress, string sessionState, string desktopGroupName)
        {
            HostedMachineName = hostedMachineName;
            AdminAddress = adminAddress;
            SessionState = sessionState;
            DesktopGroupName = desktopGroupName;
        }

        [Required(ErrorMessage = "HostedMachineName is a required property.")]
        public string HostedMachineName { get; set; }

        [Required(ErrorMessage = "AdminAddress is a required property.")]
        public string AdminAddress { get; set; }

        public string SessionState { get; set; }

        public string DesktopGroupName { get; set; }

        public string DesktopType { get; set; }

        public static List<VirtualDesktop> GetPlaceholders()
        {
            var placeholders = new List<VirtualDesktop>();

            placeholders.Add(new VirtualDesktop("XDBP07GCD00193", "CTXDDC01", "Active", "TV Clinician Desktop"));
            placeholders.Add(new VirtualDesktop("XDBP07GCD00194", "CTXDDC02", "Disconnected", "TV Clinician Desktop"));

            return placeholders;
        }

        public override string ToString()
        {
            string formattedHostedMachineName = "";
            string formattedSessionState = "";
            string formattedDesktopGroupName = "";

            if (!string.IsNullOrWhiteSpace(HostedMachineName))
            {
                formattedHostedMachineName = HostedMachineName;

                if (!string.IsNullOrWhiteSpace(SessionState)) {
                    formattedSessionState = $" ({SessionState})";
                }

                if (!string.IsNullOrWhiteSpace(DesktopGroupName))
                {
                    formattedDesktopGroupName = $" - {DesktopGroupName}";
                }
            }

            return $"{formattedHostedMachineName}{formattedSessionState}{formattedDesktopGroupName}";
        }
    }
}
