using UnityEngine;

namespace Tools
{
    public class Arrow : MonoBehaviour
    {
        public void UpdateRotation(Transform lookAt)
        {
            transform.LookAt(lookAt);
        }
    }
}