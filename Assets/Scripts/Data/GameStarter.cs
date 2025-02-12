using System.Collections;
using DG.Tweening;
using Textures;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Data
{
    public class GameStarter : MonoBehaviour
    {
        public LoadingOverlay loadingOverlay;
        private AudioSource[] _audioSources;
        
        public IEnumerator Start()
        {
            loadingOverlay.FadeOut();
            Debug.Log("Called Loading Fade Out on GAME");

            _audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            yield return new WaitUntil(() => GameData.Instance != null);
            GameData.Instance.OnStartGame();

            GameData.Instance.GameOver += () => StartCoroutine(ReturnToMainMenu());
        }

        private IEnumerator ReturnToMainMenu()
        {
            LoadingOverlay overlay = GameObject.Find("LoadingOverlay").gameObject.GetComponent<LoadingOverlay>();
            overlay.FadeIn();

            Sequence sequence = DOTween.Sequence();
            
            foreach (var audioSource in _audioSources)
            {
                sequence.Append(audioSource.DOFade(0f, 1f));
            }

            sequence.Play();
            
            yield return new WaitForSeconds(1.5f);
            
            SceneManager.LoadScene(0);
        }
    }
}