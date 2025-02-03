using UnityEngine;

namespace Data
{
    public abstract class Enemy : ScriptableObject
    {
        public GameObject prefab;

        public float health;
        public float speed;
    }
}