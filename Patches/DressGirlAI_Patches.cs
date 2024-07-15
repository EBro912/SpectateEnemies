using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(DressGirlAI), "Start")]
    internal class DressGirlAI_Patches
    {
        private static void Postfix(DressGirlAI __instance)
        {
            Spectatable s = __instance.gameObject.GetComponent<Spectatable>();
            if (s == null)
            {
                s = __instance.gameObject.AddComponent<Spectatable>();
                s.enemyName = __instance.enemyType.enemyName;
                s.enemyInstance = __instance;
            }
            s.type = SpectatableType.GhostGirl;
        }
    }
}
