using System.Linq;
using Brains;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Defense/Turret", order = 0)]
    public class Turret : Defense
    {
        public GameObject projectilePrefab;

        public override void ActivateBrain()
        {
        }

        public override void DeactivateBrain()
        {
        }

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
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

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