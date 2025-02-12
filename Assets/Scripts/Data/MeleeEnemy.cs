using Brains;
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
                TryAttack(args.EnemyBrain);
            }
            else
            {
                NextCell(args.EnemyBrain);
            }
        }

        public override void OnCellReached(EnemyArgs args)
        {
            //Debug.Log(name + " reached cell");
            
            //arrived cell
            var currentCell = args.EnemyBrain.GetCurrentCellPosition();

            //check enemy
            if (args.GridManager.IsPositionOccupied(currentCell))
            {
                GameObject target = args.GridManager.GetTargetOnPosition(currentCell);
                
                if (target.GetComponent<DefenseBrain>().GetDefenseType() is Trap)
                {
                    NextCell(args.EnemyBrain);
                    return;
                }
                
                args.EnemyBrain.AcquireTarget(target);
                Debug.Log(currentCell + " is occupied and will be attacked");
                
                args.EnemyBrain.attackTimer += Time.deltaTime;
                
                TryAttack(args.EnemyBrain);
                
                return;
            }

            //if no enemy, next cell
            NextCell(args.EnemyBrain);
        }

        private void TryAttack(EnemyBrain en)
        {
            if (en.attackTimer >= attackCooldown)
            {
                en.animator.SetTrigger(Attack);
                en.attackTimer = 0f;
            }
        }

        private void NextCell(EnemyBrain en)
        {
            if (en.CheckNextPossibleLaneIndex())
            {
                en.GoToNext();
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

            if (def.GetComponent<DefenseBrain>().GetDefenseType() is Trap)
                return;
            
            IDamageable defense = def.GetComponent<IDamageable>();
            defense?.Damage(damage);
        }
    }
}