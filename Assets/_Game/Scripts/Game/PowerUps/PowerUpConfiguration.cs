
using UnityEngine;

namespace LightItUp.Game.PowerUps

{
    [CreateAssetMenu(fileName = "PowerUpData", menuName = "[HyperCasual]/PowerUpData")]
    public class PowerUpConfiguration : ScriptableObject
    {
        public PowerUpType powerUpType;
        [Tooltip("Missiles count per each use, 0 means no missiles will be shot")]
        public int missilesCount = 3;
        public float missileSpeed = 10f;
        [Tooltip("Use limit per level. 0 means feature is disabled.")]
        public int powerUpUseLimit = 1;
        [Tooltip("This feature is beta and may not work as expected.")]
        public bool avoidObstacles;
    }
}