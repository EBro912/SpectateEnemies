using UnityEngine;

namespace SpectateEnemy
{
    public class Spectatable : MonoBehaviour
    {
        public SpectatableType type = SpectatableType.Enemy;
        public string enemyName = "Enemy";
    }

    public enum SpectatableType
    {
        Enemy,
        Turret,
        Landmine
    }
}
