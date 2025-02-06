using Data;
using Interfaces;
using Placement;
using UnityEngine;

namespace Brains
{
    public class EnemyBrain : MonoBehaviour, IDamageable
    {
        private float _currentHealth;
        private Vector3 _target;
        private Enemy _data;
        
        public event System.Action OnDeath;

        private void Die()
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        public void Activate(Enemy en, Vector3 spawnPos)
        {
            _data = en;
            _currentHealth = _data.health;
            
            GridManager grid = FindAnyObjectByType<GridManager>();
            int rowIndex = grid.spawnPositions.IndexOf(spawnPos);

            if (rowIndex != -1)
            {
                _target = grid.gridPositions[0, rowIndex]; 
            }
        }

        private void Update()
        {
            if (_target != Vector3.zero)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target, 
                    _data.speed * Time.deltaTime);
            }
        }

        public void Damage(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }
}