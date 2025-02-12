using System.Linq;
using Brains;
using Data.Structs;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Defense/Turret", order = 0)]
    public class Turret : Defense
    {
        public GameObject projectilePrefab;

        [Header("Turret")] 
        public AudioClip shootSound;
        public float damage;
        public float range = 5f;
        public float fireRate = 1f;
        public LayerMask enemyLayer;

        public virtual void OnInterval(DefenseIntervalArgs args)
        {
            GameObject target = FindTarget(args.Transform);
            
            if (target != null)
            {
                Shoot(target, args.Brain);
            }
        }

        private GameObject FindTarget(Transform transform)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, enemyLayer);

            return (from hitCollider in hitColliders where hitCollider.CompareTag("Enemy")
                select hitCollider.gameObject).FirstOrDefault();
        }

        private void Shoot(GameObject target, DefenseBrain brain)
        {
            if (projectilePrefab != null && brain.firePoint != null)
            {
                brain.animator.SetTrigger("Shoot");
                Debug.Log($"{brain.name} tried to shoot {target.name}");
                
                if (shootSound != null)
                {
                    brain.PlaySound(shootSound);
                }
                
                GameObject projectile = Instantiate(projectilePrefab, 
                    brain.firePoint.position, Quaternion.identity);
                Projectile projScript = projectile.GetComponent<Projectile>();
                projScript.Initialize(target.transform, damage, null);
                
                brain.ResetFireCooldown();
            }
        }
    }
}