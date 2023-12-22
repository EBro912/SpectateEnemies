using UnityEngine;

namespace SpectateEnemy
{
    internal class Spectatable : MonoBehaviour
    {
        public SpectatableType type = SpectatableType.Enemy;
        public string enemyName = "Enemy";
    }

    internal enum SpectatableType
    {
        Enemy,
        Turret,
        Landmine
    }
}
