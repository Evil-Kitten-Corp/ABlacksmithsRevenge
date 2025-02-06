using System.Collections;
using System.Collections.Generic;
using Brains;
using Data;
using Placement;
using UnityEngine;

namespace Waves
{
    public class EnemyWaveSpawner : MonoBehaviour
    {
        public List<EnemyWave> wavesInOrder;
        public GridManager gridManager;
        public float intervalBetweenWaves;

        private int _currentWaveIndex = 0;
        private List<GameObject> activeEnemies = new List<GameObject>();

        private void Start()
        {
            StartCoroutine(SpawnWaves());
        }

        private IEnumerator SpawnWaves()
        {
            while (_currentWaveIndex < wavesInOrder.Count)
            {
                yield return new WaitForSeconds(wavesInOrder[_currentWaveIndex].spawnInterval);
                StartWave();

                // wait until all spawned enemies are dead
                yield return new WaitUntil(() => activeEnemies.Count == 0);

                // reward the player
                wavesInOrder[_currentWaveIndex].Reward();

                // wait before starting next wave
                yield return new WaitForSeconds(intervalBetweenWaves);

                // move to next wave
                _currentWaveIndex++;
            }

            Debug.Log("All waves completed!");
        }

        void StartWave()
        {
            activeEnemies.Clear();
            
            for (int i = 0; i < wavesInOrder[_currentWaveIndex].maxEnemiesToSpawn; i++)
            {
                SpawnEnemy();
            }
        }

        void SpawnEnemy()
        {
            if (gridManager.spawnPositions.Count == 0) 
                return;

            Vector3 spawnPos = gridManager.spawnPositions[Random.Range(0, gridManager.spawnPositions.Count)];
            var en = wavesInOrder[_currentWaveIndex].GetRandom();
            GameObject enemy = Instantiate(en.prefab, spawnPos, Quaternion.identity);
            enemy.AddComponent<EnemyBrain>().Activate(en, spawnPos);
            
            activeEnemies.Add(enemy);
            enemy.GetComponent<EnemyBrain>().OnDeath += () => activeEnemies.Remove(enemy);
        }
    }
}