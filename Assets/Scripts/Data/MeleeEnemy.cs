using Data.Structs;
using Interfaces;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemies/Simple", order = 0)]
    public class MeleeEnemy : Enemy
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        
        public override void UpdateBehavior(EnemyArgs args)
        {
            args.EnemyBrain.attackTimer += Time.deltaTime;
            
            //are we walking?
            if (args.EnemyBrain.agent.hasPath)
            {
                return;
            }
            
            if (args.EnemyBrain.target)
            {
                if (args.EnemyBrain.attackTimer >= attackCooldown)
                {
                    args.EnemyBrain.GetComponent<Animator>()?.SetTrigger(Attack);
                    args.EnemyBrain.attackTimer = 0f;
                }
            }
            else
            {
                if (args.EnemyBrain.CheckNextPossibleLaneIndex())
                {
                    args.EnemyBrain.GoToNext();
                }
            }
        }

        public override void OnCellReached(EnemyArgs args)
        {
            Debug.Log(name + " reached cell");
            
            //arrived cell
            var currentCell = args.EnemyBrain.GetCurrentCellPosition();

            //check enemy
            if (args.GridManager.IsPositionOccupied(currentCell))
            {
                args.EnemyBrain.AcquireTarget(args.GridManager.GetTargetOnPosition(currentCell));
                Debug.Log(currentCell + " is occupied and will be attacked");
                
                args.EnemyBrain.attackTimer += Time.deltaTime;
                
                if (args.EnemyBrain.attackTimer >= attackCooldown)
                {
                    args.EnemyBrain.GetComponent<Animator>()?.SetTrigger(Attack);
                    args.EnemyBrain.attackTimer = 0f;
                }
                
                return;
            }

            //if no enemy, next cell
            if (args.EnemyBrain.CheckNextPossibleLaneIndex())
            {
                args.EnemyBrain.GoToNext();
            }
        }

        public override void DealDamage(EnemyArgs args)
        {
            AudioClip clip = weaponHitSounds[Random.Range(0, weaponHitSounds.Length)];
            args.EnemyBrain.audioSource.PlayOneShot(clip);
            
            //get the defense in our cell
            var def = args.GridManager.GetTargetOnPosition(args.EnemyBrain.GetCurrentCellPosition());
            
            if (def == null) 
                return;
            
            IDamageable defense = def.GetComponent<IDamageable>();
            defense?.Damage(damage);
        }
    }
}