using UnityEngine;

namespace Data.Structs
{
    public struct DefenseIntervalArgs
    {
        public readonly Transform Transform;
        public readonly Transform FirePoint;

        public DefenseIntervalArgs(Transform transform, Transform firePoint)
        {
            Transform = transform;
            FirePoint = firePoint;
        }
    }
}