using System.Collections.Generic;
using System.Linq;
using Brains;
using Data.Structs;
using Interfaces;
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
                
                int targetLinha = args.EnemyBrain.target.GetComponent<EnemyBrain>().linha;
                int targetColuna = args.EnemyBrain.target.GetComponent<EnemyBrain>().coluna;

                Vector3[] possiblePositions = 
                {
                    GetValidTargetPosition(targetLinha + 1, targetColuna, args), // Front
                    GetValidTargetPosition(targetLinha - 1, targetColuna, args), // Back
                    GetValidTargetPosition(targetLinha, targetColuna + 1, args), // Right
                    GetValidTargetPosition(targetLinha, targetColuna - 1, args)  // Left
                };

                List<GameObject> otherTargets = 
                    (from pos in possiblePositions 
                        where pos != Vector3.zero && args.GridManager.IsPositionOccupied(pos) 
                        select args.GridManager.GetTargetOnPosition(pos)).ToList();

                // fully dmg the main target
                if (projectilePrefab != null && args.EnemyBrain.firePoint != null)
                {
                    GameObject projectile = Instantiate(projectilePrefab, 
                        args.EnemyBrain.firePoint.position, Quaternion.identity);
                    Projectile projScript = projectile.GetComponent<Projectile>();
                    projScript.Initialize(args.EnemyBrain.target.transform, args.EnemyBrain.target.GetComponent<IDamageable>(), damage, weaponHitSounds);
                }
                
                // half dmg the other targets
                foreach (var t in otherTargets)
                {
                    if (projectilePrefab != null && args.EnemyBrain.firePoint != null && t != null)
                    {
                        GameObject projectile = Instantiate(projectilePrefab, 
                            args.EnemyBrain.firePoint.position, Quaternion.identity);
                        Projectile projScript = projectile.GetComponent<Projectile>();
                        projScript.Initialize(t.transform, t.GetComponent<IDamageable>(),damage / 2f, weaponHitSounds);
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets a valid target position, skipping traps.
        /// </summary>
        private Vector3 GetValidTargetPosition(int linha, int coluna, EnemyArgs args)
        {
            Vector3 pos = new Vector3(linha, coluna, 0);

            if (!args.GridManager.IsPositionOccupied(pos)) 
                return pos; //if empty, also return as valid

            GameObject potentialTarget = args.GridManager.GetTargetOnPosition(pos);
            
            if (potentialTarget == null) 
                return pos;

            DefenseBrain defense = potentialTarget.GetComponent<DefenseBrain>();
            
            if (defense != null && defense.GetDefenseType() is Trap)
            {
                //we move one more step in the same direction
                int dLinha = linha - args.EnemyBrain.target.GetComponent<EnemyBrain>().linha;
                int dColuna = coluna - args.EnemyBrain.target.GetComponent<EnemyBrain>().coluna;
                
                Vector3 newPos = new Vector3(linha + dLinha, coluna + dColuna, 0);

                //if the new position is occupied and not a trap, return it; otherwise, return zero
                if (args.GridManager.IsPositionOccupied(newPos))
                {
                    GameObject nextTarget = args.GridManager.GetTargetOnPosition(newPos);
                    if (nextTarget != null && (nextTarget.GetComponent<DefenseBrain>() == null || 
                                               nextTarget.GetComponent<DefenseBrain>().GetDefenseType() is not Trap))
                    {
                        return newPos;
                    }
                }
                return Vector3.zero; //no valid target
            }

            return pos; //return original if not a trap
        }
    }
}
