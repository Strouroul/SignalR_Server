using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SignalRSelfHost
{

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
        
    }


    class Program
    {
        static void Main(string[] args)
        {

            ////////// This will *ONLY* bind to localhost, if you want to bind to all addresses
            ////////// use http://*:8080 to bind to all addresses. 
            ////////// See http://msdn.microsoft.com/library/system.net.httplistener.aspx 
            ////////// for more information.
            ////////string url = "http://localhost:8080";
            ////////using (WebApp.Start(url))
            ////////{
            ////////    Console.WriteLine("Server running on {0}", url);
            ////////    Console.ReadLine();
            ////////}
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };

            string url = "http://*:8088";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                while (true)
                {
                    string key = Console.ReadLine();
                    if (key.ToUpper() == "W")
                    {
                        

                        IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                        hubContext.Clients.All.addMessage("server", "ServerMessage");
                        Console.WriteLine("Server Sending addMessage\n");
                    }
                    if (key.ToUpper() == "E")
                    {
                        IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                        hubContext.Clients.All.heartbeat();
                        Console.WriteLine("Server Sending heartbeat\n");
                    }
                    if (key.ToUpper() == "R")
                    {
                        IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();

                        var vv = new HelloModel { Age = 37, Molly = "pushed direct from Server " };

                        hubContext.Clients.All.sendHelloObject(vv);
                        Console.WriteLine("Server Sending sendHelloObject\n");
                    }
                    if (key.ToUpper() == "C")
                    {
                        break;
                    }
                }

                Console.ReadLine();
            }
        }
    }
    


    public class HelloModel
    {
        public string Molly { get; set; }

        public int Age { get; set; }
    }

    public class Group_Info
    {
        public string group_Name { get; set; }
        public string group_ID { get; set; }
        public string group_Owner { get; set; }

    }
    
    public class Group_Handler
    {
        public Group_Info groups =new Group_Info();
    }
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            // your logic to fetch a user identifier goes here.

            // for example:
            
            var userId = request.User.Identity.Name;
            return userId.ToString();
        }
        
    }

    



    public class MyHub : Hub
    {
        public string hub_Domain = "MY_HUB";
       

        public void SendChatMessage(string who, string message)
        {
            string name = Context.User.Identity.Name;
            Clients.Group(name).addChatMessage(name, message);
            //Clients.Group("2@2.com").addChatMessage(name, message);
        }

        
        public void AddMessage(string name, string message)
        {
            Console.WriteLine("Hub :" + hub_Domain + "<BR>" + "<BR>AddMessage {0} {1}\n", name, message);
            Clients.All.addMessage(name, message);
        }

        public void Heartbeat()
        {
            Console.WriteLine("Hub Heartbeat\n");
            Clients.All.heartbeat();
        }

        public void SendHelloObject(HelloModel hello)
        {
            Console.WriteLine("Hub hello {0} {1}\n", hello.Molly, hello.Age);
            Clients.All.sendHelloObject(hello);
        }
        

        public override Task OnConnected()
        {
            
            var version = Context.QueryString["version"];
            Console.WriteLine("Version : " + version);
            try
            {
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                
                //string name = Context.User.Identity.Name.ToString();
                string name = Context.ConnectionId;
                Groups.Add(Context.ConnectionId, name);
                Console.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
                Console.WriteLine("Group : " + name);
                Console.WriteLine("Query String" + version);
                
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
            //UserHandler.ConnectedIds.Add(Context.ConnectionId);
           // Console.WriteLine("User ID : " + Clients.Caller);
           
           

            return base.OnConnected();
        }
       

        //////public override Task OnDisconnected()
        //////{
        //////    Console.WriteLine("Hub OnDisconnected {0}\n", Context.ConnectionId);
        //////    return (base.OnDisconnected());
        //////}

        //////public override Task OnReconnected()
        //////{
        //////    Console.WriteLine("Hub OnReconnected {0}\n", Context.ConnectionId);
        //////    return (base.OnDisconnected());
        //////}
    }
}