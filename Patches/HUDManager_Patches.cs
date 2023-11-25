using HarmonyLib;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(HUDManager), "Update")]
    public class HUDManager_Patches
    {
        private static void Postfix(HUDManager __instance)
        {
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead && !TimeOfDay.Instance.shipLeavingAlertCalled)
            {
                __instance.holdButtonToEndGameEarlyText.text += "\nToggle between Players/Enemies\n: [E]";
            }
        }
    }
}
