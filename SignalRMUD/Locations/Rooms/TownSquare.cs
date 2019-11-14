using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class TownSquare : IRoom
    {
        private IHubContext<MainHub> _hubContext;
        public IHubContext<MainHub> hubContext { 
            get => _hubContext;
            set => _hubContext = value; 
        }

        private string _roomName = "Town Square";
        public string roomName { get => _roomName; }

        private string _roomDescription = "It is quiet. Most people are staying inside.";
        public string roomDescription { get => _roomDescription; }

        private Dictionary<string, object> _friendHandles;
        public Dictionary<string, object> friendHandles { 
            get => _friendHandles;
            set => _friendHandles = value; 
        }

        public TownSquare(IHubContext<MainHub> hubContext) 
        {
            this.hubContext = hubContext;
        }

        public void Heartbeat() {
            // Room logic here
            hubContext.Clients.Group(_roomName).SendAsync("ReceiveMessage", roomDescription);
        }

        public string GreetRequest(string target) 
        {
            if (friendHandles.TryGetValue(target, out dynamic friendObj)) {
                return friendObj.Greet();
            }
            // else if (enemyHandles.TryGetValue(target, out dynamic enemyObj)) {
            //     return "notFriend";
            // }
            else {
                return "notFound";
            }
        }
    }
}