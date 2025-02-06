using Data.Structs;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemies/Archer", order = 0)]
    public class ArcherEnemy : Enemy
    {
        [Header("Bow Animation")] 
        public string animationTrigger;
        
        public override void OnInterval(EnemyIntervalArgs args)
        {
            if (args.CanAttack)
            {
                args.Animator.SetTrigger(animationTrigger);
            }
        }
    }
}