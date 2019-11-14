using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    public class MainHub : Hub
    {
        private readonly RoomManager _roomManager;
        private readonly Navigation _navigation;

        public MainHub(RoomManager roomManager, Navigation navigation) {
            _roomManager = roomManager;
            _navigation = navigation;
        }
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
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessage(string message)
        {
            string[] splitMessage = message.Split(' ');
            List<string> messageList = new List<string>(splitMessage);
            string command = messageList[0];

            messageList.RemoveAt(0);
            string argument = String.Join(" ", messageList);

            await interpretCommand(command, argument);
        }

        public async Task interpretCommand(string command, string argument) {
            if (command == "say") {
                if (String.IsNullOrEmpty(argument)) {
                    // Nothing to say
                    await Clients.Caller.SendAsync("ReceiveMessage", "Cat got your tongue?");
                }
                else {
                    // E.g. add "Bob: Hi" into chat
                    await Clients.All.SendAsync("ReceiveMessage", $"{Context.Items["characterName"]}: {argument}.");
                }
            }
            else if (command == "go") {
                string currentRoom = Context.Items["currentRoom"].ToString();
                string destination = _navigation.NavigationRequest(currentRoom, argument);
                // await Clients.Caller.SendAsync("ReceiveMessage", $"{navigationResult}");
                if (destination != "invalid") {
                    // await Clients.Caller.SendAsync("ReceiveMessage", $"Going {argument}.");
                    await MoveToRoom(currentRoom, destination);
                }
                else {
                    await Clients.Caller.SendAsync("ReceiveMessage", "You can't go that way.");
                }
            }
            else if (command == "greet") {
                string currentRoom = Context.Items["currentRoom"].ToString();
                string result = _roomManager.RelayGreetRequest(currentRoom, argument);
                // The friendly NPC object sends the message directly to the player on success in the current implementation
                if (result == "notFriend") {
                    await Clients.Caller.SendAsync("ReceiveMessage", $"Couldn't find anyone named {argument}.");
                }
                // else if (result == "notFriend") {
                //     await Clients.Caller.SendAsync("ReceiveMessage", $"{argument} just growls at you.");
                // }
                else {
                    //success
                    await Clients.Caller.SendAsync("ReceiveMessage", result);
                }
            }
            else {
                await Clients.Caller.SendAsync("ReceiveMessage", "Invalid command. Try again.");
            }
        }

        public async Task CreateCharacter(string name, string race)
        {
            // Some error handling here?
            this.Context.Items.Add("characterName", name);
            this.Context.Items.Add("characterRace", race);
            // Add to "The Inn" group
            await Clients.All.SendAsync("ReceiveMessage", $"A new hero has appeared: {name} the {race}!");
            await EnterRoom("The Inn");
        }

        public async Task EnterRoom(string roomName)
        {
            this.Context.Items["currentRoom"] = roomName;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            // Remove after debugging
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.Items["characterName"]} has entered {roomName}.");
        }

        public async Task MoveToRoom(string currentRoomName, string newRoomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentRoomName);
            // Remove after debugging
            await Clients.Group(currentRoomName).SendAsync("ReceiveMessage", $"{Context.Items["characterName"]} has exited {currentRoomName}.");

            await EnterRoom(newRoomName);
        }
    }
}