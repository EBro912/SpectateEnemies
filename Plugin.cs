using BepInEx;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using System.Reflection;

namespace SpectateEnemy;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static int spectatedEnemyIndex = -1;
    public static bool spectatingEnemies = false;
    public static MethodInfo raycastSpectate = null;

    private ConfigEntry<bool> spectateTurrets;
    private ConfigEntry<bool> spectateLandmines;
    private ConfigEntry<bool> spectatePassives;
    public static bool doSpectateTurrets;
    public static bool doSpectateLandmines;
    public static bool doSpectatePassives;

    private Harmony harmony;

    private void Awake()
    {
        spectateTurrets = Config.Bind("Config", "Spectate Turrets", false, "Enables spectating turrets.");
        doSpectateTurrets = spectateTurrets.Value;
        spectateLandmines = Config.Bind("Config", "Spectate Landmines", false, "Enables spectating landmines.");
        doSpectateLandmines = spectateLandmines.Value;
        spectatePassives = Config.Bind("Config", "Spectate Passives", false, "Enables spectating passive enemies, such as Docile Locust Bees and Manticoils.");
        doSpectatePassives = spectatePassives.Value;

        harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        raycastSpectate = AccessTools.Method(typeof(PlayerControllerB), "RaycastSpectateCameraAroundPivot");
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} loaded!");
    }
}
