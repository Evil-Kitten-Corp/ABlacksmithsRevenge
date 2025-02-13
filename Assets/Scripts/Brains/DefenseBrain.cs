using System;
using Data;
using Data.Structs;
using Effects;
using Interfaces;
using Placement;
using UnityEngine;

namespace Brains
{
    [RequireComponent(typeof(AudioSource), typeof(Collider))]
    public class DefenseBrain : MonoBehaviour, IDamageable
    {
        public Animator animator;
        public Transform firePoint;
        public AudioSource audioSource;
        
        [Header("Debug Only")] 
        [SerializeField] private Defense defenseType;
        [SerializeField] private float currentHealth;
        
        private DefenseVisualState _defenseVisualState;
        private DefenseAudioState _defenseAudioState;

        private bool _paused;
        private Renderer[] _modelRenderers;

        public float fireCooldown { get; set; }

        public float generalTimer { get; set; }

        public bool generalActive { get; set; }
        
        public bool exploding {get; set;}

        public Renderer[] modelRenderers => _modelRenderers;

        private GridManager _grid;

        public Vector3 intendedPos { get; private set; }
        
        public event Action OnDeath;

        private void Start()
        {
            _defenseVisualState = GetComponent<DefenseVisualState>();
            _defenseAudioState = GetComponent<DefenseAudioState>();
            _modelRenderers = GetComponentsInChildren<Renderer>();
            _grid = FindAnyObjectByType<GridManager>();
            
            GameData.Instance.Pause += () => _paused = true;
            GameData.Instance.Resume += () => _paused = false;

            if (defenseType != null)
            {
                AssignDefense(defenseType, transform.position);
            }
        }

        public void AssignDefense(Defense def, Vector3 pos)
        {
            defenseType = def;
            currentHealth = defenseType.health;
            intendedPos = pos;
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
                    TryDestroy();
                }
                
                defenseType.Interval(new DefenseArgs(this));
            }
        }

        public void TryDestroy(float f = 1f)
        {
            OnDeath?.Invoke();
            
            _grid.ClearOccupied(intendedPos);

            defenseType.OnDeath(new DefenseArgs(this));
            
            defenseType = null;
            Debug.Log("Calling destroy");
            Destroy(gameObject, f);
        }

        public void ResetFireCooldown()
        {
            defenseType.ResetCooldown(new DefenseArgs(this));
        }
        
        public void Damage(float damage)
        {
            currentHealth -= damage;
            UpdateVisualState();
        }
        
        public void Repair(float amount) 
        {
            float oldHealth = currentHealth;
            currentHealth = Mathf.Min(currentHealth + amount, defenseType.health);
            
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
            if (generalActive && other.CompareTag("Enemy") && !exploding)
            {
                defenseType.Special(new DefenseArgs(this));
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (generalActive && other.CompareTag("Enemy") && !exploding)
            {
                defenseType.Special(new DefenseArgs(this));
            }
        }

        private void OnDrawGizmos()
        {
            if (defenseType is Turret turret)
            {
                Gizmos.color = Color.red;
                
                if (Application.isPlaying)
                    Gizmos.DrawWireCube(transform.position + turret.originOffset, turret.halfExtents * 2);
            }

            if (defenseType is Trap trap)
            {
                Gizmos.color = Color.yellow;
                
                if (Application.isPlaying)
                    Gizmos.DrawWireSphere(transform.position, trap.explosionRadius);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void FireSpecialVfx(GameObject explosionPrefab)
        {
            var exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(exp, 2.5f);
        }
    }
}