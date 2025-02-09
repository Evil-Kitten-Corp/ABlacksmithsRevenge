using UnityEngine;

namespace Data.Old
{
    public struct xEnemyIntervalArgs
    {
        public Animator Animator;
        public bool CanAttack;

        public xEnemyIntervalArgs(Animator animator, bool attack)
        {
            Animator = animator;
            CanAttack = attack;
        }
    }
}