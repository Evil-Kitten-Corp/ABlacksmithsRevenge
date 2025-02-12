using System.Collections;
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

        private AudioSource[] _audioSources;
        
        private void Start()
        {
            _audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            loadingOverlay.FadeOut();
            Debug.Log("Called Loading Fade Out on MENU");
            
            if (PlayerPrefs.HasKey("FirstTime"))
            {
                if (PlayerPrefs.GetInt("FirstTime") == 0)
                {
                    retryText.text = "Start";
                }
                else
                {
                    int wv = PlayerPrefs.GetInt("WaveIndex");

                    retryText.text = $"Retry Wave {wv + 1}";
                }
            }
            else
            {
                retryText.text = "Start";
                PlayerPrefs.SetInt("FirstTime", 1);
            }
        }
        
        public void GoToGame()
        {
            StartCoroutine(LoadGame());
        }

        public void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
            #else
                Application.Quit();
            #endif
        }

        private IEnumerator LoadGame()
        {
            loadingOverlay.FadeIn();
            
            Sequence sequence = DOTween.Sequence();
            
            foreach (var audioSource in _audioSources)
            {
                sequence.Append(audioSource.DOFade(0f, 2f));
            }

            sequence.Play();
            
            yield return new WaitForSeconds(2.5f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
