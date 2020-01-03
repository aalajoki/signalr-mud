using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    public class MainHub : Hub
    {
        private readonly RoomManager _roomManager;

        public MainHub(RoomManager roomManager) {
            _roomManager = roomManager;
        }

        public async Task SendMessageGlobal(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        // When a player sends a message, it is split into a command (first word) and the command's argument (rest of the message)
        public async Task SendMessage(string message)
        {
            string[] splitMessage = message.Split(' ');
            List<string> messageList = new List<string>(splitMessage);
            string command = messageList[0];

            messageList.RemoveAt(0);
            string argument = String.Join(" ", messageList);

            await InterpretCommand(command, argument);
        }

        // Command and its argument are recognized and processed to affect the game environment (if valid)
        public async Task InterpretCommand(string command, string argument) 
        {
            string currentRoomName = Context.Items["currentRoomName"].ToString();
            string characterName = Context.Items["characterName"].ToString();

            switch (command)
            {
                case "say":
                    if (String.IsNullOrEmpty(argument)) {
                        await Clients.Caller.SendAsync("ReceiveMessage", "Cat got your tongue?");
                    }
                    else {
                        // E.g. add "Bob: Hi" into chat
                        await Clients.All.SendAsync("ReceiveMessage", $"{Context.Items["characterName"]}: {argument}");
                    }
                    break;
                case "go":
                    string destination = _roomManager.RelayNavigationRequest(currentRoomName, argument);

                    if (destination == "invalid") {
                        await Clients.Caller.SendAsync("ReceiveMessage", "You can't go that way.");
                    }
                    else {
                        // Create an illusion of movement by subscribing to another room's heartbeat events
                        await MoveToRoom(currentRoomName, destination);
                    }
                    break;
                case "greet":
                case "talk":
                    // Relay the greet command to the correct room and NPC through RoomManager
                    string greetRequestResponse = _roomManager.RelayGreetRequest(currentRoomName, argument);
                    
                    if (greetRequestResponse == "notFound") {
                        await Clients.Caller.SendAsync("ReceiveMessage", $"Couldn't find anyone named {argument}.");
                    }
                    else {
                        // Send the NPC response to the player who sent the command
                        await Clients.Caller.SendAsync("ReceiveMessage", greetRequestResponse);
                    }
                    break;
                case "attack":
                    // Relay the attack command to the correct room and NPC through RoomManager
                    string attackRequestResponse = _roomManager.RelayAttackRequest(currentRoomName, characterName, argument, 1);

                    if (attackRequestResponse == "notFound") {
                        await Clients.Caller.SendAsync("ReceiveMessage", $"Couldn't find enemies named {argument}.");
                    }
                    // If successful, player's attack is added to the room-specific AttackQueue
                    break;
                case "stop":
                    // Player's attack is removed from the AttackQueue
                    _roomManager.RelayStopAttack(currentRoomName, characterName);
                    break;
                default:
                    await Clients.Caller.SendAsync("ReceiveMessage", "Invalid command. Try again.");
                    break;
            }
        }

        public async Task CreateCharacter(string name, string race)
        {
            this.Context.Items.Add("characterName", name);
            this.Context.Items.Add("characterRace", race);
            await Clients.All.SendAsync("ReceiveMessage", $"A new hero has appeared: {name} the {race}!");
            await EnterRoom("The Inn");
        }

        public async Task MoveToRoom(string currentRoomName, string newRoomName)
        {
            string characterName = Context.Items["characterName"].ToString();
            // Stop attacking
            _roomManager.RelayStopAttack(currentRoomName, characterName);
            // Remove player from the old room's group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentRoomName);
            // Tell all players inside the old room that this player has left
            await Clients.Group(currentRoomName).SendAsync(
                "ReceiveMessage", 
                $"{Context.Items["characterName"]} left and entered {newRoomName}."
            );
            await EnterRoom(newRoomName);
        }

        public async Task EnterRoom(string currentRoomName)
        {
            this.Context.Items["currentRoomName"] = currentRoomName;
            // Add player to the new room's group
            await Groups.AddToGroupAsync(Context.ConnectionId, currentRoomName);
            // Tell all players inside the new room that this player has entered
            await Clients.Group(currentRoomName).SendAsync(
                "ReceiveMessage", 
                $"{Context.Items["characterName"]} has entered {currentRoomName}."
            );
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