using System;
using Data;
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

        private void Start()
        {
            _defenseVisualState = GetComponent<DefenseVisualState>();
            _defenseAudioState = GetComponent<DefenseAudioState>();
        }

        public void AssignDefense(Defense def)
        {
            defenseType = def;
            currentHealth = defenseType.health;
            defenseType.ActivateBrain();
            defenseState = DefenseState.Pristine;
        }

        private void Update()
        {
            if (defenseType != null)
            {
                _fireCooldown -= Time.deltaTime;

                if (_fireCooldown <= 0)
                {
                    defenseType.OnInterval(transform, firePoint);
                    _fireCooldown = 1 / defenseType.fireRate;
                }
                
                if (defenseState == DefenseState.Destroyed)
                {
                    Destroy();
                }
            }
        }

        private void Destroy()
        {
            defenseType.DeactivateBrain();
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

        private void OnDrawGizmosSelected()
        {
            if (defenseType)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, defenseType.range);
            }
        }
    }
}