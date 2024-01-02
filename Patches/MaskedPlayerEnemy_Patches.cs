using HarmonyLib;

namespace SpectateEnemy.Patches
{
    // MaskedPlayerEnemy inherits EnemyAI but doesn't call base.Start... wtf?
    [HarmonyPatch(typeof(MaskedPlayerEnemy), "Start")]
    internal class MaskedPlayerEnemy_Patches
    {
        private static void Postfix(MaskedPlayerEnemy __instance)
        {
            Spectatable s = __instance.gameObject.AddComponent<Spectatable>();
            s.type = SpectatableType.Masked;
            s.enemyName = __instance.enemyType.enemyName;
            if (__instance.mimickingPlayer != null)
            {
                s.maskedName = __instance.mimickingPlayer.playerUsername;
            }
        }
    }
}
