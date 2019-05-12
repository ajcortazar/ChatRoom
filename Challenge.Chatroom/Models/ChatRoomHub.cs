using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Challenge.Chatroom.Db.Model;
using Microsoft.AspNetCore.SignalR;

namespace Challenge.Chatroom.Models
{
    public class ChatRoomHub : Hub
    {
        static List<string> ConnectionSessions = new List<string>();
        static List<User> Users = new List<User>();
        static List<Post> Posts = new List<Post>();

        public override Task OnConnectedAsync()
        {
            var id = Context.ConnectionId;
            var user = GetUserInformation(Context.User.Claims);
            if(user.Id == 0) { 
}
            user.ConnectionId = id;
            user.LoginTime = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            Users.Add(user);            

            ConnectionSessions.Add(id);
            Clients.All.SendAsync("ConnectedUsers", Users);
            Clients.All.SendAsync("ReceivedMessage", Posts);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connection = ConnectionSessions.FirstOrDefault(p => p == Context.ConnectionId);
            if (!string.IsNullOrEmpty(connection))
            {
                ConnectionSessions.Remove(Context.ConnectionId);
                var user = Users.First(p => p.Id == int.Parse(Context.User.Claims.First(q => q.Type == "IdUser").Value));
                Users.Remove(user);
            }
            return base.OnDisconnectedAsync(exception);
        }
        
        public async Task SendMessage(string message)
        {
            AddMessage(GetStockQuote(message), DateTime.Now.ToString("HH:mm:ss"));
            await Clients.All.SendAsync("ReceivedMessage", Posts);
        }

        private void AddMessage(string message, string datetime)
        {
            Posts.Add(new Post { Message = message, DateTime = datetime });
            Posts = Posts.OrderBy(p => p.DateTime).ToList();
            if (Posts.Count > 50)
                Posts.RemoveAt(0);
        }

        public User GetUserInformation(IEnumerable<Claim> claims)
        {
            var userInfo = new User();

            foreach (var claim in claims)
            {
                var x = claim;
                switch (claim.Type)
                {
                    case "FullName": userInfo.FullName = claim.Value; break;
                    case "Identification": userInfo.Identification = claim.Value; break;
                    case "IdUser": userInfo.Id = int.Parse(claim.Value); break;
                }
            }
            
            return userInfo;
        }

        public string GetStockQuote(string message)
        {
            var name = Users.First(p => p.ConnectionId == Context.ConnectionId).FullName;
            var format = "/stock=";
            string queue = "stock_queue";

            if (message.Contains(format))
            {
                var index = message.IndexOf('=') + 1;
                message = message.Substring(index);

                ServiceClient client = new ServiceClient(queue);
                var connection = client.GetConnection();
                var model = connection.CreateModel();
                message = client.SendMessageToQueue(message, model, TimeSpan.FromMinutes(1));
                return "Bot: " + message;                
            }
            else
            {
                return name + ": " + message;
            }
        }
    }
}
