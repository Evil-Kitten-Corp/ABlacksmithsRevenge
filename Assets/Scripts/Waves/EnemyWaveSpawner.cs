using System.Collections;
using System.Collections.Generic;
using Brains;
using Data;
using DG.Tweening;
using Placement;
using TMPro;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Waves
{
    public class EnemyWaveSpawner : MonoBehaviour
    {
        public List<EnemyWave> wavesInOrder;
        public GridManager gridManager;
        public float intervalBetweenWaves;
        public float restTime;
        
        public TextMeshProUGUI countdownText;
        public AudioSource horn;

        public AudioSource countdown;
        
        [Header("Audio")]
        public AudioClip countdownClip;
        public AudioClip definitiveWin;
        public AudioClip winClip;
        public AudioClip loseClip;
        public AudioClip nextLevelClip;

        [Header("Debug")] 
        public bool doNotSpawn;
        
        private int _currentWaveIndex;

        private bool _spawnComplete;
        private readonly List<GameObject> _activeEnemies = new();

        private bool _paused;
        private Coroutine _spawningCoroutine;
        
        private Tween _pausedTween;

        private void Start()
        {
            GameData.Instance.StartGame += i =>
            {
                if (doNotSpawn)
                    return;

                StartCoroutine(SpawnWaves(i));
            };
            
            GameData.Instance.Pause += OnPause;
            GameData.Instance.Resume += OnResume;

            GameData.Instance.GameOver += () =>
            {
                countdown.PlayOneShot(loseClip);
            };
        }

        private void OnDestroy()
        {
            if (GameData.Instance != null)
            {
                GameData.Instance.Pause -= OnPause;
                GameData.Instance.Resume -= OnResume;
            }
        }

        private void OnPause()
        {
            _paused = true;
        }

        private void OnResume()
        {
            _paused = false;
        }

        public void Defeat()
        {
            GameData.Instance.OnGameOver(_currentWaveIndex);
            GameData.Instance.Save();
        }

        private IEnumerator SpawnWaves(int i)
        {
            _currentWaveIndex = i;
            
            while (_currentWaveIndex < wavesInOrder.Count)
            {
                yield return StartCoroutine(ShowCountdown(intervalBetweenWaves));
                StartWave();

                yield return new WaitUntil(() => _activeEnemies.Count == 0 && _spawnComplete);

                countdown.PlayOneShot(winClip);
                wavesInOrder[_currentWaveIndex].Reward();
                Cleanup();

                yield return new WaitForSeconds(restTime);

                _currentWaveIndex++;
            }
            
            Debug.Log("All waves completed!");
            countdown.PlayOneShot(definitiveWin);
            StartCoroutine(Win());
        }
        
        private IEnumerator Win() 
        {
            yield return new WaitForSeconds(1f);
            GameData.Instance.Save(_currentWaveIndex);
            FindAnyObjectByType<GameStarter>().ReturnToMainMenu();
        }

        private void Cleanup()
        {
            var turrets = GameObject.FindGameObjectsWithTag("Turret");

            foreach (var t in turrets)
            {
                t.GetComponent<DefenseBrain>().TryDestroy(0.2f);
            }
            
            countdown.PlayOneShot(nextLevelClip);
            _spawnComplete = false;
        }

        private IEnumerator ShowCountdown(float duration)
        {
            for (int i = Mathf.CeilToInt(duration); i > 0; i--)
            {
                while (_paused)
                {
                    if (countdownText.alpha != 0 && _pausedTween == null)
                    {
                        _pausedTween = countdownText
                            .DOFade(0, 0.5f)
                            .OnComplete(() => _pausedTween = null);
                    }
                    
                    yield return null;
                }

                if (i == 3 && !countdown.isPlaying)
                {
                    countdown.PlayOneShot(countdownClip);
                }
                
                countdownText.text = i.ToString();
                countdownText.transform.localScale = Vector3.one * 1.5f;
                countdownText.DOFade(1, 0.1f);
                countdownText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);

                yield return new WaitForSeconds(1f);
            }
            
            while (_paused)
            {
                if (countdownText.alpha != 0 && _pausedTween == null)
                {
                    _pausedTween = countdownText
                        .DOFade(0, 0.5f)
                        .OnComplete(() => _pausedTween = null);
                }
                
                yield return null;
            }

            countdownText.text = "GO!";
            countdownText.transform.localScale = Vector3.one * 1.5f;
            countdownText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);

            yield return new WaitForSeconds(0.5f);
            countdownText.DOFade(0, 0.5f);
        }

        private void StartWave()
        {
            horn.Play();
            Debug.Log("Starting wave " +  _currentWaveIndex);
            _activeEnemies.Clear();

            StartCoroutine(StartSpawning());
        }

        private IEnumerator StartSpawning()
        {
            for (int i = 0; i < wavesInOrder[_currentWaveIndex].maxEnemiesToSpawn; i++)
            {
                while (_paused)
                {
                    yield return null;
                }
                
                SpawnEnemy();
                yield return new WaitForSeconds(wavesInOrder[_currentWaveIndex].spawnInterval);
            }

            _spawnComplete = true;
        }

        private void SpawnEnemy()
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

        public int GetWaveIndex() => _currentWaveIndex;
    }
}