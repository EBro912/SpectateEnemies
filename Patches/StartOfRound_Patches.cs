using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
    public class StartOfRound_Patches
    {
        private static void Postfix()
        {
            Plugin.spectatedEnemyIndex = -1;
            Plugin.firstPlayerSpectated = null;
        }
    }
}
