using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    class Innkeeper : INonPlayerCharacter
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

        private string _name = "Bob the Innkeeper";
        public string name { get => _name; } 

        private string _description;
        public string description { get => _description; }

        private List<string> _monologue = new List<string>{
            "Nice weather, isn't it?",
            "Be careful. There's monsters lurking around every corner...not in my inn, though.",
            "Good day.",
            "Lots of people passing through now, with all the problems going on."
        };
        public List<string> monologue { get => _monologue; }

        private List<string> _idle = new List<string>{
            " coughs.",
            " sweeps the floor with a broom.",
            " washes a mug.",
            " wipes the counter with a rag."
        };
        public List<string> idle { get => _idle; }

        private int _health = 1;
        public int health {
            get => _health;
            set => _health = value;
        }

        private Random _rand = new Random();
        
        public Innkeeper(IHubContext<MainHub> hubContext, string room)
        {
            this.hubContext = hubContext;
            this.roomName = room;
        }

        public void Heartbeat()
        {
            // 1:5 chance for a random idle message per heartbeat
            if (_rand.Next(0, 100) < 20) {
                int rand = _rand.Next(0, idle.Count);
                hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", $"{name} {idle[rand]}");
            }
        }

        public string Greet() {
            int rand = _rand.Next(0, monologue.Count);
            string response = $"{name}: {monologue[rand]}";
            return response;
        }

        // respond with {name}: {idletext}
    }
}