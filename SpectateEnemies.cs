using BepInEx.Configuration;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpectateEnemy
{
    internal class SpectateEnemies : MonoBehaviour
    {
        public static SpectateEnemies Instance;

        private static readonly Dictionary<string, ConfigEntry<bool>> settings = [];

        private bool WindowOpen = false;
        private Rect window = new(10, 10, 500, 400);
        private Vector2 scrollPos = Vector2.zero;

        public int SpectatedEnemyIndex = -1;
        public bool SpectatingEnemies = false;
        public Spectatable[] SpectatorList;
        public float ZoomLevel = 1f;
        public ConfigEntry<bool> HideControls;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            SetupKeybinds();
            Instance = this;
            DontDestroyOnLoad(gameObject);

            HideControls = Plugin.Configuration.Bind("Config", "Hide Controls", false, "Hides the controls toolip on the right hand side.");
        }

        private void SetupKeybinds()
        {
            Plugin.Inputs.SwapKey.performed += OnSwapKeyPressed;
            Plugin.Inputs.MenuKey.performed += OnMenuKeyPressed;
            Plugin.Inputs.FlashlightKey.performed += OnFlashlightKeyPressed;
            Plugin.Inputs.ZoomOutKey.performed += OnZoomOutPressed;
            Plugin.Inputs.ZoomInKey.performed += OnZoomInPressed;
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

        private void OnZoomOutPressed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!SpectatingEnemies) return;
            ZoomLevel += 0.1f;
            if (ZoomLevel > 10f)
                ZoomLevel = 10f;
        }

        private void OnZoomInPressed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!SpectatingEnemies) return;
            ZoomLevel -= 0.1f;
            if (ZoomLevel < 1f)
                ZoomLevel = 1f;
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
                Spectatable currentEnemy = SpectatorList.ElementAtOrDefault(SpectatedEnemyIndex);
                if (currentEnemy == null)
                {
                    GetNextValidSpectatable();
                    return;
                }
                if (currentEnemy.type == SpectatableType.Enemy || currentEnemy.type == SpectatableType.Masked || currentEnemy.type == SpectatableType.GhostGirl)
                {
                    if (currentEnemy.enemyInstance != null && currentEnemy.enemyInstance.isEnemyDead)
                    {
                        GetNextValidSpectatable();
                        return;
                    }
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

                GameNetworkManager.Instance.localPlayerController.spectateCameraPivot.position = position.Value;
                if (currentEnemy.type == SpectatableType.Masked && currentEnemy.maskedName != string.Empty)
                    HUDManager.Instance.spectatingPlayerText.text = string.Format("(Spectating: {0}) [{1:F1}x]", currentEnemy.maskedName, ZoomLevel);
                else if (currentEnemy.type == SpectatableType.GhostGirl)
                {
                    // TODO: improve
                    DressGirlAI ghost = currentEnemy.gameObject.GetComponent<DressGirlAI>();
                    ghost.EnableEnemyMesh(true, true);
                    PlayerControllerB target = ghost.targetPlayer;
                    if (target != null)
                    {
                        HUDManager.Instance.spectatingPlayerText.text = string.Format("(Spectating: {0}) [{1:F1}x]\n(Targeting: {2})", currentEnemy.maskedName, ZoomLevel, target.playerUsername);
                    }
                    else
                    {
                        HUDManager.Instance.spectatingPlayerText.text = string.Format("(Spectating: {0}) [{1:F1}x]\n(No Target)", currentEnemy.maskedName, ZoomLevel);
                    }
                }
                else
                    HUDManager.Instance.spectatingPlayerText.text = string.Format("(Spectating: {0}) [{1:F1}x]", currentEnemy.enemyName, ZoomLevel);
                Plugin.raycastSpectate.Invoke(GameNetworkManager.Instance.localPlayerController, []);
                // Thanks HalfyRed!
                GameNetworkManager.Instance.localPlayerController.spectateCameraPivot.GetComponentInChildren<Camera>().transform.localPosition = Vector3.back * (ZoomLevel + 0.5f);
            }
        }

        private Vector3? GetSpectatePosition(Spectatable obj)
        {
            if (obj.type == SpectatableType.Enemy || obj.type == SpectatableType.Masked || obj.type == SpectatableType.GhostGirl)
            {
                EnemyAI enemy = obj.enemyInstance;
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
                SpectatorList = FindObjectsByType<Spectatable>(FindObjectsSortMode.None).Where(x => GetSetting(SanitizeEnemyName(x.enemyName))).ToArray();
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
                        float closest = 999999f;
                        int index = 0;
                        for (int i = 0; i < SpectatorList.Length; i++)
                        {
                            float dist = (SpectatorList[i].transform.position - __instance.spectatedPlayerScript.transform.position).sqrMagnitude;
                            if (dist < closest * closest)
                            {
                                closest = dist;
                                index = i;
                            }
                        }
                        SpectatedEnemyIndex = index;
                    }
                }
                __instance.spectatedPlayerScript = null;
            }
            else
            {
                __instance.spectatedPlayerScript = __instance.playersManager.allPlayerScripts.FirstOrDefault(x => !x.isPlayerDead && x.isPlayerControlled);
                HUDManager.Instance.spectatingPlayerText.text = $"(Spectating: {__instance.spectatedPlayerScript.playerUsername})";
            }
        }

        public void PopulateSettings()
        {
            EnemyType[] allEnemies = Resources.FindObjectsOfTypeAll<EnemyType>();
            foreach (EnemyType type in allEnemies)
            {
                if (type.enemyName == "Red pill" || type.enemyName == "Lasso")
                    continue;
                string name = SanitizeEnemyName(type.enemyName);
                settings.TryAdd(name, Plugin.Configuration.Bind("Enemies", name, !type.isDaytimeEnemy, "Enables spectating " + name));
            }

            settings.TryAdd("Landmine", Plugin.Configuration.Bind("Enemies", "Landmine", false, "Enables spectating Landmines"));
            settings.TryAdd("Turret", Plugin.Configuration.Bind("Enemies", "Turret", false, "Enables spectating Turrets"));

            Debug.LogWarning("[SpectateEnemies]: Config loaded");
            AssertSettings();
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

        private string SanitizeEnemyName(string enemyName)
        {
            return enemyName.Replace("\\", "").Replace("\"", "").Replace("'", "").Replace("[", "").Replace("]", "");
        }

        private void GetNextValidSpectatable()
        {
            SpectatorList = FindObjectsByType<Spectatable>(FindObjectsSortMode.None).Where(x => GetSetting(SanitizeEnemyName(x.enemyName))).ToArray();
            int enemiesChecked = 0;
            int current = SpectatedEnemyIndex;
            while (enemiesChecked < SpectatorList.Length)
            {
                current++;
                if (current >= SpectatorList.Length)
                {
                    current = 0;
                }
                Spectatable enemy = SpectatorList.ElementAtOrDefault(current);
                if (enemy != null)
                {
                    if (enemy.type == SpectatableType.Enemy || enemy.type == SpectatableType.Masked || enemy.type == SpectatableType.GhostGirl)
                    {
                        if (enemy.enemyInstance != null && enemy.enemyInstance.isEnemyDead)
                        {
                            enemiesChecked++;
                            continue;
                        }
                    }
                    if (settings.ContainsKey(SanitizeEnemyName(enemy.enemyName)))
                    {
                        SpectatedEnemyIndex = current;
                        return;
                    }
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
                Debug.LogWarning($"{t.Key} : {t.Value.Value}");
            }
        }

        public bool GetSetting(string name)
        {
            if (settings.ContainsKey(name))
                return settings[name].Value;
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
                scrollPos = GUI.BeginScrollView(new Rect(5, 20, 490, 370), scrollPos, new Rect(5, 20, 490, 370), false, true);
                GUI.Label(new Rect(10, 30, 500, 100), "Tip: Checking the box next to an enemy name will enable spectating it.");
                int x = 0;
                int y = 0;
                foreach (string k in settings.Keys.ToList())
                {
                    settings[k].Value = GUI.Toggle(new Rect(10 + (150 * x), 60 + (30 * y), 150, 20), settings[k].Value, k);
                    x++;
                    if (x == 3)
                    {
                        x = 0;
                        y++;
                    }
                }
                GUI.EndScrollView(true);
            }
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
    }
}
