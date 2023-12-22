using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(Landmine), "Start")]
    internal class Landmine_Patches
    {
        private static void Postfix(Landmine __instance)
        {
            Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
            s.type = SpectatableType.Landmine;
            s.enemyName = "Landmine";
        }
    }
}
