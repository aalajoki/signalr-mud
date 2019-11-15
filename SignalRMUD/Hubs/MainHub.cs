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
                    await Clients.All.SendAsync("ReceiveMessage", $"{Context.Items["characterName"]}: {argument}");
                }
            }
            else if (command == "go") {
                string currentRoomName = Context.Items["currentRoomName"].ToString();
                string destination = _navigation.NavigationRequest(currentRoomName, argument);
                // await Clients.Caller.SendAsync("ReceiveMessage", $"{navigationResult}");
                if (destination != "invalid") {
                    // await Clients.Caller.SendAsync("ReceiveMessage", $"Going {argument}.");
                    await MoveToRoom(currentRoomName, destination);
                }
                else {
                    await Clients.Caller.SendAsync("ReceiveMessage", "You can't go that way.");
                }
            }
            else if (command == "greet") {
                string currentRoomName = Context.Items["currentRoomName"].ToString();
                string result = _roomManager.RelayGreetRequest(currentRoomName, argument);
                // The friendly NPC object sends the message directly to the player on success in the current implementation
                if (result == "notFound") {
                    await Clients.Caller.SendAsync("ReceiveMessage", $"Couldn't find anyone named {argument}.");
                }
                else {
                    //success
                    await Clients.Caller.SendAsync("ReceiveMessage", result);
                }
            }
            else if (command == "attack") {
                string currentRoomName = Context.Items["currentRoomName"].ToString();
                string characterName = Context.Items["characterName"].ToString();
                string result = _roomManager.RelayAttackRequest(currentRoomName, characterName, argument, 1);
                // The friendly NPC object sends the message directly to the player on success in the current implementation
                if (result == "notFound") {
                    await Clients.Caller.SendAsync("ReceiveMessage", $"Couldn't find enemies named {argument}.");
                }
            }
            else if (command == "stop") {
                string currentRoomName = Context.Items["currentRoomName"].ToString();
                string characterName = Context.Items["characterName"].ToString();
                
                _roomManager.RelayStopAttack(currentRoomName, characterName);
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

        public async Task MoveToRoom(string currentRoomName, string newRoomName)
        {
            string characterName = Context.Items["characterName"].ToString();

            _roomManager.RelayStopAttack(currentRoomName, characterName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentRoomName);
            await Clients.Group(currentRoomName).SendAsync("ReceiveMessage", $"{Context.Items["characterName"]} has left the area.");

            await EnterRoom(newRoomName);
        }

        public async Task EnterRoom(string currentRoomName)
        {
            this.Context.Items["currentRoomName"] = currentRoomName;

            await Groups.AddToGroupAsync(Context.ConnectionId, currentRoomName);
            await Clients.Group(currentRoomName).SendAsync("ReceiveMessage", $"{Context.Items["characterName"]} has entered the area.");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string currentRoomName = Context.Items["currentRoomName"].ToString();
            string characterName = Context.Items["characterName"].ToString();

            _roomManager.RelayStopAttack(currentRoomName, characterName);

            await Clients.Group(currentRoomName).SendAsync("ReceiveMessage", $"{Context.Items["characterName"]} has disconnected.");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentRoomName);
        }
    }
}