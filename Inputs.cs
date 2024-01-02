using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace SpectateEnemy
{
    internal class Inputs : LcInputActions
    {
        [InputAction("<Keyboard>/e", Name = "Swap between Players/Enemies")]
        public InputAction SwapKey { get; set; }

        [InputAction("<Keyboard>/insert", Name = "Open Config Menu")]
        public InputAction MenuKey { get; set; }

        [InputAction("<Mouse>/rightButton", Name = "Toggle Flashlight")]
        public InputAction FlashlightKey { get; set; }
    }
}
