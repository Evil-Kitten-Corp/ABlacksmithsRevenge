using UnityEngine;

namespace Data.Structs
{
    public struct EnemyIntervalArgs
    {
        public Animator Animator;
        public bool CanAttack;

        public EnemyIntervalArgs(Animator animator, bool attack)
        {
            Animator = animator;
            CanAttack = attack;
        }
    }
}