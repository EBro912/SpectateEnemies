using UnityEngine;

namespace SpectateEnemy
{
    public class SpectateEnemiesAPI
    {
        /// <summary>
        /// Returns true if Spectate Enemies is loaded and ready
        /// </summary>
        public static bool IsLoaded => SpectateEnemies.Instance != null;

        /// <summary>
        /// Returns true if the player is currently spectating an enemy
        /// </summary>
        public static bool IsSpectatingEnemies => SpectateEnemies.Instance.SpectatingEnemies;

        /// <summary>
        /// Gets the <see cref="GameObject"></see> of the enemy that the player is currently spectating.
        /// </summary>
        /// <returns>The parent <see cref="GameObject"/> of the enemy if the player is spectating an enemy, otherwise null</returns>
        public static GameObject CurrentEnemySpectating()
        {
            if (IsSpectatingEnemies && SpectateEnemies.Instance.SpectatedEnemyIndex > -1)
            {
                return SpectateEnemies.Instance.SpectatorList[SpectateEnemies.Instance.SpectatedEnemyIndex].gameObject;
            }
            return null;
        }
    }
}
