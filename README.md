# SpectateEnemies
 Allows you to spectate enemies in Lethal Company.

## Usage
When dead, pressing your "Swap" key (default: E) will swap between spectating players and spectating enemies. When spectating either side, you can left click to move to the next player or enemy as normal. The mod will also remember your last spectated enemy, as long as they are still alive.

Pressing your "Flashlight" key (default: RMB) will toggle a flashlight on your spectator camera to see enemies and players better in the dark.

Pressing the "Zoom Out/Zoom In" keys (default: Scroll Wheel) while spectating an enemy will zoom the camera in and out. This can help with bigger enemies that are hard to see at default zoom.

 ## Installation
Requires the latest version of [BepInEx 5](https://github.com/BepInEx/BepInEx) and [InputUtils](https://thunderstore.io/c/lethal-company/p/Rune580/LethalCompany_InputUtils/). After both have been installed, drag `SpectateEnemy.dll` into the `BepInEx/plugins` folder in the game's root directory.

## Keybinds
You can set your keybinds in the in-game keybind menu. The following SpectateEnemies keybinds correspond to these default controls:

"Swap between Players/Enemies" : E 
"Open Config Menu" : Insert 
"Toggle Flashlight" : Right Button 
"Zoom Out" : Scroll Wheel Down 
"Zoom In" : Scroll Wheel Up

## Config Menu
Pressing your "Menu" key (default: Insert) will open the config menu. Clicking the box next to an enemy's name will enable/disable spectating that enemy.

You can also find a config in BepInEx/configs/SpectateEnemy.cfg, although I wouldn't mess with this file unless you know what you're doing.

If you would like to hide the controls tooltip on the right side of the screen, open the config file and change the `Hide Controls` option to `true`.
