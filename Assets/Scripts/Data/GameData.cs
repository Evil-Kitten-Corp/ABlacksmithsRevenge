using System;
using UnityEngine;
using Waves;

namespace Data
{
    public class GameData : MonoBehaviour
    {
        public static GameData Instance;

        private bool _gameOver;
        private bool _paused;

        private int _waveIndex;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        public event Action<int> StartGame;
        public event Action GameOver;
        public event Action Pause;
        public event Action Resume;

        private void Start()
        {
            _waveIndex = PlayerPrefs.HasKey("WaveIndex") ? PlayerPrefs.GetInt("WaveIndex") : 0;
        }

        public void Save()
        {
            PlayerPrefs.SetInt("WaveIndex", _waveIndex);
        }

        public void OnGameOver(int wave)
        {
            if (_gameOver)
            {
               return; 
            }
            
            GameOver?.Invoke();
            _waveIndex = wave;
            _gameOver = true;
        }

        public void OnPause()
        {
            if (_paused)
            {
                OnResume();
                return;
            }
            
            Pause?.Invoke();
            _paused = true;
        }

        protected virtual void OnResume()
        {
            if (!_paused)
            {
                OnPause();
                return;
            }
            
            Resume?.Invoke();
            _paused = false;
        }

        public virtual void OnStartGame()
        {
            _gameOver = false;
            StartGame?.Invoke(_waveIndex);
        }

        private void OnApplicationQuit()
        {
            var ews = FindAnyObjectByType<EnemyWaveSpawner>();
            
            if (ews != null)
            {
                _waveIndex = ews.GetWaveIndex();
            }
            
            Save();
        }
    }
}