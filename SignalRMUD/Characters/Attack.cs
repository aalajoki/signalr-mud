using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class Attack
    {
        private string _attacker;
        public string attacker {
            get => _attacker;
            set => _attacker = value;
        }

        private string _target;
        public string target {
            get => _target;
            set => _target = value;
        }

        private int _attackStat;
        public int attackStat {
            get => _attackStat;
            set => _attackStat = value;
        }

        public Attack(string attacker, string target, int attackStat) {
            this.attacker = attacker;
            this.target = target;
            this.attackStat = attackStat;
        }
    }
}