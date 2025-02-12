using Brains;
using UnityEngine;

namespace Data.Structs
{
    public struct DefenseIntervalArgs
    {
        public readonly DefenseBrain Brain;
        public readonly Transform Transform;
        public readonly Transform FirePoint;

        public DefenseIntervalArgs(Transform transform, Transform firePoint, DefenseBrain brain)
        {
            Transform = transform;
            FirePoint = firePoint;
            Brain = brain;
        }
    }
}