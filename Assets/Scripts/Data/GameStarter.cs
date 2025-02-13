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

            _audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            yield return new WaitUntil(() => GameData.Instance != null);
            GameData.Instance.OnStartGame();

            GameData.Instance.GameOver += ReturnToMainMenu;
        }

        private void ReturnToMainMenu()
        {
            LoadingOverlay overlay = GameObject.Find("LoadingOverlay").gameObject.GetComponent<LoadingOverlay>();
            overlay.FadeIn();

            Sequence sequence = DOTween.Sequence();
            
            foreach (var audioSource in _audioSources)
            {
                sequence.Append(audioSource.DOFade(0f, .5f));
            }

            sequence.onComplete += () => SceneManager.LoadScene(0);
            sequence.Play();
        }
    }
}