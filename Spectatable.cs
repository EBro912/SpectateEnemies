using UnityEngine;

namespace SpectateEnemy
{
    internal class Spectatable : MonoBehaviour
    {
        public SpectatableType type = SpectatableType.Enemy;
        public string enemyName = "Enemy";
        public string maskedName = string.Empty;
    }

    internal enum SpectatableType
    {
        Enemy,
        Turret,
        Landmine,
        Masked
    }
}
