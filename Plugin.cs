using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using System.Reflection;

namespace SpectateEnemy;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    public static MethodInfo raycastSpectate = null;
    public static MethodInfo displaySpectatorTip = null;

    private Harmony harmony;

    private void Awake()
    {
        harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        raycastSpectate = AccessTools.Method(typeof(PlayerControllerB), "RaycastSpectateCameraAroundPivot");
        displaySpectatorTip = AccessTools.Method(typeof(HUDManager), "DisplaySpectatorTip");
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} loaded!");
    }
}
