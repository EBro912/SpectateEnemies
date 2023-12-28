using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
    internal class StartOfRound_Patches
    {
        private static void Postfix()
        {
            SpectateEnemies.Instance.SpectatedEnemyIndex = -1;
            SpectateEnemies.Instance.SpectatingEnemies = false;
            SpectateEnemies.Instance.Hide();
        }
    }
}
