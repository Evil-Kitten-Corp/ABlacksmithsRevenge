using Interfaces;
using UnityEngine;

namespace Brains
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 10f;
        
        private float _damage;
        private Transform _target;

        public void Initialize(Transform target, float damage)
        {
            _target = target;
            _damage = damage;
        }

        private void Update()
        {
            if (_target != null)
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
                    HitTarget();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        private void HitTarget()
        {
            if (_target != null)
            {
                IDamageable enemy = _target.GetComponent<IDamageable>();

                enemy?.Damage(_damage);
            }
        
            Destroy(gameObject);
        }
    }
}