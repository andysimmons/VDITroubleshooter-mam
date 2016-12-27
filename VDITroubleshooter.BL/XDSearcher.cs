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
using System.Collections;
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

                // Inject some custom properties via Select-Object
                var selectObject = new Command("Select-Object");
                var selectProps = new List<object>()
                {
                    "*",

                    new Hashtable()
                    {
                        { "Name", "SiteName" },
                        { "Expression", ScriptBlock.Create($"(Get-BrokerSite -AdminAddress {adminAddress}).Name") }
                    },

                    new Hashtable()
                    {
                        { "Name", "AdminAddress" },
                        { "Expression", ScriptBlock.Create($"'{adminAddress}'") }
                    },

                    new Hashtable()
                    {
                        { "Name", "IsExternal" },
                        { "Expression", ScriptBlock.Create("[bool]$_.SmartAccessTags.Count") }
                    }
                };
                selectObject.Parameters.Add("Property", selectProps);

                Pipeline pipeline = runSpace.CreatePipeline();
                pipeline.Commands.Add(getSession);
                pipeline.Commands.Add(selectObject);
                Collection<PSObject> psResults = pipeline.Invoke();

                foreach (PSObject psResult in psResults)
                {
                    sessions.Add(PSObjectToSession(psResult));
                }

                getSession.Parameters.Clear();
                selectObject.Parameters.Clear();
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

