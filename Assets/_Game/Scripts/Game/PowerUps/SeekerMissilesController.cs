using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightItUp.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LightItUp.Game.PowerUps
{
    public class SeekerMissilesController : MonoBehaviour
    {
        private PowerUpService _powerUpService;
        private PowerUpConfiguration _seekerMissileConfig;
        private GameLevel _gameLevel;
        private List<SeekerMissile> _missiles = new List<SeekerMissile>();
        public void Init(PowerUpService powerUpService, GameManager gameManager, PowerUpConfiguration seekerMissileConfig) {
           _powerUpService = powerUpService;
           _gameLevel = gameManager.currentLevel;
              _seekerMissileConfig = seekerMissileConfig;
        }

        /// <summary>
        /// Get the closest unlit blocks to the player
        /// Priority is given to standard blocks
        /// </summary>
        /// <returns></returns>
        private List<BlockController> GetClosestUnlitBlocks()
        {
            var playerPosition = _gameLevel.player.transform.position;
            var unlitBlocks = _gameLevel.blocks
                .Where(block => !block.IsLit)
                .OrderBy(block => (block.transform.position - playerPosition).sqrMagnitude)
                .ToList();

            // Partition the unlit blocks into standard and non-standard blocks
            var partitionedBlocks = unlitBlocks.AsParallel()
                .GroupBy(block => block.IsStandardBlock());

            var standardBlocks = partitionedBlocks.FirstOrDefault(group => group.Key == true)?.ToList() ??
                                 new List<BlockController>();
            var nonStandardBlocks = partitionedBlocks.FirstOrDefault(group => group.Key == false)?.ToList() ??
                                    new List<BlockController>();

            // Concatenate the non-standard blocks to the end of the standard blocks list
            standardBlocks.AddRange(nonStandardBlocks);

            return standardBlocks;
        }
        
        
        /// <summary>
        /// Launch and initialize the missiles
        /// A list of target is given to each missile
        /// Shuffles the target list by moving the first element
        /// to the last position after firing a missile
        /// </summary>
        public void LaunchMissiles()
        {
            var targetList = GetClosestUnlitBlocks();
            var spawnPosition = _gameLevel.player.transform.position;
            spawnPosition.y += .75f;
    
            for (var i = 0; i < _seekerMissileConfig.missilesCount; i++)
            {
                var missile = ObjectPool.GetSeekerMissile();
                spawnPosition.x += Random.Range(-2f, 2f);
                missile.transform.position = spawnPosition;
                missile.Init(
                    this, 
                    targetList, 
                    _gameLevel.blocks, 
                    _seekerMissileConfig.missileSpeed,
                    _seekerMissileConfig.avoidObstacles
                    );
                
                _missiles.Add(missile);
                _gameLevel.player.camFocus.AddMissileTarget(missile);
        
                // Reorder the targetList: move the first element to the last position
                if (targetList.Count <= 0) continue;
                
                var firstTarget = targetList[0];
                targetList.RemoveAt(0); 
                targetList.Add(firstTarget);
            }
        }
        
        public void OnMissileExploded(SeekerMissile missile, bool returned)
        {
            _missiles.Remove(missile);
            _gameLevel.player.camFocus.RemoveMissileTarget(missile);
            if(returned) return;
            ObjectPool.ReturnSeekerMissile(missile);
        }
    }
}
