using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class Inn : IRoom
    {
        private IHubContext<MainHub> _hubContext;
        public IHubContext<MainHub> hubContext { 
            get => _hubContext;
            set => _hubContext = value; 
        }

        private string _roomName = "The Inn";
        public string roomName { get => _roomName; }

        private string _roomDescription = "This is a pleasant inn. Warm and homely.";
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

        private SortedDictionary<string, Attack> _attackQueue = new SortedDictionary<string, Attack>();

        private Innkeeper bob;

        public Inn(IHubContext<MainHub> hubContext) 
        {
            this.hubContext = hubContext;
            bob = new Innkeeper(hubContext, roomName);

            friendHandles = new Dictionary<string, object>() {
                {"bob", bob},
                {"the innkeeper", bob},
                {"innkeeper", bob},
                {"bob the innkeeper", bob},
            };

            enemyHandles = new Dictionary<string, object>() {};
        }

        public void Heartbeat()
        {
            bob.Heartbeat();
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
    }
}