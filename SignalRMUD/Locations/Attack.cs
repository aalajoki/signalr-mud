using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public struct Attack
    {
        public string attacker;
        public string target;
        public int attackPower;
        
        public Attack(string attacker, string target, int attackPower) {
            this.attacker = attacker;
            this.target = target;
            this.attackPower = attackPower;
        }
    }
}