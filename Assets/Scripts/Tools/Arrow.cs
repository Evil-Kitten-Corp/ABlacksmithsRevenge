using UnityEngine;

namespace Tools
{
    public class Arrow : MonoBehaviour
    {
        public void UpdateRotation(Transform lookAt)
        {
            Debug.Log("Tried to rotate arrow");
            transform.LookAt(lookAt);
        }
    }
}