using Brains;
using Data.Structs;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemies/Archer", order = 0)]    
    public class ArcherEnemy : Enemy
    {
        private static readonly int Shoot = Animator.StringToHash("Shoot");
        
        public GameObject projectilePrefab;

        public override void UpdateBehavior(EnemyArgs args)
        {
            args.EnemyBrain.attackTimer += Time.deltaTime;
            
            //do we have an enemy?
            if (args.EnemyBrain.Target)
            {
                //is our attack cd up?
                if (args.EnemyBrain.attackTimer >= attackCooldown)
                {
                    //if can attack, shoot
                    args.EnemyBrain.GetComponent<Animator>().SetTrigger(Shoot);
                    args.EnemyBrain.attackTimer = 0f;
                }
            }
            else //we don't have a target
            {
                //same behavior as if we had reached a cell then
                OnCellReached(args);
            }
        }

        private bool Scan(EnemyArgs args)
        {
            //let's scan to find the farthest enemy
            bool targetFound = false;

            if (args.GridManager.invertSpawnPoints)
            {
                for (int i = args.EnemyBrain.linha; i < args.GridManager.gridWidth; i++)
                {
                    Vector3 cellPos = args.GridManager.gridPositions[i, args.EnemyBrain.coluna];

                    if (args.GridManager.IsPositionOccupied(cellPos))
                    {
                        targetFound = true;
                        args.EnemyBrain.AcquireTarget(args.GridManager.GetTargetOnPosition(cellPos));
                        break;
                    }
                }
            }
            else
            {
                for (int i = args.EnemyBrain.linha; i >= 0; i--)
                {
                    Vector3 cellPos = args.GridManager.gridPositions[i, args.EnemyBrain.coluna];

                    if (args.GridManager.IsPositionOccupied(cellPos))
                    {
                        targetFound = true;
                        args.EnemyBrain.AcquireTarget(args.GridManager.GetTargetOnPosition(cellPos));
                        break;
                    }
                }
            }

            return targetFound;
        }

        public override void OnCellReached(EnemyArgs args)
        {
            bool targetFound = Scan(args);
            
            if (targetFound) //is there enemy?
            {
                //check cooldown
                if (args.EnemyBrain.attackTimer >= attackCooldown)
                {
                    //if can attack, shoot
                    args.EnemyBrain.GetComponent<Animator>().SetTrigger(Shoot);
                    args.EnemyBrain.attackTimer = 0f;
                }
                
                return;
            }
            
            //if no enemy, walk
            if (args.EnemyBrain.CheckNextPossibleLaneIndex())
            {
                args.EnemyBrain.GoToNext();
            }
        }

        public override void DealDamage(EnemyArgs args)
        {
            if (args.EnemyBrain.Target != null)
            {
                if (projectilePrefab != null && args.EnemyBrain.firePoint != null)
                {
                    GameObject projectile = Instantiate(projectilePrefab, 
                        args.EnemyBrain.firePoint.position, Quaternion.identity);
                    Projectile projScript = projectile.GetComponent<Projectile>();
                    projScript.Initialize(args.EnemyBrain.Target.transform, damage);
                }
            }
        }
    }
}