using System.Collections.Generic;
using System.Linq;
using Brains;
using Data.Structs;
using Interfaces;
using Placement;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemies/Mage", order = 0)]
    public class MageEnemy : Enemy
    {
        private static readonly int Cast = Animator.StringToHash("Attack");
        
        public AudioClip[] weaponCastSounds;
        public GameObject projectilePrefab;

        public override void UpdateBehavior(EnemyArgs args)
        {
            args.EnemyBrain.attackTimer += Time.deltaTime;
            
            if (args.EnemyBrain.agent.hasPath)
            {
                return;
            }
            
            //do we have an enemy?
            if (args.EnemyBrain.target)
            {
                //is our attack cd up?
                TryAttack(args.EnemyBrain);
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
                        GameObject target = args.GridManager.GetTargetOnPosition(cellPos);

                        if (!target)
                        {
                            continue;
                        }
                        
                        if (target.GetComponent<DefenseBrain>().GetDefenseType() is Trap)
                        {
                            continue;
                        }
                        
                        targetFound = true;
                        args.EnemyBrain.AcquireTarget(target);
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
                        GameObject target = args.GridManager.GetTargetOnPosition(cellPos);

                        if (!target)
                        {
                            continue;
                        }
                        
                        if (target.GetComponent<DefenseBrain>().GetDefenseType() is Trap)
                        {
                            continue;
                        }
                        
                        targetFound = true;
                        args.EnemyBrain.AcquireTarget(target);
                        break;
                    }
                }
            }

            return targetFound;
        }

        private void TryAttack(EnemyBrain en)
        {
            //check cooldown
            if (en.attackTimer >= attackCooldown)
            {
                //if can attack, shoot
                en.animator.SetTrigger(Cast);
                    
                AudioClip clip = weaponCastSounds[Random.Range(0, weaponCastSounds.Length)];
                en.audioSource.PlayOneShot(clip);
                    
                en.attackTimer = 0f;
            }
        }
        
        public override void OnCellReached(EnemyArgs args)
        {
            bool targetFound = Scan(args);
            
            if (targetFound) //is there an enemy?
            {
                TryAttack(args.EnemyBrain);
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
            // Mage deals AOE damage in a cross pattern.
            if (args.EnemyBrain.target != null)
            {
                // we have a target, now we need to get the units in front, back, right and left
                // of it (if they even exist)
                
                GridManager gridManager = args.GridManager;
                Transform targetTransform = args.EnemyBrain.target.transform;

                Vector3 targetGridPos = gridManager.GetClosestGridPosition(targetTransform.position);
                
                Vector3[] directions = 
                {
                    new(gridManager.cellSize, 0, 0),  // Right
                    new(-gridManager.cellSize, 0, 0), // Left
                    new(0, 0, gridManager.cellSize + gridManager.rowSpacing),  // Up
                    new(0, 0, -gridManager.cellSize - gridManager.rowSpacing) // Down
                };
                
                List<GameObject> otherTargets = new();

                foreach (Vector3 dir in directions)
                {
                    Vector3 checkPos = targetGridPos + dir;
                    GameObject possibleTarget = gridManager.GetTargetOnPosition(checkPos);

                    if (possibleTarget != null)
                    {
                        DefenseBrain defense = possibleTarget.GetComponent<DefenseBrain>();
                
                        if (defense != null && defense.GetDefenseType() is Trap)
                        {
                            Vector3 nextPos = checkPos + dir;
                            GameObject nextTarget = gridManager.GetTargetOnPosition(nextPos);

                            if (nextTarget != null)
                            {
                                otherTargets.Add(nextTarget);
                            }
                        }
                        else
                        {
                            otherTargets.Add(possibleTarget);
                        }
                    }
                }

                // fully dmg the main target
                if (projectilePrefab != null && args.EnemyBrain.firePoint != null)
                {
                    GameObject projectile = Instantiate(projectilePrefab, 
                        args.EnemyBrain.firePoint.position, Quaternion.identity);
                    Projectile projScript = projectile.GetComponent<Projectile>();
                    projScript.Initialize(args.EnemyBrain.target.transform, 
                        args.EnemyBrain.target.GetComponent<IDamageable>(), damage, weaponHitSounds);
                }
                
                // half dmg the other targets
                foreach (var t in otherTargets)
                {
                    if (projectilePrefab != null && args.EnemyBrain.firePoint != null && t != null)
                    {
                        GameObject projectile = Instantiate(projectilePrefab, 
                            args.EnemyBrain.firePoint.position, Quaternion.identity);
                        Projectile projScript = projectile.GetComponent<Projectile>();
                        projScript.Initialize(t.transform, t.GetComponent<IDamageable>(),
                            damage / 2f, weaponHitSounds);
                    }
                }
            }
        }
    }
}
