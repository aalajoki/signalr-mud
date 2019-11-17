using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    class Thief : INonPlayerCharacter
    {
        private IHubContext<MainHub> _hubContext;
        public IHubContext<MainHub> hubContext { 
            get => _hubContext;
            set => _hubContext = value; 
        }

        private string _roomName;
        public string roomName {
            get => _roomName;
            set => _roomName = value;
        }

        private string _name = "Thief";
        public string name { get => _name; } 

        private string _description;
        public string description { get => _description; }

        private List<string> _monologue = new List<string>{
            "You saw nothing.",
        };
        public List<string> monologue { get => _monologue; }

        private List<string> _idle = new List<string>{
            " lurks in the shadows.",
        };
        public List<string> idle { get => _idle; }

        private int _health = 99;
        public int health {
            get => _health;
            set => _health = value;
        }

        private Random _rand = new Random();

        public Thief(IHubContext<MainHub> hubContext, string room)
        {
            this.hubContext = hubContext;
            this.roomName = room;
        }

        public void Heartbeat()
        {
            // 1:5 chance for a random idle message per heartbeat
            if (_rand.Next(0, 100) < 10) {
                int rand = _rand.Next(0, idle.Count);
                hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", $"{name} {idle[rand]}");
            }
        }

        public string Greet() {
            int rand = _rand.Next(0, monologue.Count);
            string response = $"{name}: {monologue[rand]}";
            return response;
        }

        public int TakeDamage(int damage) {
            health -= damage;
            if (health >= 0) {
                return 0;
            }
            else {
                return health;
            }
        }

        // respond with {name}: {idletext}
    }
}