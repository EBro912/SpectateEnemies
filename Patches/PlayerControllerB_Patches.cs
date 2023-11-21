using HarmonyLib;
using System.Linq;
using GameNetcodeStuff;
using System;
using UnityEngine;

namespace SpectateEnemy.Patches
{
    internal class Handler {
        public static Spectatable[] spectatorList;

        public static bool Spectate(PlayerControllerB __instance)
        {        
            if (Plugin.spectatedEnemyIndex > -1)
            {
                Plugin.spectatedEnemyIndex++;
                if (Plugin.spectatedEnemyIndex >= spectatorList.Length)
                {
                    __instance.spectatedPlayerScript = __instance.playersManager.allPlayerScripts.FirstOrDefault(x => !x.isPlayerDead);
                    Plugin.spectatedEnemyIndex = -1;
                    return true;
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Interact_performed")]
    public class PlayerControllerB_Interact
    {
        private static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.IsOwner && __instance.isPlayerDead && (!__instance.IsServer || __instance.isHostPlayerObject))
            {
                return Handler.Spectate(__instance);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Use_performed")]
    public class PlayerControllerB_Use
    {
        private static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.IsOwner && __instance.isPlayerDead && (!__instance.IsServer || __instance.isHostPlayerObject))
            {
                return Handler.Spectate(__instance);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public class PlayerControllerB_LateUpdate
    {
        private static void Postfix(PlayerControllerB __instance)
        {
            if (Plugin.spectatedEnemyIndex > -1)
            {
                if (Plugin.spectatedEnemyIndex >= Handler.spectatorList.Length)
                {
                    __instance.spectatedPlayerScript = __instance.playersManager.allPlayerScripts.FirstOrDefault(x => !x.isPlayerDead);
                    Plugin.spectatedEnemyIndex = -1;
                    return;
                }
                Spectatable currentEnemy = Handler.spectatorList[Plugin.spectatedEnemyIndex];
                if (currentEnemy == null)
                {
                    Plugin.spectatedEnemyIndex++;
                    if (Plugin.spectatedEnemyIndex >= Handler.spectatorList.Length)
                    {
                        __instance.spectatedPlayerScript = __instance.playersManager.allPlayerScripts.FirstOrDefault(x => !x.isPlayerDead);
                        Plugin.spectatedEnemyIndex = -1;
                    }
                    return;
                }
                Vector3? position = GetSpectatePosition(currentEnemy);
                if (!position.HasValue)
                {
                    Plugin.spectatedEnemyIndex++;
                    if (Plugin.spectatedEnemyIndex >= Handler.spectatorList.Length)
                    {
                        __instance.spectatedPlayerScript = __instance.playersManager.allPlayerScripts.FirstOrDefault(x => !x.isPlayerDead);
                        Plugin.spectatedEnemyIndex = -1;
                    }
                    return;
                }
                __instance.spectateCameraPivot.position = position.Value + Vector3.up * 0.7f;
                HUDManager.Instance.spectatingPlayerText.text = "(Spectating: " + currentEnemy.enemyName + ")";
                Plugin.raycastSpectate.Invoke(__instance, Array.Empty<object>());
            }
        }

        private static Vector3? GetSpectatePosition(Spectatable obj)
        {
            if (obj.type == SpectatableType.Enemy)
            {
                EnemyAI enemy = obj.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    return enemy.eye == null ? enemy.transform.position : enemy.eye.position;
                }
            }
            else if (obj.type == SpectatableType.Turret)
            {
                Turret enemy = obj.GetComponent<Turret>();
                if (enemy != null)
                {
                    return enemy.centerPoint.transform.position;
                }
            }
            else if (obj.type == SpectatableType.Landmine)
            {
                return obj.transform.position;
            }
            else
            {
                Debug.LogError("[SpectateEnemy]: Error when spectating: no handler for SpectatableType " +  obj.type);
            }
            return null;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "SpectateNextPlayer")]
    public class PlayerControllerB_SpectateNext
    {
        private static bool Prefix()
        {
            return Plugin.spectatedEnemyIndex == -1;
        }

        private static void Postfix(PlayerControllerB __instance)
        {
            if (__instance.spectatedPlayerScript != null)
            {
                // keep track of this so we know when we loop back to the first player
                // when we do, we should start spectating enemies
                if (Plugin.firstPlayerSpectated == null)
                {
                    Plugin.firstPlayerSpectated = __instance.spectatedPlayerScript.playerClientId;
                    return;
                }
                if (Plugin.firstPlayerSpectated == __instance.spectatedPlayerScript.playerClientId)
                {
                    Handler.spectatorList = UnityEngine.Object.FindObjectsByType<Spectatable>(FindObjectsSortMode.None);
                    if (Handler.spectatorList.Length == 0)
                    {
                        return;
                    }
                    Plugin.spectatedEnemyIndex = 0;
                    __instance.spectatedPlayerScript = null;
                    Plugin.firstPlayerSpectated = null;
                }
            }
        }
    }
}
