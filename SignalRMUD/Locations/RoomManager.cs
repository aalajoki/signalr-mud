using Microsoft.AspNetCore.SignalR;
using System;
using System.Timers;
using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    public class RoomManager
    {
        private readonly IHubContext<MainHub> _hubContext;
        private static System.Timers.Timer _timer;

        // Rooms
        private Inn _inn;
        private TownSquare _townSquare;

        private Dictionary<string, object> _roomHandles;

        public RoomManager(IHubContext<MainHub> hubContext) 
        {
            _hubContext = hubContext;
            _inn = new Inn(_hubContext);
            _townSquare = new TownSquare(_hubContext);

            _roomHandles = new Dictionary<string, object>() {
                {"The Inn", _inn}
            };

            SetPulse(2000);
        }
        
        private void SetPulse(int interval) 
        {
            _timer = new System.Timers.Timer(interval);
            _timer.Elapsed += Heartbeat;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        
        private void Heartbeat(Object source, ElapsedEventArgs e) 
        {
            _inn.Heartbeat();
            _townSquare.Heartbeat();
        }

        public string RelayGreetRequest(string currentRoom, string target) 
        {
            // return _roomHandles[currentRoom].ToString();

            dynamic room = _roomHandles[currentRoom];
            string response = room.GreetRequest(target);
            return response;
        }
    }
}