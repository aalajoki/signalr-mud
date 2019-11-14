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

        public Inn(IHubContext<MainHub> hubContext) 
        {
            this.hubContext = hubContext;
        }

        public void Heartbeat() {
            // Room logic here
            hubContext.Clients.Group(_roomName).SendAsync("ReceiveMessage", roomDescription);
        }
    }
}