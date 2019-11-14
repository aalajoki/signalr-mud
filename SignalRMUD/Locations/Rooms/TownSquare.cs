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

        public TownSquare(IHubContext<MainHub> hubContext) 
        {
            this.hubContext = hubContext;
        }

        public void Heartbeat() {
            // Room logic here
            hubContext.Clients.Group(_roomName).SendAsync("ReceiveMessage", roomDescription);
        }
    }
}