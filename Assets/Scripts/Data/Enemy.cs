using Data.Structs;
using UnityEngine;

namespace Data
{
    public abstract class Enemy : ScriptableObject
    {
        public GameObject prefab;

        public float health;
        public float speed;

        public float damage;
        public float range;
        [Range(0.1f, 3f)] public float attacksPerSecond;

        public virtual void OnInterval(EnemyIntervalArgs args)
        { }
    }
}