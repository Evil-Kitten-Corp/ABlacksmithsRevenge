using UnityEngine;

namespace Data
{
    public abstract class Defense : ScriptableObject
    {
        public GameObject prefab;
        
        [Header("Stats")]
        public float health;
        public float manaCost = 50f;

        [Header("Buying")] 
        public bool unlocked;

        public virtual void ActivateBrain() {}
        public virtual void DeactivateBrain() {}
        public virtual void OnInterval(Transform transform, Transform firePoint) {}
    }
}