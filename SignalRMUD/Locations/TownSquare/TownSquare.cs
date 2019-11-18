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

        private List<Attack> _attackQueue = new List<Attack>();

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

        public string NavigationRequest(string direction) {
            if (navigationDirections.TryGetValue(direction, out string nextRoom)) {
                return nextRoom;
            }
            else {
                return "invalid";
            }
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
            foreach( Attack atk in _attackQueue )
            {
                if (enemyHandles.TryGetValue(atk.target, out dynamic enemyObj)) {

                    enemyObj.TakeDamage(atk.attackPower);

                    if (enemyObj.health <= 0) {
                        hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", $"{atk.target} was killed by {atk.attacker}!");
                        _attackQueue.Remove(atk);
                        // Deactivate enemyObj and remove it from enemyHandles until it respawns
                    }
                    else {
                        hubContext.Clients.Group(roomName).SendAsync(
                            "ReceiveMessage", 
                            $"{atk.attacker} attacked {atk.target} and dealt {atk.attackPower} damage! {enemyObj.health} HP remaining."
                        );
                    }

                }
                else {
                    _attackQueue.Remove(atk);
                }
            }
        }

        public string AttackRequest(string attacker, string target, int attackPower) 
        {
            string targetLowercase = target.ToLower();
            
            if (enemyHandles.TryGetValue(targetLowercase, out dynamic enemyObj)) {
                Attack atk = new Attack(attacker, target, attackPower);
                _attackQueue.Add(atk);

                return "success";
            }
            else {
                return "notFound";
            }
        }

        public void StopAttack(string attacker)
        {
            // Find the attack from the attackQueue that has the matching attacker name
            Attack atk = _attackQueue.Find(x => x.attacker == attacker);
            _attackQueue.Remove(atk);
        }
    }
}