using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Services.Client;
using System.Net;

namespace ODataSandbox
{
    static class Program
    {
        static void DisplayConnection(XDService.Connection connection)
        {
            Console.WriteLine($"Brokering Date: {connection.BrokeringDate} (via {connection.ClientAddress})");
        }

        static void ListAllConnections(XDService.DatabaseContext dbContext)
        {
            foreach (XDService.Connection c in dbContext.Connections)
            {
                DisplayConnection(c);
            }
        }

        static void Main(string[] args)
        {
            var uri = new Uri("http://sltctxddc01.sl1.stlukes-int.org/Citrix/Monitor/Odata/v2/Data/");
            var dbContext = new XDService.DatabaseContext(uri);
            dbContext.Credentials = CredentialCache.DefaultNetworkCredentials;

            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime lastWeek = now.AddDays(-7).ToUniversalTime();
            string userName = "simmonsa";

            IQueryable<XDService.Connection> connections =
                from c in dbContext.Connections
                where 
                (
                    c.CreatedDate >= lastWeek &&
                    c.CreatedDate <= now &&
                    c.Session.User.UserName == userName
                )
                select c;

            foreach (XDService.Connection c in connections)
            {
                DisplayConnection(c);
            }
        }
    }
}
