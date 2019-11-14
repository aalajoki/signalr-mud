using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    interface IRoom
    {
        IHubContext<MainHub> hubContext {get; set;}
        string roomName {get;}
        string roomDescription {get;}
        Dictionary<string, object> friendHandles {get; set;}
        Dictionary<string, object> enemyHandles {get; set;}

        // Room-specific code that is run once per heartbeat
        void Heartbeat();
        string GreetRequest(string target);
        //string AttackRequest(string target);
    }
}