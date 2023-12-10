using HarmonyLib;
using UnityEngine;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(HUDManager), "Update")]
    public class HUDManager_Patches
    {
        private static void Postfix(HUDManager __instance)
        {
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                if (StartOfRound.Instance.shipIsLeaving)
                {
                    Light light = __instance.playersManager.spectateCamera.GetComponent<Light>();
                    if (light != null)
                    {
                        light.enabled = false;
                    }
                    return;
                }
                // who needs to change the y position when u can just \n :sunglasses:
                __instance.holdButtonToEndGameEarlyText.text += $"\n\n\n\n\nSwitch to {(Plugin.spectatingEnemies ? "Players" : "Enemies")}: [E]\nToggle Flashlight : [RMB] (Click)";
                if (__instance.playerActions.Movement.PingScan.WasReleasedThisFrame())
                {
                    // flashlight already exists on spectator camera, thanks zeekerss
                    Light light = __instance.playersManager.spectateCamera.GetComponent<Light>();
                    if (light != null)
                    {
                        light.enabled = !light.enabled;
                    }
                }
            }
        }
    }
}
