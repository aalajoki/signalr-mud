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
                {"The Inn", _inn},
                {"Town Square", _townSquare}
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
            dynamic room = _roomHandles[currentRoom];
            string response = room.GreetRequest(target);
            return response;
        }

        public string RelayAttackRequest(string currentRoom, string attacker, string target, int attackPower) 
        {
            dynamic room = _roomHandles[currentRoom];
            string response = room.AttackRequest(attacker, target, attackPower);
            return response;
        }

        public void RelayStopAttack(string currentRoom, string attacker)
        {
            dynamic room = _roomHandles[currentRoom];
            room.StopAttack(attacker);
        }

        public string RelayNavigationRequest(string currentRoom, string direction)
        {
            dynamic room = _roomHandles[currentRoom];
            string destination = room.NavigationRequest(direction);
            return destination;
        }
    }
}