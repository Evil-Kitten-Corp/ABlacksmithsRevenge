using System;
using UnityEngine;

namespace Data
{
    public class GameData : MonoBehaviour
    {
        public static GameData Instance;

        private bool _gameOver;
        private bool _paused;

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
        
        public event Action GameOver;
        public event Action Pause;
        public event Action Resume;

        public void OnGameOver()
        {
            if (_gameOver)
            {
               return; 
            }
            
            GameOver?.Invoke();
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
    }
}