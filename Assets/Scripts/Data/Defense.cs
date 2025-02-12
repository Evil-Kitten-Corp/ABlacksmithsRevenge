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
        
        public virtual void Interval(DefenseArgs args) {}

        public virtual void Special(DefenseArgs args) {}

        public virtual void OnDeath(DefenseArgs args) {}

        public virtual void ResetCooldown(DefenseArgs args) {}
    }
}