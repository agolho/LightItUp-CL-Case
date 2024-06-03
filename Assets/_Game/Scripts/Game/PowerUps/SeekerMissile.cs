using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightItUp.Data;
using UnityEngine;

namespace LightItUp.Game.PowerUps
{
    public class SeekerMissile : PooledObject
    {
        private SeekerMissilesController _seekerMissilesController;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private LayerMask _blockLayer;
        [SerializeField] private LayerMask _blockLayerLit;
        private bool _hasTarget;
        private bool _avoidObstacles;
        private List<BlockController> _targets;
        private List<BlockController> _allBlocks;
        private BlockController _target;
        private Collider2D _targetCollider;
        private float _speed;
        private bool _returned;

        private BlockController.ShapeType _shapeType;
        private float _circleRadius;
        private Vector3 _blockSize;

        private Vector3 _previousPosition;
        private int _stuckFrames;
        private float _currentSafetyDistance;


        private const int StuckFrameThreshold = 10;
        private const float MovementThreshold = 0.01f;


        public void Init(SeekerMissilesController seekerMissilesController, List<BlockController> targets, List<BlockController> allBlocks, float speed, bool avoidObstacles)
        {
            SetTargets(targets, allBlocks);
            SetSpeed(speed);
            SetObstacleAvoidance(avoidObstacles);
            _seekerMissilesController = seekerMissilesController;
            _trailRenderer.Clear();
        }

        private void SetTargets(List<BlockController> targets, List<BlockController> allBlocks)
        {
            _targets = targets;
            _allBlocks = allBlocks;
            _hasTarget = true;
            _currentSafetyDistance = DefaultSafetyDistance;
            PickTarget();
        }

        private void SetSpeed(float speed)
        {
            _speed = speed;
        }
        
        private void SetObstacleAvoidance(bool avoidObstacles)
        {
            _avoidObstacles = avoidObstacles;
        }

        /// <summary>
        /// Picking the first target from the list of targets
        /// The list is optimized in the class SeekerMissilesController
        /// </summary>
        private void PickTarget()
        {
            if (_targets.Count <= 0)
            {
                ExplodeMissile();
                return;
            }
            _target = _targets[0];
            _targetCollider = _target.GetComponent<Collider2D>();
            PaintToTargetColor();
        }

        
        /// <summary>
        /// If the target is lit, remove it from the list of targets
        /// Needed for if the target is lit by something else
        /// </summary>
        private void CheckTarget()
        {
            if (_targets.Count == 0)
            {
                ExplodeMissile();
                return;
            }

            if (!_targets[0].IsLit) return;

            _targets.RemoveAt(0);
            PickTarget();
        }

        private void PaintToTargetColor()
        {
            if (_targets.Count == 0) return;
            _spriteRenderer.color = _targets[0].color;
            _trailRenderer.startColor = _targets[0].color;
            _trailRenderer.endColor = _targets[0].color;
        }

        private void Update()
        {
            if (!_hasTarget) return;
            CheckTarget();
            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            if (!_target)
            {
                ExplodeMissile();
                return;
            }

            var currentPosition = transform.position;
            var directionToTarget = CalculateDirectionToTarget(currentPosition, _target);

            if (directionToTarget == Vector3.zero)
            {
                _currentSafetyDistance = Mathf.Min(_currentSafetyDistance + SafetyDistanceReduction, DefaultSafetyDistance * 2);
                return;
            }

            var avoidanceDirection = CalculateAvoidanceDirection(currentPosition, directionToTarget);
            MoveMissile(currentPosition, avoidanceDirection);
            HandleStuckState();
        }

