using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(HUDManager), "Update")]
    internal class HUDManager_Patches
    {
        private static void Postfix(HUDManager __instance)
        {
            if (GameNetworkManager.Instance.localPlayerController != null && GameNetworkManager.Instance.localPlayerController != null)
            {
                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
                if (player.isPlayerDead)
                {
                    if (StartOfRound.Instance.shipIsLeaving)
                    {
                        SpectateEnemies.Instance.Hide();
                        Light light = __instance.playersManager.spectateCamera.GetComponent<Light>();
                        if (light != null)
                        {
                            light.enabled = false;
                        }
                        return;
                    }
                    string swapKey = InputControlPath.ToHumanReadableString(Plugin.Inputs.SwapKey.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    string menuKey = InputControlPath.ToHumanReadableString(Plugin.Inputs.MenuKey.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    string flashlightKey = InputControlPath.ToHumanReadableString(Plugin.Inputs.FlashlightKey.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    string zoomOutKey = InputControlPath.ToHumanReadableString(Plugin.Inputs.ZoomOutKey.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    string zoomInKey = InputControlPath.ToHumanReadableString(Plugin.Inputs.ZoomInKey.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    if (!SpectateEnemies.Instance.HideControls.Value)
                    {
                        // who needs to change the y position when u can just \n: sunglasses:
                        if (SpectateEnemies.Instance.SpectatingEnemies)
                            __instance.holdButtonToEndGameEarlyText.text += $"\n\n\n\n\nSpectate Players: [{swapKey}]\nFlashlight : [{flashlightKey}]\nZoom Out : [{zoomOutKey}]\nZoom In : [{zoomInKey}]\nConfig Menu : [{menuKey}]";
                        else
                            __instance.holdButtonToEndGameEarlyText.text += $"\n\n\n\n\nSpectate Enemies: [{swapKey}]\nFlashlight : [{flashlightKey}]\nConfig Menu : [{menuKey}]";
                    }

                }
            }
        }
    }
}
