using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    interface IEnemy
    {
        IHubContext<MainHub> hubContext {get; set;}
        string roomName {get; set;}
        string name {get;}
        string description {get;}
        List<string> monologue {get;}
        List<string> idle {get;}
        int health {get; set;}

        void Heartbeat();
        string Greet();
    }
}