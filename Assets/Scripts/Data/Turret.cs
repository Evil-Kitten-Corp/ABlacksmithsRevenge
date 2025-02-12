using System.Linq;
using Brains;
using Data.Structs;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Turret", menuName = "Defense/Turret", order = 0)]
    public class Turret : Defense
    {
        private static readonly int ShootAnim = Animator.StringToHash("Shoot");
        
        public GameObject projectilePrefab;

        [Header("Turret")] 
        public AudioClip shootSound;
        public float damage;
        public float fireRate = 1f;
        public LayerMask enemyLayer;
        
        [Header("Range")]
        public Vector3 halfExtents;
        public Vector3 originOffset;

        public override void Interval(DefenseArgs args)
        {
            args.Brain.fireCooldown -= Time.deltaTime;

            if (args.Brain.fireCooldown <= 0)
            {
                GameObject target = FindTarget(args.Brain.transform);
            
                if (target != null)
                {
                    Shoot(target, args.Brain);
                }
            }
        }

        private GameObject FindTarget(Transform transform)
        {
            Collider[] hitColliders = Physics.OverlapBox(transform.position + originOffset, 
                halfExtents, Quaternion.identity, enemyLayer);

            return (from hitCollider in hitColliders where hitCollider.CompareTag("Enemy")
                select hitCollider.gameObject).FirstOrDefault();
        }

        private void Shoot(GameObject target, DefenseBrain brain)
        {
            Debug.Log("Try Shooting");
            
            if (projectilePrefab != null && brain.firePoint != null)
            {
                brain.animator.SetTrigger(ShootAnim);
                
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

        public override void ResetCooldown(DefenseArgs args)
        {
            args.Brain.fireCooldown = 1 / fireRate;
        }
    }
}