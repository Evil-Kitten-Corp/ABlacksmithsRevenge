using UnityEngine;

namespace Data
{
    public abstract class Defense : ScriptableObject
    {
        public GameObject prefab;
        
        [Header("Stats")]
        public float health;
        public float damage;
        public float range = 5f; 
        public float fireRate = 1f; 

        public abstract void ActivateBrain();
        public abstract void DeactivateBrain();
        public abstract void OnInterval(Transform transform, Transform firePoint);
    }
}