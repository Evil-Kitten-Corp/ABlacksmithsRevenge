using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    [CreateAssetMenu(fileName = "EnemyWave", menuName = "Waves/New...", order = 0)]
    public class EnemyWave : ScriptableObject
    {
        public static event Action OnRewardUnlocked;
        
        [SerializedDictionary("Enemies", "Chance To Spawn from 0 to 1")]
        public SerializedDictionary<Enemy, float> enemiesSpawn;
        public float spawnInterval = 5f;
        public int maxEnemiesToSpawn = 5;
        public Defense reward;

        public Enemy GetRandom()
        {
            float totalWeight = enemiesSpawn.Sum(pair => pair.Value);
            float randomValue = Random.value * totalWeight; //random value between 0 and total weight
            float cumulativeWeight = 0;

            foreach (var pair in enemiesSpawn)
            {
                cumulativeWeight += pair.Value;
                
                if (randomValue <= cumulativeWeight) 
                {
                    return pair.Key;
                }
            }

            return null; //should never happen if weights are set properly
        }

        public void Reward()
        {
            if (reward != null)
            {
                reward.unlocked = true;
                OnRewardUnlocked?.Invoke();
            }
        }
    }
}