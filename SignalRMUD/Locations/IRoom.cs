using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    interface IRoom
    {
        IHubContext<MainHub> hubContext {get; set;}
        string roomName {get;}
        string roomDescription {get;}

        void Heartbeat();
    }
}