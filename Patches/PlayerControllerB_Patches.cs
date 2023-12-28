using HarmonyLib;
using GameNetcodeStuff;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Interact_performed")]
    internal class PlayerControllerB_Interact
    {
        private static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.IsOwner && __instance.isPlayerDead && !StartOfRound.Instance.shipIsLeaving && (!__instance.IsServer || __instance.isHostPlayerObject))
            {
                SpectateEnemies.Instance.ToggleSpectatingMode(__instance);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "ActivateItem_performed")]
    internal class PlayerControllerB_Use
    {
        private static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.IsOwner && __instance.isPlayerDead && !StartOfRound.Instance.shipIsLeaving && (!__instance.IsServer || __instance.isHostPlayerObject))
            {
                if (SpectateEnemies.Instance.IsMenuOpen())
                {
                    return false;
                }
                if (SpectateEnemies.Instance.SpectatingEnemies)
                {
                    SpectateEnemies.Instance.SpectateNextEnemy();
                }
                return true;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "SpectateNextPlayer")]
    internal class PlayerControllerB_SpectateNext
    {
        private static bool Prefix()
        {
            return !SpectateEnemies.Instance.SpectatingEnemies;
        }
    }
}
