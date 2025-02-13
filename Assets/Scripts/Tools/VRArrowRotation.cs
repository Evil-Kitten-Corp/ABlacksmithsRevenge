using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools
{
    public class VRArrowRotation : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] impactSounds;
        [SerializeField] private float homingSpeed = 15f;
        
        private Transform _homingTarget;
        private bool _isHoming;
        
        private void FixedUpdate()
        {
            if (_isHoming && _homingTarget != null)
            {
                //Vector3 direction = (_homingTarget.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, _homingTarget.position, 
                    homingSpeed * Time.fixedDeltaTime);
                transform.LookAt(_homingTarget);
                //transform.forward = Vector3.Slerp(transform.forward, direction, Time.fixedDeltaTime * 10);
            }
            else if (rb != null)
            {
                transform.LookAt(_homingTarget);
                // transform.forward = Vector3.Slerp(transform.forward, rb.linearVelocity.normalized, 
                //     Time.fixedDeltaTime * 10);
            }
        }
        
        public void SetHomingTarget(Transform target)
        {
            _homingTarget = target;
            _isHoming = true;
            rb.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
                audioSource.PlayOneShot(clip);
                other.GetComponent<IDamageable>().Damage(50);
                Destroy(gameObject, clip.length);
            }
        }
    }
}