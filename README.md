# NightbaneManualEvents

Empower your server with **NightbaneManualEvents**, a plugin that puts event management in your hands. Whether it’s a chaotic PvP showdown or a cooperative challenge, this tool gives admins the power to start, pause, and control events with precision—while players join the action seamlessly.

Example of use: https://www.youtube.com/watch?v=mJTzd9U83l0


## Features

- **Manual Event Control**: Start, pause, or end events on your terms.
- **Player Management**: Ban, kick, or gather participants with ease.
- **Dynamic Participation**: Players can join or leave events with simple commands.
- **Inventory Check**: Ensures fair play by requiring empty inventories to join.
- **Real-Time Updates**: Broadcast event status to all players via system messages.

## Planned Features

- **Freeze all players**
- **Give a specific gear set and remove it when event ends**
- **PvP Protection**


## Commands

### Admin Commands (Restricted to Admins)
- `.event add <playerName>`  
  Manually add a specific player to the Event (works while the event is closed to new participants).

- `.event pause`  
  Pause or resume the event. Pausing closes it to new participants.  
  *Example Reply: "The event is now paused."*

- `.event count`  
  Check the number of players currently in the event.  
  *Example Reply: "There are 5 Players in Event."*

- `.event start <eventName>`  
  Launch a new event with a custom name. Only one event can run at a time.  
  *Fails if an event is already active.*

- `.event end`  
  Shut down the active event and reset the system.  
  *Fails if no event is active.*

- `.event ban <playerName>`  
  Ban a player from joining events. Persistent across sessions.  
  *Fails if the player doesn’t exist.*

- `.event unban <playerName>`  
  Lift a ban, allowing the player back into events.  
  *Fails if the player isn’t banned or doesn’t exist.*

- `.event kick <playerName>`  
  Remove a player from the current event.  
  *Fails if the player isn’t in the event or doesn’t exist.*

- `.event gather`  
  Teleport all event participants to your location.  
  *Example Reply: "Teleported all event players to you."*  
  *Fails if no event is active.*

### Player Commands (Available to All)
- `.event join`  
  Join the active event (requires an empty inventory).  
  *Fails if the event is paused, you’re banned, or no event is active.*  
  *Example Reply: "You have joined the event. Use .event leave to exit."*

- `.event leave`  
  Exit the event and return to your original position.  
  *Fails if you’re not in the event.*  
  *Example Reply: "You have left the event and were teleported back."*


## Configuration

No config file is required—this plugin is ready to use out of the box. Future updates may add options like custom teleport locations or event rules.


## Dependencies

- `BepInEx`
- `VampireCommandFramework`
- `Bloodstone.API`
- `Bloody.Core`

For bug reports or suggestions, join Nightbane discord community!
https://discord.gg/23Bd9ryzUH
