using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
//using Citrix.Broker.Admin.SDK;

namespace VDITroubleshooter.BL
{
    public static class XDSearcher
    {
        public static List<Session> GetSessions(string[] AdminAddresses, string UserName)
        {
            var sessions = new List<Session>();

            Runspace runSpace = RunspaceFactory.CreateRunspace();
            runSpace.Open();
            PSSnapInException psex;
            runSpace.RunspaceConfiguration.AddPSSnapIn("Citrix.Broker.Admin.V2", out psex);

            foreach (string adminAddress in AdminAddresses)
            {
                var getSession = new Command("Get-BrokerSession");
                getSession.Parameters.Add("AdminAddress", adminAddress);
                getSession.Parameters.Add("UserName", UserName);

                Pipeline pipeline = runSpace.CreatePipeline();
                pipeline.Commands.Add(getSession);
                Collection<PSObject> psResults = pipeline.Invoke();

                foreach (PSObject psResult in psResults)
                {
                    sessions.Add(PSObjectToSession(psResult));
                }

                getSession.Parameters.Clear();
                pipeline.Dispose();
            }
            runSpace.Dispose();

            return sessions;
        }

        /// <summary>
        /// Convert the de-serialized Citrix.Broker.Admin.SDK.Session from a PSObject
        /// to our own session type.
        /// </summary>
        /// <param name="PSObject"></param>
        /// <returns></returns>
        private static Session ToSession(PSObject PSObject)
        {
            var session = new Session();

            string strAgentVersion = PSObject.Members["AgentVersion"]?.Value.ToString();
            Version agentVersion;
            if (Version.TryParse(strAgentVersion, out agentVersion))
            {
                session.AgentVersion = agentVersion;
            }
            
            session.CatalogName = PSObject.Members["CatalogName"]?.Value.ToString();

            string strClientAddress = PSObject.Members["ClientAddress"]?.Value.ToString();
            IPAddress clientAddress;
            if (IPAddress.TryParse(strClientAddress, out clientAddress))
            {
                session.ClientAddress = clientAddress;
            }

            session.ClientName = PSObject.Members["ClientName"]?.Value.ToString();

            string strClientVersion = PSObject.Members["ClientVersion"]?.Value.ToString();
            Version clientVersion;
            if (Version.TryParse(strClientVersion, out clientVersion))
            {
                session.ClientVersion = clientVersion;
            }

            session.ConnectedViaHostName = PSObject.Members["ConnectedViaHostName"]?.Value.ToString();

            string strConnectedViaIP = PSObject.Members["ConnectedViaIP"]?.Value.ToString();
            IPAddress connectedViaIP;
            if (IPAddress.TryParse(strConnectedViaIP, out connectedViaIP))
            {
                session.ConnectedViaIP = connectedViaIP;
            }

            session.ControllerDNSName = PSObject.Members["ControllerDNSName"]?.Value.ToString();

            session.DNSName = PSObject.Members["DNSName"]?.Value.ToString();

            session.DesktopGroupName = PSObject.Members["DesktopGroupName"]?.Value.ToString();

            string strDesktopGroupUid = PSObject.Members["DesktopGroupUid"]?.Value.ToString();
            int desktopGroupUid;
            if (int.TryParse(strDesktopGroupUid, out desktopGroupUid))
            {
                session.DesktopGroupUid = desktopGroupUid;
            }

            string DesktopKind = PSObject.Members["DesktopKind"]?.Value.ToString();

            string strDesktopSID = PSObject.Members["DesktopSID"]?.Value.ToString();
            Guid desktopSID;
            if (Guid.TryParse(strDesktopSID, out desktopSID))
            {
                session.DesktopSID = desktopSID;
            }

            session.DeviceId = PSObject.Members["DeviceId"]?.Value.ToString();

            string strEstablishmentDuration = PSObject.Members["EstablishmentDuration"]?.Value.ToString();
            int establishmentDuration;
            if (int.TryParse(strEstablishmentDuration, out establishmentDuration))
            {
                session.EstablishmentDuration = establishmentDuration;
            }

            string strEstablishmentTime = PSObject.Members["EstablishmentTime"]?.Value.ToString();
            DateTime establishmentTime;
            if (DateTime.TryParse(strEstablishmentTime, out establishmentTime))
            {
                session.EstablishmentTime = establishmentTime;
            }

            session.HardwareId = PSObject.Members["HardwareId"]?.Value.ToString();

            string strHidden = PSObject.Members["Hidden"]?.Value.ToString();
            bool hidden;
            if (bool.TryParse(strHidden, out hidden))
            {
                session.Hidden = hidden;
            }

            session.HostedMachineName = PSObject.Members["HostedMachineName"]?.Value.ToString();

            session.HostingServerName = PSObject.Members["HostingServerName"]?.Value.ToString();

            session.HypervisorConnectionName = PSObject.Members["HypervisorConnectionName"]?.Value.ToString();

            string strIPAddress = PSObject.Members["IPAddress"]?.Value.ToString();
            IPAddress ipAddress;
            if (IPAddress.TryParse(strIPAddress, out ipAddress))
            {
                session.IPAddress = ipAddress;
            }

            string strImageOutOfDate = PSObject.Members["ImageOutOfDate"]?.Value.ToString();
            bool imageOutOfDate;
            if (bool.TryParse(strImageOutOfDate, out imageOutOfDate))
            {
                session.ImageOutOfDate = imageOutOfDate;
            }

            string strInMaintenanceMode = PSObject.Members["InMaintenanceMode"]?.Value.ToString();
            bool inMaintenanceMode;
            if (bool.TryParse(strInMaintenanceMode, out inMaintenanceMode))
            {
                session.InMaintenanceMode = inMaintenanceMode;
            }

            string strIsPhysical = PSObject.Members["IsPhysical"]?.Value.ToString();
            bool isPhysical;
            if (bool.TryParse(strIsPhysical, out isPhysical))
            {
                session.IsPhysical = isPhysical;
            }

            session.LaunchedViaHostname = PSObject.Members["LaunchedViaHostname"]?.Value.ToString();

            string strLaunchedViaIP = PSObject.Members["LaunchedViaIP"]?.Value.ToString();
            IPAddress launchedViaIP;
            if (IPAddress.TryParse(strLaunchedViaIP, out launchedViaIP))
            {
                session.LaunchedViaIP = launchedViaIP;
            }

            session.MachineName = PSObject.Members["MachineName"]?.Value.ToString();

            session.MachineSummaryState = PSObject.Members["MachineSummaryState"]?.Value.ToString();

            string strMachineUid = PSObject.Members["MachineUid"]?.Value.ToString();
            int machineUid;
            if (int.TryParse(strMachineUid, out machineUid))
            {
                session.MachineUid = machineUid;
            }

            session.OSType = PSObject.Members["OSType"]?.Value.ToString();

            session.PersistUserChanges = PSObject.Members["PersistUserChanges"]?.Value.ToString();

            session.PowerState = PSObject.Members["PowerState"]?.Value.ToString();

            session.Protocol = PSObject.Members["Protocol"]?.Value.ToString();

            session.ProvisioningType = PSObject.Members["ProvisioningType"]?.Value.ToString();

            string strSecureIcaActive = PSObject.Members["SecureIcaActive"]?.Value.ToString();
            bool secureIcaActive;
            if (bool.TryParse(strSecureIcaActive, out secureIcaActive))
            {
                session.SecureIcaActive = secureIcaActive;
            }

            string strSessionId = PSObject.Members["SessionId"]?.Value.ToString();
            int sessionId;
            if (int.TryParse(strSessionId, out sessionId))
            {
                session.SessionId = sessionId;
            }

            string strSessionKey = PSObject.Members["SessionKey"]?.Value.ToString();
            Guid sessionKey;
            if (Guid.TryParse(strSessionKey, out sessionKey))
            {
                session.SessionKey = sessionKey;
            }

            session.SessionState = PSObject.Members["SessionState"]?.Value.ToString();

            string strSessionStateChangeTime = PSObject.Members["SessionStateChangeTime"]?.Value.ToString();
            DateTime sessionStateChangeTime;
            if (DateTime.TryParse(strSessionStateChangeTime, out sessionStateChangeTime))
            {
                session.SessionStateChangeTime = sessionStateChangeTime;
            }

            session.SessionSupport = PSObject.Members["SessionSupport"]?.Value.ToString();

            session.SessionType = PSObject.Members["SessionType"]?.Value.ToString();

            string strStartTime = PSObject.Members["StartTime"]?.Value.ToString();
            DateTime startTime;
            if (DateTime.TryParse(strStartTime, out startTime))
            {
                session.StartTime = startTime;
            }

            string strUid = PSObject.Members["Uid"]?.Value.ToString();
            int uid;
            if (int.TryParse(strUid, out uid))
            {
                session.Uid = uid;
            }

            session.UserFullName = PSObject.Members["UserFullName"]?.Value.ToString();

            session.UserName = PSObject.Members["UserName"]?.Value.ToString();

            string strUserSID = PSObject.Members["UserSID"]?.Value.ToString();
            Guid userSID;
            if (Guid.TryParse(strUserSID, out userSID))
            {
                session.UserSID = userSID;
            }

            session.UserUPN = PSObject.Members["UserUPN"]?.Value.ToString();

            return session;
        }

