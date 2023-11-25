using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(EnemyAI), "Start")]
    public class EnemyAI_Patches
    {
        private static void Postfix(EnemyAI __instance)
        {
            if (!Plugin.doSpectatePassives && (__instance.enemyType.enemyName == "Docile Locust Bees" || __instance.enemyType.enemyName == "Manticoil"))
                return;
            Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
            s.enemyName = __instance.enemyType.enemyName;
        }
    }
}
