# SpectateEnemies
 Allows you to spectate enemies in Lethal Company.

## Usage
When dead, pressing your "Interact" key (default: E) will swap between spectating players and spectating enemies. When spectating either side, you can left click to move to the next player or enemy as normal. The mod will also remember your last spectated enemy, as long as they are still alive.

Pressing RMB will toggle a flashlight on your spectator camera, allowing you to see better in darker areas.

## Config Options
Located in `BepInEx/config/SpectateEnemy.cfg`

- `Spectate Turrets` : Enables/disables spectating turrets. (Default: false)
- `Spectate Landmines`: Enables/disables spectating landmines. (Default: false)
- `Spectate Passives`: Enables/disables spectating passive enemies, such as Manticoils. (Default: false)

 ## Installation
 Requires the latest version of [BepInEx 5](https://github.com/BepInEx/BepInEx). After BepInEx has been installed, drag `SpectateEnemy.dll` into the `BepInEx/plugins` folder in the game's root directory. You will need to run the game once for the `SpectateEnemy.cfg` file to generate.
