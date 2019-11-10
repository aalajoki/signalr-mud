using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class MainHub : Hub
    {
        // public async Task SendMessage(string user, string message)
        // {
        //     if (message == "1123124" || message == "aaaaa") {
        //         this.Context.Items.Add("test", message);
        //     }
        //     await Clients.All.SendAsync("ReceiveMessage", user, message);
        //     await Clients.All.SendAsync("ReceiveMessage", user, this.Context.Items["test"]);
        // }

        public async Task SendMessageGlobal(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", this.Context.Items["characterName"] + ": " + message);
        }

        public async Task CreateCharacter(string name, string race)
        {
            this.Context.Items.Add("characterName", name);
            this.Context.Items.Add("characterRace", race);
            // Send response
        }

        // public async Task Set(string user, string message)
        // {
        //     messagesList.Items.Add(user + ": " + message);
        // }
        // public async Task Get()
        // {

        //     await Clients.All.SendAsync("ReceiveMessage", user, content);
        // }
    }
}