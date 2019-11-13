using System;
using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    public class Navigation
    {
        private Dictionary<string, Dictionary<string, string>> _navigation;

        public Navigation() {
            _navigation = new Dictionary<string, Dictionary<string, string>>() {
                { "The Inn", new Dictionary<string,string>(){
                    {"out", "Town Square"},
                    {"outside", "Town Square"}
                }},
                { "Town Square", new Dictionary<string,string>(){
                    {"in", "The Inn"},
                    {"inside", "The Inn"}
                }},
            };
        }

        public string NavigationRequest(string currentLocation, string direction) {

            string destination;

            // See if the given direction is valid in the current location
            if (_navigation[currentLocation].TryGetValue(direction, out destination))
            {
                return destination;
            }
            else {
                return "invalid";
            }
        }
    }
}