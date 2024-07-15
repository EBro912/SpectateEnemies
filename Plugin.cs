using BepInEx;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using System.Reflection;

namespace SpectateEnemy;

[BepInPlugin("SpectateEnemy", "SpectateEnemy", "2.5.0")]
[BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
internal class Plugin : BaseUnityPlugin
{
    public static MethodInfo raycastSpectate = null;
    public static MethodInfo displaySpectatorTip = null;

    public static Inputs Inputs = new();
    public static ConfigFile Configuration;

    private Harmony harmony;

    private void Awake()
    {
        Configuration = Config;

        harmony = new Harmony("SpectateEnemy");
        harmony.PatchAll();

        raycastSpectate = AccessTools.Method(typeof(PlayerControllerB), "RaycastSpectateCameraAroundPivot");
        displaySpectatorTip = AccessTools.Method(typeof(HUDManager), "DisplaySpectatorTip");
        Logger.LogInfo("SpectateEnemy loaded!");
    }
}
