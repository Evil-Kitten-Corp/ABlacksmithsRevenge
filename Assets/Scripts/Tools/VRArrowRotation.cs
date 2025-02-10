using UnityEngine;

namespace Tools
{
    public class VRArrowRotation : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;

        private void FixedUpdate()
        {
            transform.forward = Vector3.Slerp(transform.forward, rb.linearVelocity.normalized, Time.fixedDeltaTime);
        }
    }
}