using Data.Structs;
using UnityEngine;

namespace Data
{
    public abstract class Enemy : ScriptableObject
    {
        [Header("Prefab & Stats")]
        public GameObject prefab;
        public float health = 100f;
        public float speed = 3f;
        public int damage = 20;
        public float attackCooldown = 1f;
        
        /// <summary>
        /// Called each frame so the behavior can check for targets, manage cooldowns, etc.
        /// </summary>
        public abstract void UpdateBehavior(EnemyArgs args);

        public abstract void OnCellReached(EnemyArgs args);
    
        /// <summary>
        /// Called by an animation event when the enemy’s attack animation triggers.
        /// </summary>
        public abstract void DealDamage(EnemyArgs args);
    }
}