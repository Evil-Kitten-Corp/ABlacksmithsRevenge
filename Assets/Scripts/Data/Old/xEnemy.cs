using Data.Structs;
using UnityEngine;

namespace Data.Old
{
    public abstract class xEnemy : ScriptableObject
    {
        public GameObject prefab;

        public float health;
        public float speed;

        public float damage;
        public float range;
        [Range(0.1f, 3f)] public float attacksPerSecond;

        public virtual void OnInterval(xEnemyIntervalArgs args)
        { }
    }
}