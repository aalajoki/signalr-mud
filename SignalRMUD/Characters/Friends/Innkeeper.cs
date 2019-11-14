using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    class Innkeeper : IFriend
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
        public string name {
            get => _name;
        } 

        private string _description;
        public string description {
            get => _description;
        }

        private List<string> _monologue = new List<string>{
            "Nice weather, isn't it?",
            "Be careful. There's monsters lurking around every corner...not in my inn, though.",
            "Good day.",
            "Lots of people passing through now, with all the problems going on."
        };
        public List<string> monologue {
            get => _monologue;
        }

        private List<string> _idle = new List<string>{
            "The innkeeper coughs.",
            "The innkeeper sweeps the floor with a broom.",
            "The innkeeper washes a mug.",
            "The innkeeper wipes the counter with a rag."
        };
        public List<string> idle {
            get => _idle;
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
                int rand = _rand.Next(0, _idle.Count);
                hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", idle[rand]);
            }
        }
    }
}