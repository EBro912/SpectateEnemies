using HarmonyLib;
using UnityEngine;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    internal class GameNetworkManager_Disconnect
    {
        private static void Postfix()
        {
            if (SpectateEnemies.Instance != null)
            {
                SpectateEnemies.Instance.SpectatedEnemyIndex = -1;
                SpectateEnemies.Instance.SpectatingEnemies = false;
                SpectateEnemies.Instance.Hide();
            }
        }
    }

    [HarmonyPatch(typeof(GameNetworkManager), "OnApplicationQuit")]
    internal class GameNetworkManager_Quit
    {
        private static void Prefix()
        {
            if (SpectateEnemies.Instance != null)
            {
                Plugin.Configuration.Save();
                Debug.LogWarning("[SpectateEnemies]: Config saved");
            }
        }
    }
}
