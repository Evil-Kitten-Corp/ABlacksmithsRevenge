using System;
using Brains;
using Data.Structs;
using Interfaces;
using Placement;
using UnityEngine;
using UnityEngine.AI;
using Waves;
using Random = UnityEngine.Random;

namespace Data
{
    [RequireComponent(typeof(Animator))]
    public class EnemyBrain : MonoBehaviour, IDamageable
    {
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int DamageAnim = Animator.StringToHash("Damage");
        public float attackTimer { get; set; }
        public NavMeshAgent agent { get; private set; }
        
        public GameObject target { get; private set; }
        public Transform head;

        public Animator animator;

        public AudioSource audioSource;
        public AudioSource speechSource;
        public AudioSource walkingSource;

        [Header("Transforms")] 
        public Transform firePoint;

        private bool _pausedGame;

        private Enemy _enemySo;
        private GridManager _grid;
        private float _health;
        private bool _activated;

        [Header("Debug, do NOT modify")]
        public int coluna = 0;
        public int linha = 0;

        private bool _hasPath;

        public event Action OnDeath;

        private void Start()
        {
            OnDeath += () =>
            {
                speechSource.PlayOneShot(_enemySo.deathSounds[Random.Range(0, _enemySo.deathSounds.Length)]);
                animator.SetTrigger(Death);
            };
        }

        /// <summary>
        /// Call this method from the spawner.
        /// It sets up the enemy using its data, spawn position, and the GridManager.
        /// </summary>
        public void Activate(Enemy enemyData, Vector3 spawnPos)
        {
            _enemySo = enemyData;
            _health = _enemySo.health;
            _grid = FindAnyObjectByType<GridManager>();

            agent = GetComponent<NavMeshAgent>();

            if (agent == null)
                agent = gameObject.AddComponent<NavMeshAgent>();

            agent.speed = _enemySo.speed;
            agent.stoppingDistance = _enemySo.minRange;

            int spawnColumn;
            int spawnRow;

            if (_grid.invertSpawnPoints)
            {
                spawnColumn = _grid.spawnPositions.IndexOf(spawnPos);
                Debug.Log("COLUNA = " + (spawnColumn + 1));

                spawnRow = 0;
                Debug.Log("LINHA = 1");
            }
            else
            {
                spawnColumn = _grid.spawnPositions.IndexOf(spawnPos);
                Debug.Log("COLUNA = " + (spawnColumn + 1));

                spawnRow = _grid.gridWidth - 1;
                Debug.Log("LINHA = " + (spawnRow + 1));
            }

            coluna = spawnColumn;
            linha = spawnRow;
            
            Vector3 startCell = _grid.gridPositions[linha, coluna];
            
            GameData.Instance.Pause += () =>
            {
                _pausedGame = true;
                agent.isStopped = true;
            };
            
            GameData.Instance.Resume += () =>
            {
                _pausedGame = false;
                agent.isStopped = false;
            };
            
            _activated = true;
            agent.SetDestination(startCell);
        }

        public Vector3 GetCurrentCellPosition() => _grid.gridPositions[linha, coluna];

        private void Update()
        {
            if (!_activated)
                return;

            if (_pausedGame)
                return;

            if (_health <= 0)
            {
                OnDeath?.Invoke();
                Destroy(gameObject, 1f);
                return;
            }

            // Check if the enemy has reached its current destination.
            if (!agent.pathPending && IsAtEndOfPath())
            {
                _enemySo.OnCellReached(new EnemyArgs(_grid, this));
            }
            else
            {
                _enemySo.UpdateBehavior(new EnemyArgs(_grid, this));
            }
        }

        /// <summary>
        /// This is intended to be called from an Animation Event when the enemy’s attack animation should deal damage.
        /// </summary>
        public void AnimationAttackEvent()
        {
            _enemySo.DealDamage(new EnemyArgs(_grid, this));
        }

        /// <summary>
        /// Apply damage to this enemy.
        /// </summary>
        public void Damage(float damageAmount)
        {
            _health -= damageAmount;

            if (_health > 0)
            {
                speechSource.PlayOneShot(_enemySo.damageSounds[Random.Range(0, _enemySo.damageSounds.Length)]);
                animator.SetTrigger(DamageAnim);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (agent != null && agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(agent.destination, 1);
            }
        }

        public bool CheckNextPossibleLaneIndex()
        {
            if (_grid.invertSpawnPoints)
            {
                if (linha + 1 < _grid.gridWidth)
                { 
                    return true;
                }
            }
            else
            {
                if (linha - 1 >= 0)
                {
                    return true;
                }
            }
            
            Debug.Log("Apparently we're in the last lane...");
            
            FindAnyObjectByType<EnemyWaveSpawner>().Defeat();
            return false;
        }

        public void GoToNext()
        {
            if (_grid.invertSpawnPoints)
            {
                linha++;
            }
            else
            {
                linha--;
            }
            
            Vector3 targetPos = _grid.gridPositions[linha, coluna];
            agent.SetDestination(targetPos);
        }

        public void AcquireTarget(GameObject getTargetOnPosition)
        {
            if (getTargetOnPosition.TryGetComponent<IDamageable>(out _))
            {
                if (target != null)
                {
                    target.GetComponent<DefenseBrain>().OnDeath -= OnTargetDeath;
                }
                
                target = getTargetOnPosition;
                target.GetComponent<DefenseBrain>().OnDeath += OnTargetDeath;
            }
        }

        private void OnDestroy()
        {
            if (target != null)
            {
                target.GetComponent<DefenseBrain>().OnDeath -= OnTargetDeath;
            }
        }

        private void OnTargetDeath()
        {
            target = null;
            _enemySo.OnCellReached(new EnemyArgs(_grid, this));
        }
        
        private bool IsAtEndOfPath()
        {
            _hasPath |= agent.hasPath;
            
            if (_hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.01f)
            {
                //arrived
                _hasPath = false;
                return true;
            }

            return false;
        }

        public void OnFootstep()
        {
            walkingSource.PlayOneShot(_enemySo.footstepSounds[Random.Range(0, _enemySo.footstepSounds.Length)]);
        }
    }
}