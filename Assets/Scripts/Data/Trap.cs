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

        public override void Interval(DefenseArgs args)
        {
            var b = args.Brain;
            
            if (args.Brain.generalActive == false)
            {
                b.generalTimer += Time.deltaTime;

                if (b.generalTimer >= timeToActivate)
                {
                    if (b.modelRenderers[0].material.color != Color.white)
                    {
                        foreach (var m in b.modelRenderers)
                        {
                            m.material.color = Color.white;
                        }
                    }
                        
                    b.PlaySound(onReadySound);
                        
                    Debug.Log("Trap ready");
                    b.generalActive = true;
                }
                else
                {
                    if (b.modelRenderers[0].material.color != Color.red)
                    {
                        foreach (var m in b.modelRenderers)
                        {
                            m.material.color = Color.red;
                        }
                    }
                }
            }
        }

        public override void Special(DefenseArgs args)
        {
            args.Brain.exploding = true;
            args.Brain.FireSpecialVfx(explosionPrefab);
            
            var enemies = Physics.OverlapSphere(args.Brain.transform.position, explosionRadius);

            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    if (enemy.TryGetComponent<IDamageable>(out var id))
                    {
                        id.Damage(damage);
                    }
                }
            }
            
            args.Brain.PlaySound(explodeSound);
            
            Transform[] model = args.Brain.transform.GetComponentsInChildren<Transform>();

            foreach (var t in model)
            {
                if (t.gameObject != args.Brain.gameObject)
                {
                    t.gameObject.SetActive(false);
                }
            }
            
            Destroy(args.Brain.gameObject, 2f);
        }
    }
}