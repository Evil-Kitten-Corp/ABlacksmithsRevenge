using UnityEngine;

namespace Data.Old
{
    
    public class xSimpleEnemy : xEnemy
    {
        [Header("Sword Animation")] 
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