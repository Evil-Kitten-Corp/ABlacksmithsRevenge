﻿using System;
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
        public float attackTimer { get; set; }
        public NavMeshAgent agent { get; private set; }
        
        public GameObject Target { get; set; }

        public AudioSource audioSource;

        [Header("Transforms")] 
        public Transform firePoint;

        private bool _pausedGame;
        
        //TBD
        public int laneIndex { get; private set; }
        public int currentColumn { get; internal set; }

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
                audioSource.PlayOneShot(_enemySo.deathSounds[Random.Range(0, _enemySo.deathSounds.Length)]);
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
            
            currentColumn = spawnColumn;
            laneIndex = spawnRow;
            
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
                audioSource.PlayOneShot(_enemySo.damageSounds[Random.Range(0, _enemySo.damageSounds.Length)]);
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
            
            GameData.Instance.OnGameOver();
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
                Target = getTargetOnPosition;
            }
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
            audioSource.PlayOneShot(_enemySo.footstepSounds[Random.Range(0, _enemySo.footstepSounds.Length)]);
        }
    }
}