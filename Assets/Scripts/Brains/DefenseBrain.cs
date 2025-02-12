using System;
using Data;
using Data.Structs;
using Effects;
using Interfaces;
using UnityEngine;

namespace Brains
{
    [RequireComponent(typeof(AudioSource), typeof(Collider))]
    public class DefenseBrain : MonoBehaviour, IDamageable
    {
        public Animator animator;
        public Transform firePoint;
        private Renderer[] _modelRenderers;
        public AudioSource audioSource;
        
        [Header("Debug Only")] 
        [SerializeField] private Defense defenseType;
        [SerializeField] private DefenseState defenseState;
        [SerializeField] private float currentHealth;
        
        private DefenseVisualState _defenseVisualState;
        private DefenseAudioState _defenseAudioState;
        
        private float _fireCooldown;
        
        private float _generalTimer;
        private bool _generalActive;
        
        private bool _paused;
        
        public event Action OnDeath;

        private void Start()
        {
            _defenseVisualState = GetComponent<DefenseVisualState>();
            _defenseAudioState = GetComponent<DefenseAudioState>();
            
            GameData.Instance.Pause += () => _paused = true;
            GameData.Instance.Resume += () => _paused = false;
            
            _modelRenderers = GetComponentsInChildren<Renderer>();
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
                if (currentHealth <= 0)
                {
                    Destroy();
                }

                if (defenseType is Turret turret)
                {
                    _fireCooldown -= Time.deltaTime;

                    if (_fireCooldown <= 0)
                    {
                        turret.OnInterval(new DefenseIntervalArgs(transform, firePoint, this));
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

                if (defenseType is Trap trap && !_generalActive)
                {
                    _generalTimer += Time.deltaTime;

                    if (_generalTimer >= trap.timeToActivate)
                    {
                        if (_modelRenderers[0].material.color != Color.white)
                        {
                            foreach (var m in _modelRenderers)
                            {
                                m.material.color = Color.white;
                            }
                        }
                        
                        PlaySound(trap.onReadySound);
                        
                        _generalActive = true;
                    }
                    else
                    {
                        if (_modelRenderers[0].material.color != Color.red)
                        {
                            foreach (var m in _modelRenderers)
                            {
                                m.material.color = Color.red;
                            }
                        }
                    }
                }
            }
        }

        private void Destroy()
        {
            OnDeath?.Invoke();

            if (defenseType is Wall wall)
            {
                wall.PlayOnDestroySound(this);
            }
            
            defenseType = null;
            Debug.Log("Calling destroy");
            Destroy(gameObject, 1f);
        }

        public void ResetFireCooldown()
        {
            if (defenseType is Turret turret)
            {
                _fireCooldown = 1 / turret.fireRate;
            }
        }
        
        public void Damage(float damage)
        {
            currentHealth -= damage;

            if (currentHealth <= defenseType.health * .5f)
            {
                defenseState = DefenseState.Damaged;
            }
            else if (currentHealth <= 0)
            {
                Destroy();
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
                if (_defenseAudioState != null)
                {
                    _defenseAudioState.PlayRepairSound();
                }
            }
            
            UpdateVisualState();
        }
        
        void UpdateVisualState()
        {
            float healthPercentage = currentHealth / defenseType.health;

            if (_defenseVisualState != null)
            {
                _defenseVisualState.UpdateVisualState(healthPercentage);
            }
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

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void FireSpecialVfx(GameObject explosionPrefab)
        {
            var exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 2f);
        }
    }
}