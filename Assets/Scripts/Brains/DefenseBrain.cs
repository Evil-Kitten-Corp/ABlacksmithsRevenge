using System;
using Data;
using Data.Structs;
using Effects;
using Interfaces;
using UnityEngine;

namespace Brains
{
    public class DefenseBrain : MonoBehaviour, IDamageable
    {
        [Header("Debug Only")] 
        [SerializeField] private Defense defenseType;
        [SerializeField] private DefenseState defenseState;
        [SerializeField] private float currentHealth;
        
        private DefenseVisualState _defenseVisualState;
        private DefenseAudioState _defenseAudioState;
        
        public Transform firePoint;
        private float _fireCooldown;
        
        private float _generalTimer;
        private bool _generalActive;
        
        private Renderer _renderer;
        
        private bool _paused;
        
        public event Action OnDeath;

        private void Start()
        {
            _defenseVisualState = GetComponent<DefenseVisualState>();
            _defenseAudioState = GetComponent<DefenseAudioState>();
            _renderer = GetComponent<Renderer>();
            
            GameData.Instance.Pause += () => _paused = true;
            GameData.Instance.Resume += () => _paused = false;
        }

        public void AssignDefense(Defense def)
        {
            defenseType = def;
            currentHealth = defenseType.health;
            defenseState = DefenseState.Pristine;
        }
        
        public Defense GetDefenseType() => defenseType;

        private void Update()
        {
            if (_paused)
                return;
            
            if (defenseType != null)
            {
                if (defenseType is Turret turret)
                {
                    _fireCooldown -= Time.deltaTime;

                    if (_fireCooldown <= 0)
                    {
                        turret.OnInterval(new DefenseIntervalArgs(transform, firePoint, this));
                        _fireCooldown = 1 / turret.fireRate;
                    }
                }
                
                if (defenseType is ManaGenerator manaGenerator)
                {
                    _generalTimer += Time.deltaTime;

                    if (_generalTimer >= manaGenerator.interval)
                    {
                        defenseType.Special(new DefenseIntervalArgs(transform, firePoint, this));
                        _generalTimer = 0f;
                    }
                }

                if (defenseType is Trap trap)
                {
                    _generalTimer += Time.deltaTime;

                    if (_generalTimer >= trap.timeToActivate)
                    {
                        _generalActive = true;

                        if (_renderer.material.color != Color.white)
                        {
                            _renderer.material.color = Color.white;
                        }
                    }
                    else
                    {
                        if (_renderer.material.color != Color.red)
                        {
                            _renderer.material.color = Color.red;
                        }
                    }
                }
                
                if (defenseState == DefenseState.Destroyed)
                {
                    Destroy();
                }
            }
        }

        private void Destroy()
        {
            OnDeath?.Invoke();
            defenseType = null;
            Destroy(gameObject);
        }

        public void Damage(float damage)
        {
            currentHealth -= damage;

            if (currentHealth <= defenseType.health * .5f)
            {
                defenseState = DefenseState.Damaged;
            }
            else if (currentHealth == 0)
            {
                defenseState = DefenseState.Destroyed;
            }
            
            UpdateVisualState();
        }
        
        public void Repair(float amount) 
        {
            float oldHealth = currentHealth;
            currentHealth = Mathf.Min(currentHealth + amount, defenseType.health);
            
            if (currentHealth <= defenseType.health * .5f)
            {
                defenseState = DefenseState.Damaged;
            }
            else if (currentHealth > defenseType.health * .5f)
            {
                defenseState = DefenseState.Pristine;
            }
            
            if (currentHealth > oldHealth)
            {
                _defenseAudioState.PlayRepairSound();
            }
            
            UpdateVisualState();
        }
        
        void UpdateVisualState()
        {
            float healthPercentage = currentHealth / defenseType.health;

            _defenseVisualState.UpdateVisualState(healthPercentage);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_generalActive && other.CompareTag("Enemy"))
            {
                defenseType.Special(new DefenseIntervalArgs(transform, firePoint, this));
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (defenseType is Turret turret)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, turret.range);
            }
        }

        public void FireSpecialVfx(GameObject explosionPrefab)
        {
            var exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 2f);
        }
    }
}