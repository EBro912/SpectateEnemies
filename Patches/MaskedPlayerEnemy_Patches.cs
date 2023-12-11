using HarmonyLib;

namespace SpectateEnemy.Patches
{
    // MaskedPlayerEnemy inherits EnemyAI but doesn't call base.Start... wtf?
    [HarmonyPatch(typeof(MaskedPlayerEnemy), "Start")]
    public class MaskedPlayerEnemy_Patches
    {
        private static void Postfix(MaskedPlayerEnemy __instance)
        {
            Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
            if (__instance.mimickingPlayer != null)
            {
                // at least i can do this now
                s.enemyName = __instance.mimickingPlayer.playerUsername;
            }
            else
            {
                s.enemyName = __instance.enemyType.enemyName;
            }
        }
    }
}
