using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools
{
    public class VRArrowRotation : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] impactSounds;
        [SerializeField] private float homingSpeed = 15f;
        [SerializeField] private float baseDamage = 50f;
        [SerializeField] private float turnSpeed = 5f; 

        private float _speed;
        private Transform _homingTarget;
        private bool _isHoming;

        private void Update()
        {
            if (_isHoming)
            {
                Vector3 direction = (_homingTarget.position - transform.position).normalized;
                transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * turnSpeed);
                transform.position += transform.forward * (homingSpeed * Time.deltaTime);
            }
            else
            {
                transform.position += transform.forward * (_speed * Time.deltaTime);
            }
        }
        
        public void SetHomingTarget(Transform target)
        {
            _homingTarget = target;
            _isHoming = true;
        }
        
        public void SetSpeed(float arrowSpeed)
        {
            _speed = arrowSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
                audioSource.PlayOneShot(clip);
                other.GetComponent<IDamageable>().Damage(baseDamage);
                Destroy(gameObject, clip.length);
            }
        }
    }
}