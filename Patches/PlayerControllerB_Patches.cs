using HarmonyLib;
using System.Linq;
using GameNetcodeStuff;
using System;
using UnityEngine;

namespace SpectateEnemy.Patches
{
    internal class Handler {
        public static Spectatable[] spectatorList;

        public static bool Spectate()
        {        
            if (Plugin.spectatingEnemies)
            {
                if (spectatorList.Length == 0)
                {
                    Plugin.spectatingEnemies = false;
                    return true;
                }
                Plugin.spectatedEnemyIndex++;
                if (Plugin.spectatedEnemyIndex >= spectatorList.Length)
                {
                    Plugin.spectatedEnemyIndex = 0;
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
                Plugin.spectatingEnemies = !Plugin.spectatingEnemies;
                if (Plugin.spectatingEnemies)
                {
                    Handler.spectatorList = UnityEngine.Object.FindObjectsByType<Spectatable>(FindObjectsSortMode.None);
                    if (Handler.spectatorList.Length == 0)
                    {
                        Plugin.spectatingEnemies = false;
                        return true;
                    }
                    Plugin.spectatedEnemyIndex = 0;
                    __instance.spectatedPlayerScript = null;
                }
                else
                {
                    Plugin.spectatedEnemyIndex = -1;
                    __instance.spectatedPlayerScript = __instance.playersManager.allPlayerScripts.FirstOrDefault(x => !x.isPlayerDead);
                }
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
                return Handler.Spectate();
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public class PlayerControllerB_LateUpdate
    {
        private static void Postfix(PlayerControllerB __instance)
        {
            if (Plugin.spectatingEnemies)
            {
                if (Handler.spectatorList.Length == 0)
                {
                    Plugin.spectatingEnemies = false;
                    return;
                }
                if (Plugin.spectatedEnemyIndex >= Handler.spectatorList.Length)
                {
                    Plugin.spectatedEnemyIndex = 0;
                }
                Spectatable currentEnemy = Handler.spectatorList[Plugin.spectatedEnemyIndex];
                if (currentEnemy == null)
                {
                    Plugin.spectatedEnemyIndex++;
                    if (Plugin.spectatedEnemyIndex >= Handler.spectatorList.Length)
                    {
                        Plugin.spectatedEnemyIndex = 0;
                    }
                    return;
                }
                Vector3? position = GetSpectatePosition(currentEnemy);
                if (!position.HasValue)
                {
                    Plugin.spectatedEnemyIndex++;
                    if (Plugin.spectatedEnemyIndex >= Handler.spectatorList.Length)
                    {
                        Plugin.spectatedEnemyIndex = 0;
                    }
                    return;
                }
                __instance.spectateCameraPivot.position = position.Value + GetZoomDistance(currentEnemy);
                HUDManager.Instance.spectatingPlayerText.text = "(Spectating: " + currentEnemy.enemyName + ")";
                Plugin.raycastSpectate.Invoke(__instance, []);
            }
        }

        private static Vector3 GetZoomDistance(Spectatable obj)
        {
            if (obj.enemyName == "ForestGiant")
                return Vector3.up * 3;
            if (obj.enemyName == "MouthDog" || obj.enemyName == "Jester")
                return Vector3.up * 2;
            else
                return Vector3.up;
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
    }
}
