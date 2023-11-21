using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(EnemyAI), "Start")]
    public class EnemyAI_Patches
    {
        private static void Postfix(EnemyAI __instance)
        {
            __instance.gameObject.AddComponent<Spectatable>();
        }
    }
}
