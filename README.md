# signalr-mud
A simple MUD-like online multiplayer game skeleton, built on top of SignalR Core. Completely text-based.

Currently supported player commands:
```
  Say X  -  Send a message X to all players.
  Go X  -  Attempt to move into the connected room X.
  Greet X or talk X  -  Get a line of dialogue from character X in the current room.
  Attack X  -  Add your attack to the room-specific attack queue, targeting enemy X.
  Stop  -  Remove yourself from the room's attack queue
```

With these commands you can navigate between the two existing rooms (Inn and Town Square), talk to one NPC in each location and attack the Thief in Town Square, as well as to communicate with other players.

WIP: Proper authentication + database, combat mechanics, look command, a better architectural pattern for room classes, quest system.
