using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    public class GameNetworkManager_Patches
    {
        private static void Postfix()
        {
            Plugin.spectatedEnemyIndex = -1;
            Plugin.spectatingEnemies = false;
        }
    }
}
