using Data.Structs;
using Interfaces;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Trap", menuName = "Defense/Trap", order = 0)]
    public class Trap: Defense
    {
        public GameObject explosionPrefab;
        
        public float damage;
        public float timeToActivate;
        public float explosionRadius;
        public AudioClip explodeSound;
        public AudioClip onReadySound;

        public override void Special(DefenseIntervalArgs args)
        {
            args.Brain.FireSpecialVfx(explosionPrefab);
            
            var enemies = Physics.OverlapSphere(args.Transform.position, explosionRadius);

            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<IDamageable>().Damage(damage);
                }
            }
            
            args.Brain.PlaySound(explodeSound);
            Destroy(args.Brain.gameObject, 1f);
        }
    }
}