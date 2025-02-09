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
            int nextCol = args.GridManager.invertSpawnPoints ? 
                args.EnemyBrain.currentColumn + 1 : args.EnemyBrain.currentColumn - 1;
            
            if (nextCol < 0 || nextCol >= args.GridManager.gridWidth)
                return;
            
            Vector3 nextCell = args.GridManager.gridPositions[nextCol, args.EnemyBrain.laneIndex];
            
            if (args.GridManager.IsPositionOccupied(nextCell))
            {
                GameObject go = args.GridManager.GetTargetOnPosition(nextCell);
                Defense defense = go.GetComponent<DefenseBrain>().GetDefenseType();
                
                // If the defense is a Wall-type, leap over it
                if (defense && defense is Wall)
                {
                    // Leap by advancing two cells instead of one (clamp to args.GridManager bounds)
                    args.EnemyBrain.currentColumn = args.GridManager.invertSpawnPoints ? 
                        Mathf.Min(args.EnemyBrain.currentColumn + 2, args.GridManager.gridWidth - 1) 
                        : Mathf.Max(args.EnemyBrain.currentColumn - 2, 0);
                    
                    Vector3 leapTarget = args.GridManager.gridPositions[args.EnemyBrain.currentColumn, 
                        args.EnemyBrain.laneIndex];
                    args.EnemyBrain.agent.SetDestination(leapTarget);
                }
                else
                {
                    // Otherwise, attack normally
                    args.EnemyBrain.attackTimer += Time.deltaTime;
                    
                    if (args.EnemyBrain.attackTimer >= attackCooldown)
                    {
                        args.EnemyBrain.GetComponent<Animator>()?.SetTrigger(Attack);
                        args.EnemyBrain.attackTimer = 0f;
                    }
                }
            }
            else
            {
                if (!args.EnemyBrain.agent.pathPending && args.EnemyBrain.agent.remainingDistance 
                    <= args.EnemyBrain.agent.stoppingDistance)
                {
                }
            }
        }

        public override void OnCellReached(EnemyArgs args)
        {
            
        }

        public override void DealDamage(EnemyArgs args)
        {
            int targetCol = args.GridManager.invertSpawnPoints ? args.EnemyBrain.currentColumn + 1 
                : args.EnemyBrain.currentColumn - 1;
            
            if (targetCol < 0 || targetCol >= args.GridManager.gridWidth)
                return;
            
            Vector3 targetCell = args.GridManager.gridPositions[targetCol, args.EnemyBrain.laneIndex];
            
            if (args.GridManager.IsPositionOccupied(targetCell))
            {
                IDamageable defense = args.GridManager.GetTargetOnPosition(targetCell).GetComponent<IDamageable>();
                defense?.Damage(damage);
            }
        }
    }
}
