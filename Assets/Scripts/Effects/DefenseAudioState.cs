using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(AudioSource))]
    public class DefenseAudioState : MonoBehaviour
    {
        [SerializeField] private AudioClip repairSound;
        
        private AudioSource _source;

        private void Start()
        {
            _source = GetComponent<AudioSource>();
        }

        public void PlayRepairSound()
        {
            _source.PlayOneShot(repairSound);
        }
    }
}