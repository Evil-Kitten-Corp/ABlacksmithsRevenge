using System.Collections;
using Brains;
using Data;
using Placement;
using UnityEngine;

namespace Waves
{
    public class EnemyWaveSpawner : MonoBehaviour
    {
        public EnemyWave wave;
        public GridManager gridManager;

        private void Start()
        {
            StartCoroutine(SpawnWaves());
        }

        private IEnumerator SpawnWaves()
        {
            while (true)
            {
                yield return new WaitForSeconds(wave.spawnInterval);
                StartWave();
            }
        }

        void StartWave()
        {
            for (int i = 0; i < wave.enemiesPerWave; i++)
            {
                SpawnEnemy();
            }
        }

        void SpawnEnemy()
        {
            if (gridManager.spawnPositions.Count == 0) 
                return;

            Vector3 spawnPos = gridManager.spawnPositions[Random.Range(0, gridManager.spawnPositions.Count)];
            var en = wave.GetRandom();
            GameObject enemy = Instantiate(en.prefab, spawnPos, Quaternion.identity);
            enemy.AddComponent<EnemyBrain>().Activate(en, spawnPos);
        }
    }
}