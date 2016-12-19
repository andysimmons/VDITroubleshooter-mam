using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace VDITroubleshooter.BL
{
    class Session
    {
        public Version AgentVersion { get; set; }

        public string CatalogName { get; set; }

        public IPAddress ClientAddress { get; set; }

        public string ClientName { get; set; }

        public Version ClientVersion { get; set; }

        public string ConnectedViaHostName { get; set; }

        public IPAddress ConnectedViaIP { get; set; }

        public string ControllerDNSName { get; set; }

        public string DNSName { get; set; }

        public string DesktopGroupName { get; set; }

        public int DestkopGroupUid { get; set; }

        public string DesktopKind { get; set; }

        public Guid DesktopSID { get; set; }

        public string DeviceId { get; set; }

        public int EstablishmentDuration { get; set; }

        public DateTime EstablishmentTime { get; set; }

        public string HardwareId { get; set; }

        public bool Hidden { get; set; }

        public string HostedMachineName { get; set; }

        public string HostingServerName { get; set; }

        public string HypervisorConnectionName { get; set; }

        public IPAddress IPAddress { get; set; }

        public bool ImageOutOfDate { get; set; }

        public bool InMaintenanceMode { get; set; }

        public bool IsPhysical { get; set; }

        public string LaunchedViaHostname { get; set; }

        public IPAddress LaunchedViaIP { get; set; }

        public string MachineName { get; set; }

        public string MachineSummaryState { get; set; }

        public int MachineUid { get; set; }

        public string OSType { get; set; }

        public string PersistUserChanges { get; set; }

        public string PowerState { get; set; }

        public string Protocol { get; set; }

        public string ProvisioningType { get; set; }

        public bool SecureIcaActive { get; set; }

        public int SessionId { get; set; }

        public Guid SessionKey { get; set; }

        public string SessionState { get; set; }

        public DateTime SessionStateChangeTime { get; set; }

        public string SessionSupport { get; set; }

        public string SessionType { get; set; }

        public DateTime StartTime { get; set; }

        public int Uid { get; set; }

        public string UserFullName { get; set; }

        public string UserName { get; set; }

        public Guid UserSID { get; set; }

        public string UserUPN { get; set; }


    }
}
