using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(AudioSource))]
    public class DefenseAudioState : MonoBehaviour
    {
        [SerializeField] private AudioClip[] repairSounds;
        [SerializeField] private AudioClip[] shovelSounds;
        
        private AudioSource _source;

        private void Start()
        {
            _source = GetComponent<AudioSource>();
        }

        public void PlayRepairSound()
        {
            return;
            
/*
            AudioClip clip = repairSounds[Random.Range(0, repairSounds.Length)];
            _source.PlayOneShot(clip);
*/
        }

        public void PlayShovelSound()
        {
            return;
            
/*
            AudioClip clip = shovelSounds[Random.Range(0, shovelSounds.Length)];
            _source.PlayOneShot(clip);
*/
        }
    }
}