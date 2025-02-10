using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools
{
    public class VRArrowRotation : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] impactSounds;

        private void FixedUpdate()
        {
            transform.forward = Vector3.Slerp(transform.forward, rb.linearVelocity.normalized, Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
                audioSource.PlayOneShot(clip);
                Destroy(gameObject, clip.length);
            }
        }
    }
}