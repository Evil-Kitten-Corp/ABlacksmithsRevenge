using Interfaces;
using UnityEngine;

namespace Brains
{
    public class Projectile : MonoBehaviour
    {
        public AudioSource source;
        public float speed = 10f;
        
        private float _damage;
        private Transform _target;
        
        private AudioClip _impactSound;
        private IDamageable _targetDmg;

        public void Initialize(Transform target, IDamageable targetDamageable, float damage, AudioClip[] weaponHitSounds)
        {
            _target = target;
            _targetDmg = targetDamageable;
            _damage = damage;
            
            Debug.Log($"PROJECTILE: Initialized with {target.name} as target and damage " +
                      $"is {damage}, and IDamageable is {targetDamageable != null}");

            if (weaponHitSounds == null)
            {
                return;
            }

            _impactSound = weaponHitSounds[Random.Range(0, weaponHitSounds.Length)];
        }

        private void Update()
        {
            if (_target)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.position, 
                    speed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_target != null)
            {
                if (other.transform == _target)
                {
                    Debug.Log("PROJECTILE: Detected other is target");
                    HitTarget();
                }
            }
        }

        private void HitTarget()
        {
            if (_target != null)
            {
                Debug.Log("PROJECTILE: Damaging enemy");
                _targetDmg.Damage(_damage);
                source.PlayOneShot(_impactSound);
            }
        
            Destroy(gameObject);
        }
    }
}