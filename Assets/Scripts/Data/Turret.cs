using System.Linq;
using Brains;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Defense/Turret", order = 0)]
    public class Turret : Defense
    {
        public GameObject projectilePrefab;
        
        [Header("Turret")]
        public float damage;
        public float range = 5f;
        public float fireRate = 1f;
        public LayerMask enemyLayer;

        public override void OnInterval(Transform transform, Transform firePoint)
        {
            GameObject target = FindTarget(transform);
            
            if (target != null)
            {
                Shoot(target, firePoint);
            }
        }

        private GameObject FindTarget(Transform transform)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, enemyLayer);

            return (from hitCollider in hitColliders where hitCollider.CompareTag("Enemy")
                select hitCollider.gameObject).FirstOrDefault();
        }

        private void Shoot(GameObject target, Transform firePoint)
        {
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Projectile projScript = projectile.GetComponent<Projectile>();
                projScript.Initialize(target.transform, damage);
            }
        }
    }
}