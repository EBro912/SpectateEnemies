﻿using HarmonyLib;
using UnityEngine;

namespace SpectateEnemy.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager), "Start")]
    internal class QuickMenuManager_Patches
    {
        private static void Postfix()
        {
            if (SpectateEnemies.Instance == null)
            {
                GameObject obj = new("SpectateEnemiesObject");
                SpectateEnemies spec = obj.AddComponent<SpectateEnemies>();
                spec.PopulateSettings();
            }
        }
    }
}
