using Data.Structs;
using Interfaces;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemies/Mage", order = 0)]
    public class MageEnemy : Enemy
    {
        private static readonly int Attack = Animator.StringToHash("Attack");

        public override void UpdateBehavior(EnemyArgs args)
        {
            // Mage scans the lane for any defense target
            bool targetFound = false;
            int step = args.GridManager.invertSpawnPoints ? 1 : -1;
            int scanStart = args.EnemyBrain.currentColumn;
            int scanEnd = args.GridManager.invertSpawnPoints ? args.GridManager.gridWidth - 1 : 0;
            
            for (int col = scanStart; args.GridManager.invertSpawnPoints ? col <= scanEnd : col >= scanEnd; col += step)
            {
                Vector3 cellPos = args.GridManager.gridPositions[col, args.EnemyBrain.laneIndex];
                
                if (args.GridManager.IsPositionOccupied(cellPos))
                {
                    targetFound = true;
                    break;
                }
            }
        
            if (targetFound)
            {
                args.EnemyBrain.agent.isStopped = true;
                args.EnemyBrain.attackTimer += Time.deltaTime;
                if (args.EnemyBrain.attackTimer >= attackCooldown)
                {
                    args.EnemyBrain.GetComponent<Animator>()?.SetTrigger(Attack);
                    args.EnemyBrain.attackTimer = 0f;
                }
            }
            else
            {
                if (args.EnemyBrain.agent.isStopped)
                {
                    args.EnemyBrain.agent.isStopped = false;
                }
            }
        }

        public override void OnCellReached(EnemyArgs args)
        {
            
        }

        public override void DealDamage(EnemyArgs args)
        {
            // Mage deals AOE damage in a cross pattern.
            // Find the farthest defense in the lane
            
            int step = args.GridManager.invertSpawnPoints ? 1 : -1;
            int scanStart = args.EnemyBrain.currentColumn;
            int scanEnd = args.GridManager.invertSpawnPoints ? args.GridManager.gridWidth - 1 : 0;
            int targetCol = -1;
            
            for (int col = scanStart; args.GridManager.invertSpawnPoints ? col <= scanEnd : col >= scanEnd; col += step)
            {
                Vector3 cellPos = args.GridManager.gridPositions[col, args.EnemyBrain.laneIndex];
                if (args.GridManager.IsPositionOccupied(cellPos))
                {
                    targetCol = col;
                    break;
                }
            }
            
            if (targetCol == -1) 
                return;
        
            // Center cell gets full damage
            Vector3 centerPos = args.GridManager.gridPositions[targetCol, args.EnemyBrain.laneIndex];
            if (args.GridManager.IsPositionOccupied(centerPos))
            {
                IDamageable defense = args.GridManager.GetTargetOnPosition(centerPos).GetComponent<IDamageable>();
                defense?.Damage(damage);
            }
        
            // Adjacent cells (up and down in the args.GridManager) get half damage
            int lane = args.EnemyBrain.laneIndex;
            
            int[] dRow = { -1, 1 };
            
            foreach (int dr in dRow)
            {
                int newRow = lane + dr;
                
                if (newRow >= 0 && newRow < args.GridManager.gridHeight)
                {
                    Vector3 adjacentPos = args.GridManager.gridPositions[targetCol, newRow];
                    
                    if (args.GridManager.IsPositionOccupied(adjacentPos))
                    {
                        IDamageable defense = args.GridManager.GetTargetOnPosition(adjacentPos).GetComponent<IDamageable>();
                        defense?.Damage(damage/2f);
                    }
                }
            }
        }
    }
}
