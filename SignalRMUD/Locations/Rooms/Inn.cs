using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class Inn : IRoom
    {
        private IHubContext<MainHub> _hubContext;
        public IHubContext<MainHub> hubContext { 
            get => _hubContext;
            set => _hubContext = value; 
        }

        private string _roomName = "The Inn";
        public string roomName { get => _roomName; }

        private string _roomDescription = "This is a pleasant inn. Warm and homely.";
        public string roomDescription { get => _roomDescription; }

        private Dictionary<string, object> _friendHandles;
        public Dictionary<string, object> friendHandles { 
            get => _friendHandles;
            set => _friendHandles = value; 
        }

        private Innkeeper bob;

        public Inn(IHubContext<MainHub> hubContext) 
        {
            this.hubContext = hubContext;
            bob = new Innkeeper(hubContext, roomName);

            friendHandles = new Dictionary<string, object>() {
                {"bob", bob},
                {"the innkeeper", bob},
                {"innkeeper", bob},
                {"bob the innkeeper", bob},
            };
        }

        public void Heartbeat()
        {
            bob.Heartbeat();
        }

        public string GreetRequest(string target) 
        {
            string targetLowercase = target.ToLower();

            if (friendHandles.TryGetValue(targetLowercase, out dynamic friendObj)) {
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