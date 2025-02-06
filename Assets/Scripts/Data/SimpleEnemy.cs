using Data.Structs;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemies/Simple", order = 0)]
    public class SimpleEnemy : Enemy
    {
        [Header("Sword Animation")] 
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