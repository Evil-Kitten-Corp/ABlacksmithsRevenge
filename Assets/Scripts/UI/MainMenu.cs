using System.Collections;
using Data;
using DG.Tweening;
using Textures;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public LoadingOverlay loadingOverlay;
        public TMP_Text retryText;
        public GameObject newGameButton;

        private AudioSource[] _audioSources;
        
        private void Start()
        {
            _audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            loadingOverlay.FadeOut();
            Debug.Log("Called Loading Fade Out on MENU");
            
            if (PlayerPrefs.HasKey("WaveIndex"))
            {
                if (PlayerPrefs.GetInt("WaveIndex") == 0)
                {
                    retryText.text = "Start";
                    newGameButton.SetActive(false);
                }
                else
                {
                    int wv = PlayerPrefs.GetInt("WaveIndex");

                    retryText.text = $"Retry Wave {wv + 1}";
                    newGameButton.SetActive(true);
                }
            }
            else
            {
                retryText.text = "Start";
                newGameButton.SetActive(false);
            }
        }
        
        public void GoToGame()
        {
            LoadGame();
        }

        public void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
            #else
                Application.Quit();
            #endif
        }

        public void NewGame()
        {
            GameData.Instance.OnGameOver(0);
            GameData.Instance.Save();
            LoadGame();
        }

        private void LoadGame()
        {
            loadingOverlay.FadeIn();
            
            Sequence sequence = DOTween.Sequence();
            
            foreach (var audioSource in _audioSources)
            {
                sequence.Append(audioSource.DOFade(0f, .5f));
            }

            sequence.onComplete += () => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            sequence.Play();
        }
    }
}
