using Data.Structs;
using UnityEngine;

namespace Data.Old
{
    
    public class xArcherEnemy : xEnemy
    {
        [Header("Bow Animation")] 
        public string animationTrigger;
        
        public override void OnInterval(xEnemyIntervalArgs args)
        {
            if (args.CanAttack)
            {
                args.Animator.SetTrigger(animationTrigger);
            }
        }
    }
}