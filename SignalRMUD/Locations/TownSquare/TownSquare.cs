using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class TownSquare : IRoom
    {
        private IHubContext<MainHub> _hubContext;
        public IHubContext<MainHub> hubContext { 
            get => _hubContext;
            set => _hubContext = value; 
        }

        private string _roomName = "Town Square";
        public string roomName { get => _roomName; }

        private string _roomDescription = "It is quiet. Most people are staying inside.";
        public string roomDescription { get => _roomDescription; }

        private Dictionary<string, object> _friendHandles;
        public Dictionary<string, object> friendHandles { 
            get => _friendHandles;
            set => _friendHandles = value; 
        }

        private Dictionary<string, object> _enemyHandles;
        public Dictionary<string, object> enemyHandles { 
            get => _enemyHandles;
            set => _enemyHandles = value; 
        }

        private Dictionary<string, string> _navigationDirections;
        public Dictionary<string, string> navigationDirections { 
            get => _navigationDirections;
            set => _navigationDirections = value; 
        }

        private SortedDictionary<string, Attack> _attackQueue = new SortedDictionary<string, Attack>();

        private Thief thief;

        public TownSquare(IHubContext<MainHub> hubContext) 
        {
            this.hubContext = hubContext;
            thief = new Thief(hubContext, roomName);

            friendHandles = new Dictionary<string, object>() {};

            enemyHandles = new Dictionary<string, object>() {
                {"thief", thief},
            };

            navigationDirections = new Dictionary<string, string>() {
                {"in", "The Inn"},
                {"inside", "The Inn"},
                {"inn", "The Inn"},
            };
        }

        public void Heartbeat() {
            // Room logic here
            thief.Heartbeat();
            HandleAttacks();
        }

        public string GreetRequest(string target) 
        {
            string targetLowercase = target.ToLower();

            if (friendHandles.TryGetValue(targetLowercase, out dynamic friendObj)) {
                return friendObj.Greet();
            }
            else if (enemyHandles.TryGetValue(targetLowercase, out dynamic enemyObj)) {
                return enemyObj.Greet();
            }
            else {
                return "notFound";
            }
        }

        public void HandleAttacks() 
        {
            foreach( KeyValuePair<string, Attack> atk in _attackQueue )
            {
                if (enemyHandles.TryGetValue(atk.Value.target, out dynamic enemyObj)) {
                    // Check if enemy is active / alive
                    enemyObj.TakeDamage(atk.Value.attackStat);
                    if (enemyObj.health <= 0) {
                        hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", $"{atk.Value.target} was killed by {atk.Key}!");
                        _attackQueue.Remove(atk.Key);
                    }
                    else {
                        hubContext.Clients.Group(roomName).SendAsync(
                            "ReceiveMessage", 
                            $"{atk.Key} attacked {atk.Value.target} and dealt {atk.Value.attackStat} damage! {enemyObj.health} HP remaining."
                        );
                    }
                }
                else {
                    _attackQueue.Remove(atk.Key);
                }
            }
        }

        public string AttackRequest(string attacker, string target, int attackStat) 
        {
            string targetLowercase = target.ToLower();
            
            if (enemyHandles.TryGetValue(targetLowercase, out dynamic enemyObj)) {
                Attack atk = new Attack(attacker, target, attackStat);
                _attackQueue.Add(attacker, atk);

                return "success";
            }
            else {
                return "notFound";
            }
        }

        public void StopAttack(string attacker) {
            _attackQueue.Remove(attacker);
        }

        public string NavigationRequest(string direction) {
            if (navigationDirections.TryGetValue(direction, out string nextRoom)) {
                return nextRoom;
            }
            else {
                return "invalid";
            }
        }
    }
}