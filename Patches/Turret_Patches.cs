using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(Turret), "Start")]
    public class Turret_Patches
    {
        private static void Postfix(Turret __instance)
        {
            if (Plugin.doSpectateTurrets)
            {
                Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
                s.type = SpectatableType.Turret;
            }
        }
    }
}
