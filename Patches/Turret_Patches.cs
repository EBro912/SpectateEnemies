using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(Turret), "Start")]
    internal class Turret_Patches
    {
        private static void Postfix(Turret __instance)
        {
            Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
            s.type = SpectatableType.Turret;
            s.enemyName = "Turret";
        }
    }
}
