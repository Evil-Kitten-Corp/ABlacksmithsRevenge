using Data.Structs;
using Game_Systems;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ManaGenerator", menuName = "Defense/Mana Generator", order = 1)]
    public class ManaGenerator : Defense
    {
        [Header("Mana Settings")]
        public float manaPerInterval = 5f;
        public float interval = 5f;

        public override void Special(DefenseIntervalArgs args)
        {
            ManaManager.instance.AddMana(manaPerInterval);
        }
    }
}