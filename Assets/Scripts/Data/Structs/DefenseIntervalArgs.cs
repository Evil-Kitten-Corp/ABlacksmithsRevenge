using UnityEngine;

namespace Data.Structs
{
    public struct DefenseIntervalArgs
    {
        public Transform Transform;
        public Transform FirePoint;

        public DefenseIntervalArgs(Transform transform, Transform firePoint)
        {
            Transform = transform;
            FirePoint = firePoint;
        }
    }
}