using Data.Structs;
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
        public Sprite icon;

        public virtual void ActivateBrain() {}
        public virtual void DeactivateBrain() {}
        public virtual void OnInterval(DefenseIntervalArgs args) {}
    }
}