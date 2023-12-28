using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    internal class GameNetworkManager_Patches
    {
        private static void Postfix()
        {
            SpectateEnemies.Instance.SpectatedEnemyIndex = -1;
            SpectateEnemies.Instance.SpectatingEnemies = false;
            SpectateEnemies.Instance.Hide();
        }
    }
}
