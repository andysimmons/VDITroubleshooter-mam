using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Citrix.Broker.Admin.SDK;

namespace VDITroubleshooter.BL
{
    public static class XDSearcher
    {
        public static Collection<PSObject> GetSessions(string AdminAddress)
        {
            Runspace runSpace = RunspaceFactory.CreateRunspace();

            PSSnapInException psex;
            runSpace.RunspaceConfiguration.AddPSSnapIn("Citrix.Broker.Admin.V2", out psex);
            Pipeline pipeline = runSpace.CreatePipeline();

            var getSession = new Command("Get-BrokerSession");
            getSession.Parameters.Add("AdminAddress", "ctxddc01");
            pipeline.Commands.Add(getSession);

            //foreach (PSObject session in sessions)
            //{
            //    string hmn = session.Members["HostedMachineName"]?.Value.ToString();
            //    string ss = session.Members["SessionState"]?.Value.ToString();

            //    Console.WriteLine($"{hmn} ({ss})");
            //}

            return pipeline.Invoke();
        }
    }
}