        // See if you can make this dynamic later... just trying to figure out the basics first.
        private static Session PSObjectToSession(PSObject PSObject)
        {
            var session = new Session();

            Type sessionType = typeof(Session);

            var properties = new Dictionary<string, object>();
            foreach (PropertyInfo prop in sessionType.GetProperties())
            {
                string dsValue;
                try
                {
                    dsValue = PSObject.Members[prop.Name]?.Value.ToString();
                }
                catch
                {
                    dsValue = "";
                }

                // If we have a match, attempt to re-cast the value appropriately, and assign it to
                // the corresponding property on the session instance we'll be returning.
                if (!string.IsNullOrWhiteSpace(dsValue))
                {
                    string propType = prop.PropertyType.ToString();
                    switch (propType)
                    {
                        case "System.String":
                            prop.SetValue(session, dsValue, null);
                            break;

                        case "System.Version":
                            Version verValue;
                            if (Version.TryParse(dsValue, out verValue))
                            {
                                prop.SetValue(session, verValue, null);
                            }
                            break;

                        case "System.Net.IPAddress":
                            IPAddress ipValue;
                            if (IPAddress.TryParse(dsValue, out ipValue))
                            {
                                prop.SetValue(session, ipValue, null);
                            }
                            break;

                        case "System.Int32":
                            int intValue;
                            if (int.TryParse(dsValue, out intValue))
                            {
                                prop.SetValue(session, intValue, null);
                            }
                            break;

                        case "System.Guid":
                            Guid guidValue;
                            if (Guid.TryParse(dsValue, out guidValue))
                            {
                                prop.SetValue(session, guidValue, null);
                            }
                            break;

                        case "System.DateTime":
                            DateTime dtValue;
                            if (DateTime.TryParse(dsValue, out dtValue))
                            {
                                prop.SetValue(session, dtValue, null);
                            }
                            break;

                        case "System.Boolean":
                            bool boolValue;
                            if (bool.TryParse(dsValue, out boolValue))
                            {
                                prop.SetValue(session, boolValue, null);
                            }
                            break;

                        default:
                            throw new ArgumentException($"No converter for type '{propType}'.");
                    }
                }
            }

            return session;
        }
    }
}

