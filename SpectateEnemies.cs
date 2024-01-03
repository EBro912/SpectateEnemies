using BepInEx;
using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpectateEnemy
{
    internal class SpectateEnemies : MonoBehaviour
    {
        public static SpectateEnemies Instance;

        private static readonly Dictionary<string, bool> settings = [];

        private bool WindowOpen = false;
        private Rect window = new(10, 10, 500, 300);

        public int SpectatedEnemyIndex = -1;
        public bool SpectatingEnemies = false;
        public Spectatable[] SpectatorList;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            SetupKeybinds();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void SetupKeybinds()
        {
            Plugin.Inputs.SwapKey.performed += OnSwapKeyPressed;
            Plugin.Inputs.MenuKey.performed += OnMenuKeyPressed;
            Plugin.Inputs.FlashlightKey.performed += OnFlashlightKeyPressed;
        }

        private void OnSwapKeyPressed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
            {
                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
                if (player.IsOwner && player.isPlayerDead && !StartOfRound.Instance.shipIsLeaving && (!player.IsServer || player.isHostPlayerObject))
                {
                    ToggleSpectatingMode(player);
                }
            }
        }

        private void OnMenuKeyPressed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
            {
                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
                if (player.IsOwner && player.isPlayerDead && !StartOfRound.Instance.shipIsLeaving && (!player.IsServer || player.isHostPlayerObject))
                {
                    Toggle();
                }
            }
        }

        private void OnFlashlightKeyPressed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
            {
                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
                if (HUDManager.Instance != null && player.IsOwner && player.isPlayerDead && !StartOfRound.Instance.shipIsLeaving && (!player.IsServer || player.isHostPlayerObject))
                {
                    // flashlight already exists on spectator camera, thanks zeekerss
                    Light light = HUDManager.Instance.playersManager.spectateCamera.GetComponent<Light>();
                    if (light != null)
                    {
                        light.enabled = !light.enabled;
                    }
                }
            }  
        }

        private void LateUpdate()
        {
            if (SpectatingEnemies)
            {
                if (SpectatorList.Length == 0)
                {
                    SpectatingEnemies = false;
                    return;
                }
                if (SpectatedEnemyIndex >= SpectatorList.Length)
                {
                    GetNextValidSpectatable();
                    return;
                }
                Spectatable currentEnemy = SpectatorList.ElementAtOrDefault(SpectatedEnemyIndex);
                if (currentEnemy == null)
                {
                    GetNextValidSpectatable();
                    return;
                }
                Vector3? position = GetSpectatePosition(currentEnemy);
                if (!position.HasValue)
                {
                    GetNextValidSpectatable();
                    return;
                }
                if (currentEnemy.enemyName == "Enemy")
                {
                    TryFixName(ref currentEnemy);
                }
                HUDManager.Instance.localPlayer.spectateCameraPivot.position = position.Value + GetZoomDistance(currentEnemy);
                if (currentEnemy.type == SpectatableType.Masked && currentEnemy.maskedName != string.Empty)
                    HUDManager.Instance.spectatingPlayerText.text = "(Spectating: " + currentEnemy.maskedName + ")";
                else
                    HUDManager.Instance.spectatingPlayerText.text = "(Spectating: " + currentEnemy.enemyName + ")";
                Plugin.raycastSpectate.Invoke(HUDManager.Instance.localPlayer, []);
            }
        }

        private Vector3? GetSpectatePosition(Spectatable obj)
        {
            if (obj.type == SpectatableType.Enemy || obj.type == SpectatableType.Masked)
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
                Debug.LogError("[SpectateEnemy]: Error when spectating: no handler for SpectatableType " + obj.type);
            }
            return null;
        }

        private Vector3 GetZoomDistance(Spectatable obj)
        {
            if (obj.enemyName == "ForestGiant")
                return Vector3.up * 3;
            if (obj.enemyName == "MouthDog" || obj.enemyName == "Jester")
                return Vector3.up * 2;
            else
                return Vector3.up;
        }

        private void TryFixName(ref Spectatable obj)
        {
            if (obj.gameObject.TryGetComponent(out MaskedPlayerEnemy masked))
            {
                if (masked.mimickingPlayer != null)
                {
                    obj.enemyName = masked.mimickingPlayer.playerUsername;
                }
                else
                {
                    obj.enemyName = masked.enemyType.enemyName;
                }
            }
            else if (obj.gameObject.TryGetComponent(out EnemyAI enemy))
            {
                obj.enemyName = enemy.enemyType.enemyName;
            }
            else if (obj.gameObject.TryGetComponent<Turret>(out _))
            {
                obj.enemyName = "Turret";
            }
            else if (obj.gameObject.TryGetComponent<Landmine>(out _))
            {
                obj.enemyName = "Landmine";
            }
        }

        public void ToggleSpectatingMode(PlayerControllerB __instance)
        {
            SpectatingEnemies = !SpectatingEnemies;
            if (SpectatingEnemies)
            {
                SpectatorList = FindObjectsByType<Spectatable>(FindObjectsSortMode.None);
                if (SpectatorList.Length == 0)
                {
                    SpectatingEnemies = false;
                    Plugin.displaySpectatorTip.Invoke(HUDManager.Instance, ["No enemies to spectate"]);
                    return;
                }
                if (SpectatedEnemyIndex == -1 || SpectatedEnemyIndex >= SpectatorList.Length)
                {
                    if (__instance.spectatedPlayerScript == null)
                    {
                        GetNextValidSpectatable();
                    }
                    else
                    {
                        List<Spectatable> matches = SpectatorList.Where(x => settings[x.enemyName]).ToList();
                        if (matches.Count == 0)
                        {
                            Plugin.displaySpectatorTip.Invoke(HUDManager.Instance, ["No enemies to spectate"]);
                            return;
                        }
                        float closest = 999999f;
                        int index = 0;
                        for (int i = 0; i < matches.Count; i++)
                        {
                            float dist = (matches[i].transform.position - __instance.spectatedPlayerScript.transform.position).sqrMagnitude;
                            if (dist < closest * closest)
                            {
                                closest = dist;
                                index = i;
                            }
                        }
                        SpectatedEnemyIndex = index;
                    }
                }
                else
                {
                    if (!settings[SpectatorList[SpectatedEnemyIndex].enemyName])
                    {
                        GetNextValidSpectatable();
                    }
                }
                __instance.spectatedPlayerScript = null;
            }
            else
            {
                __instance.spectatedPlayerScript = __instance.playersManager.allPlayerScripts.FirstOrDefault(x => !x.isPlayerDead && x.isPlayerControlled);
                HUDManager.Instance.spectatingPlayerText.text = "(Spectating: " + __instance.spectatedPlayerScript.playerUsername + ")";
            }
        }

        public void PopulateSettings(SelectableLevel level)
        {
            foreach (SpawnableEnemyWithRarity t in level.Enemies)
            {
                settings.TryAdd(t.enemyType.enemyName, true);
            }
            foreach (SpawnableEnemyWithRarity t in level.OutsideEnemies)
            {
                settings.TryAdd(t.enemyType.enemyName, true);
            }
            foreach (SpawnableEnemyWithRarity t in level.DaytimeEnemies)
            {
                settings.TryAdd(t.enemyType.enemyName, false);
            }
            settings.TryAdd("Landmine", false);
            settings.TryAdd("Turret", false);

            // TODO: this is terrible, improve
            try
            {
                if (File.Exists(Paths.ConfigPath + "/SpectateEnemy.cfg"))
                {
                    string[] config = File.ReadAllLines(Paths.ConfigPath + "/SpectateEnemy.cfg");
                    if (config[3] == "[Config]")
                    {
                        Debug.LogWarning("[SpectateEnemies]: Config not found, using default values!");
                        return;
                    }
                    foreach (string s in config)
                    {
                        string[] c = s.Split(':');
                        if (c.Length != 2) continue;
                        if (settings.ContainsKey(c[0]))
                            if (bool.TryParse(c[1], out bool value))
                                settings[c[0]] = value;
                    }
                    Debug.LogWarning("[SpectateEnemies]: Config loaded");
                }
                else
                {
                    Debug.LogWarning("[SpectateEnemies]: Config not found, using default values!");
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("[SpectateEnemies]: Config failed to load, using default values!");
            }

            // AssertSettings();
        }

        private void OnApplicationQuit()
        {
            StringBuilder sb = new();
            foreach (var s in settings)
            {
                sb.Append(s.Key);
                sb.Append(':');
                sb.Append(s.Value);
                sb.AppendLine();
            }
            File.WriteAllText(Paths.ConfigPath + "/SpectateEnemy.cfg", sb.ToString());
            Debug.LogWarning("[SpectateEnemies]: Config saved");
        }

        public bool SpectateNextEnemy()
        {
            if (SpectatorList.Length == 0)
            {
                SpectatingEnemies = false;
                return true;
            }
            GetNextValidSpectatable();
            return false;
        }

        private void GetNextValidSpectatable()
        {
            int enemiesChecked = 0;
            int current = SpectatedEnemyIndex;
            while (enemiesChecked < SpectatorList.Length)
            {
                current++;
                if (current >= SpectatorList.Length)
                {
                    current = 0;
                }
                if (settings[SpectatorList[current].enemyName])
                {
                    SpectatedEnemyIndex = current;
                    return;
                }
                enemiesChecked++;
            }
            SpectatingEnemies = false;
            Plugin.displaySpectatorTip.Invoke(HUDManager.Instance, ["No enemies to spectate"]);
        }

        private void AssertSettings()
        {
            foreach (var t in settings)
            {
                Debug.LogWarning($"{t.Key} : {t.Value}");
            }
        }

        public bool GetSetting(string name)
        {
            if (settings.ContainsKey(name))
                return settings[name];
            return false;
        }

        public void Toggle()
        {
            WindowOpen = !WindowOpen;
        }

        public void Hide()
        {
            WindowOpen = false;
        }

        public bool IsMenuOpen()
        {
            return WindowOpen;
        }

        private void OnGUI()
        {
            if (WindowOpen)
            { 
                GUI.color = Color.gray;
                window = GUI.Window(0, window, DrawGUI, "Spectator Settings");
            }
        }

        private void DrawGUI(int windowID)
        {
            if (windowID == 0)
            {
                GUI.Label(new Rect(10, 30, 500, 100), "Tip: Checking the box next to an enemy name will enable spectating it.");
                int x = 0;
                int y = 0;
                foreach (string k in settings.Keys.ToList())
                {
                    settings[k] = GUI.Toggle(new Rect(10 + (150 * x), 60 + (30 * y), 150, 20), settings[k], k);
                    x++;
                    if (x == 3)
                    {
                        x = 0;
                        y++;
                    }
                }
            }
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
    }
}
