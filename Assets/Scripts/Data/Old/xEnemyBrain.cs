using System;
using Data.Structs;
using Interfaces;
using Placement;
using UnityEngine;
using UnityEngine.AI;
using Waves;

namespace Data.Old
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class xEnemyBrain : MonoBehaviour, IDamageable
    {
        #region Events

        public event Action OnDeath;
        public event Action<IDamageable> OnTargetAcquired;
        public event Action OnTargetLost;

        #endregion
        
        private float _currentHealth;
        private Vector3 _target;
        private xEnemy _data;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        
        private bool _hasPath;
        private GridManager _grid;
        private bool _forceAttack;

        private int _positionIndex;
        private int _col;
        
        private float _attackCooldown;
        private IDamageable _victim;

        private const float PathEndThreshold = 0.1f;
        
        private void Start()
        {
            OnTargetAcquired += t =>
            {
                _navMeshAgent.isStopped = true;
                _target = transform.position;
                _forceAttack = true;
                t.OnDeath += () => OnTargetLost?.Invoke();
            };
            
            OnTargetLost += () =>
            {
                _navMeshAgent.isStopped = false;
                _target = _grid.GetClosestGridPosition(transform.position);
                _navMeshAgent.SetDestination(_target);
                _victim = null;
                _forceAttack = false;
            };
            
            _grid = FindAnyObjectByType<GridManager>();
            
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Die()
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        public void Activate(xEnemy en, Vector3 spawnPos)
        {
            _data = en;
            _currentHealth = _data.health;
            _navMeshAgent.speed = _data.speed;
            
            // GridManager grid = FindAnyObjectByType<GridManager>();
            // int rowIndex = grid.spawnPositions.IndexOf(spawnPos);
            //
            // if (rowIndex != -1)
            // {
            //     _target = grid.gridPositions[0, rowIndex]; 
            // }
            
            //we start by wanting to go to the last cell of this column
            _positionIndex = _grid.gridWidth; 
            _col = _grid.spawnPositions.IndexOf(spawnPos);

            if (_col != -1)
            {
                _target = _grid.gridPositions[_positionIndex, _col];
            }
        }

        private void Update()
        {
            //are we even moving?
            if (_navMeshAgent.hasPath)
            {
                //this will probably have to be changed because
                //the archer enemy for example just finds the farthest
                //damageable in the column and attacks it, and only walks
                //forward when there are no more enemies to attack
                        
                //melee enemy > walks from cell to cell checking if there are enemies in between
                //archer enemy > stays still shooting at the farthest enemy, only walks when no target is found
                //leaper enemy > same as melee but instead of attacking walls, leaps over them
                //mage enemy > same as archer but the attacks are aoe so they affect in a cross in the grid (the
                //main target takes full dmg and the units in the cells front back left or right of the target
                //take half the damage)
                //tank enemy > same as melee but has more health
                
                if (IsAtEndOfPath()) //if we have arrived at target place
                {
                    if (_grid.IsPositionOccupied(_target)) //check if occupied
                    {
                        //if yes attack
                        IDamageable target = _grid.GetTargetOnPosition(_target).GetComponent<IDamageable>();

                        if (target != null)
                        {
                            _victim = target;
                            OnTargetAcquired?.Invoke(target);
                        }
                    }
                    else
                    {
                        //move to next cell on the column
                        _positionIndex--;
                        
                        if (_positionIndex < 0)
                        {
                            _target = _grid.gridPositions[_positionIndex, _col];
                        }
                        else
                        {
                            GameData.Instance.OnGameOver();
                        }
                    }
                }
            }
            //or are we fighting?
            else if (_forceAttack)
            {
                _data.OnInterval(new xEnemyIntervalArgs(_animator, _attackCooldown <= 0));

                if (_attackCooldown <= 0)
                {
                    _attackCooldown = 1 / _data.attacksPerSecond;
                }
            }
            //if not, let's move
            else if (!_forceAttack)
            {
                //do we have a target?
                if (_target != transform.position)
                {
                    _navMeshAgent.SetDestination(_target);
                }
            }
            
            _attackCooldown -= Time.deltaTime;
        }
        
        private bool IsAtEndOfPath()
        {
            _hasPath |= _navMeshAgent.hasPath;
            if (_hasPath && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + PathEndThreshold)
            {
                //arrived
                _hasPath = false;
                return true;
            }

            return false;
        }

        public void Damage(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void AnimationMeleeHelper()
        {
            if (_forceAttack && _victim != null)
            {
                _victim.Damage(_data.damage);
            }
        }
    }
}