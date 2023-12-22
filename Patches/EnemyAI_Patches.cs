using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(EnemyAI), "Start")]
    internal class EnemyAI_Patches
    {
        private static void Postfix(EnemyAI __instance)
        {
            Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
            s.enemyName = __instance.enemyType.enemyName;
        }
    }
}
