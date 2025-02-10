using System.Collections;
using Brains;
using Data.Structs;
using Interfaces;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemies/Leaper", order = 0)]    
    public class LeaperEnemy : Enemy
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
            
            if (args.EnemyBrain.Target)
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
                //check if its occupied by a wall
                var t = args.GridManager.GetTargetOnPosition(currentCell);

                if (t.TryGetComponent<DefenseBrain>(out var db))
                {
                    if (db.GetDefenseType() is Wall)
                    {
                        PerformLeapOver(args, db.transform.position);
                        return;
                    }
                    else
                    {
                        args.EnemyBrain.AcquireTarget(t);
                    }
                }
                
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
            //get the defense in our cell
            var def = args.GridManager.GetTargetOnPosition(args.EnemyBrain.GetCurrentCellPosition());
            
            if (def == null) 
                return;
            
            IDamageable defense = def.GetComponent<IDamageable>();
            defense?.Damage(damage);
        }

        private void PerformLeapOver(EnemyArgs args, Vector3 obstaclePosition)
        {
            // temporarily disable NavMeshAgent to allow manual movement
            args.EnemyBrain.agent.isStopped = true;
            args.EnemyBrain.agent.enabled = false;

            // calculate jump arc
            Vector3 jumpTarget = obstaclePosition + args.EnemyBrain.transform.forward * 2f;
            args.EnemyBrain.StartCoroutine(JumpToPosition(args, jumpTarget));
        }

        private IEnumerator JumpToPosition(EnemyArgs args, Vector3 targetPosition)
        {
            float jumpTime = 0.5f;
            float elapsedTime = 0f;
            Vector3 startPosition = args.EnemyBrain.transform.position;
            Vector3 peakPosition = (startPosition + targetPosition) / 2 + Vector3.up * 2f;

            while (elapsedTime < jumpTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / jumpTime;

                // parabolic interpolation
                args.EnemyBrain.transform.position = Vector3.Lerp(Vector3.Lerp(startPosition, peakPosition, t), 
                    Vector3.Lerp(peakPosition, targetPosition, t), t);
                yield return null;
            }

            // reactivate NavMeshAgent
            args.EnemyBrain.agent.enabled = true;
            args.EnemyBrain.agent.isStopped = false;
        }
    }
}
