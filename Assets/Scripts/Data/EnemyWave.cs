using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "EnemyWave", menuName = "Waves/New...", order = 0)]
    public class EnemyWave : ScriptableObject
    {
        public List<Enemy> enemiesToSpawn;
        public float spawnInterval = 5f;
        public int enemiesPerWave = 5;
        public Defense reward;

        public Enemy GetRandom()
        {
            return enemiesToSpawn[Random.Range(0, enemiesToSpawn.Count)];
        }

        public void Reward()
        {
            reward.unlocked = true;
        }
    }
}