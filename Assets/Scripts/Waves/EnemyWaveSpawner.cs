using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Data.Old;
using Placement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Waves
{
    public class EnemyWaveSpawner : MonoBehaviour
    {
        public static event Action GameOver;
        
        public List<EnemyWave> wavesInOrder;
        public GridManager gridManager;
        public float intervalBetweenWaves;

        private int _currentWaveIndex;
        private readonly List<GameObject> _activeEnemies = new();

        private void Start()
        {
            StartCoroutine(SpawnWaves());
        }

        private IEnumerator SpawnWaves()
        {
            while (_currentWaveIndex < wavesInOrder.Count)
            {
                yield return new WaitForSeconds(intervalBetweenWaves);
                StartWave();

                yield return new WaitUntil(() => _activeEnemies.Count == 0);

                Debug.Log("Enemies dead, rewarding now");
                wavesInOrder[_currentWaveIndex].Reward();

                yield return new WaitForSeconds(intervalBetweenWaves);

                _currentWaveIndex++;
            }

            Debug.Log("All waves completed!");
        }

        void StartWave()
        {
            Debug.Log("Starting wave " +  _currentWaveIndex);
            _activeEnemies.Clear();

            StartCoroutine(StartSpawning());
        }

        IEnumerator StartSpawning()
        {
            for (int i = 0; i < wavesInOrder[_currentWaveIndex].maxEnemiesToSpawn; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(wavesInOrder[_currentWaveIndex].spawnInterval);
            }
        }

        void SpawnEnemy()
        {
            if (gridManager.spawnPositions.Count == 0) 
                return;

            Debug.Log("Spawning Enemy, Spawn Pos Is Valid");
            Vector3 spawnPos = gridManager.spawnPositions[Random.Range(0, gridManager.spawnPositions.Count)];
            var en = wavesInOrder[_currentWaveIndex].GetRandom();
            GameObject enemy = Instantiate(en.prefab, spawnPos, Quaternion.identity);
            Debug.Log(enemy.name + " successfully spawned!");
            enemy.GetComponent<EnemyBrain>().Activate(en, spawnPos);
            
            _activeEnemies.Add(enemy);
            enemy.GetComponent<EnemyBrain>().OnDeath += () => _activeEnemies.Remove(enemy);
        }

        public static void OnGameOver()
        {
            Debug.Log("OnGameOver called");
            GameOver?.Invoke();
        }
    }
}