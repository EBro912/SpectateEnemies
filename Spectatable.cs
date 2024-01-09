using System;
using UnityEngine;

namespace SpectateEnemy
{
    internal class Spectatable : MonoBehaviour
    {
        public SpectatableType type = SpectatableType.Enemy;
        public string enemyName = "Enemy";
        public string maskedName = string.Empty;
        public EnemyAI enemyInstance = null;
    }

    internal enum SpectatableType
    {
        Enemy,
        Turret,
        Landmine,
        Masked
    }
}