        private Vector3 CalculateDirectionToTarget(Vector3 currentPosition, BlockController target)
        {
            var targetPosition = target.transform.position;
            
            if (_targetCollider != null)
            {
                // Get the closest point on the target's collider to the missile's current position
                targetPosition = _targetCollider.ClosestPoint(currentPosition);
            }

            var direction = (targetPosition - currentPosition).normalized;

            // If the missile is very close to the target, make sure it doesn't orbit by reducing lateral adjustments
            if (Vector3.Distance(currentPosition, targetPosition) < 1.0f)
            {
                direction = (targetPosition - currentPosition).normalized;
            }

            return direction;
        }
        

        
        private const float DefaultSafetyDistance = 1.2f;
        private const float MinimumSafetyDistance = 0.01f;
        private const float SafetyDistanceReduction = 0.2f;
        private const float RaycastDistance = 1.0f;
        private const int RayCount = 8;
        private const float pullInDistance = 8.5f;
        
        /// <summary>
        /// Obstacle avoidance
        /// Casts rays in a circle around the missile to detect obstacles
        /// If the raycast hits an obstacle, the missile will move in the direction of the first ray that doesn't hit an obstacle
        /// If the raycasts hits the target the missile will move directly towards the target
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="directionToTarget"></param>
        /// <returns></returns>
        private Vector3 CalculateAvoidanceDirection(Vector3 currentPosition, Vector3 directionToTarget)
        {
            if(!_avoidObstacles) return directionToTarget;
            var bestDirection = directionToTarget;
            
            for (var i = 0; i < RayCount; i++)
            {
                var angle = i * (360f / RayCount);
                Vector3 testDirection = Quaternion.Euler(0, 0, angle) * directionToTarget;
                var hit = Physics2D.Raycast(currentPosition, testDirection, RaycastDistance, _blockLayer | _blockLayerLit);

                if (hit.collider != null) continue;
                // if the raycast hit the target dont change direction
                if (hit.collider == _targetCollider)
                {
                    return directionToTarget;
                }
                return testDirection; // No obstacle in this direction
            }

            return bestDirection;
        }

        private void MoveMissile(Vector3 currentPosition, Vector3 directionToTarget)
        {
            // Check distance to target and move directly if close enough
            var closestPointOfTarget = _targetCollider.ClosestPoint(currentPosition);
            if (Vector3.Distance(currentPosition, closestPointOfTarget) < pullInDistance)
            {
                transform.position = Vector3.MoveTowards(currentPosition, closestPointOfTarget, _speed * Time.deltaTime);
            }
            else
            {
                const float offsetDistance = pullInDistance + 3f;
                Vector3 offsetPosition = currentPosition + directionToTarget + (directionToTarget.normalized * offsetDistance);
                transform.position = Vector3.MoveTowards(currentPosition, offsetPosition, _speed * Time.deltaTime);
            }
        }

        
        /// <summary>
        /// In the event that the missile is stuck, reduce the safety distance
        /// Safety distance is how close can the missile get to any block
        /// </summary>
        private void HandleStuckState()
        {
            if (Vector3.Distance(_previousPosition, transform.position) < MovementThreshold)
            {
                _stuckFrames++;
                if (_stuckFrames > StuckFrameThreshold)
                {
                    _currentSafetyDistance = Mathf.Max(_currentSafetyDistance - SafetyDistanceReduction, MinimumSafetyDistance);
                    _stuckFrames = 0;
                }
            }
            else
            {
                _stuckFrames = 0;
                _currentSafetyDistance = DefaultSafetyDistance;
            }

            _previousPosition = transform.position;
        }

        public void ExplodeMissile()
        {
            _targets = null;
            _target = null;
            _hasTarget = false;
            _seekerMissilesController.OnMissileExploded(this, _returned);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(other.gameObject == _target.gameObject)
            {
                _target.Collide(true);
                StartCoroutine(WaitAndExecute(0.1f, ExplodeMissile));
            }
            else
            {
                Physics2D.IgnoreCollision(_collider2D, other.collider);
            }
        }

        private IEnumerator WaitAndExecute(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        public override void OnReturnedPoolObj()
        {
            base.OnReturnedPoolObj();
            _returned = true;
        }

        public override void OnInitPoolObj()
        {
            base.OnInitPoolObj();
            _returned = false;
        }

    }
}
