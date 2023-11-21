using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(Landmine), "Start")]
    public class Landmine_Patches
    {
        private static void Postfix(Landmine __instance)
        {
            if (Plugin.doSpectateLandmines)
            {
                Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
                s.type = SpectatableType.Landmine;
                s.name = "Landmine";
            }
        }
    }
}
