using Brains;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Wall", menuName = "Defense/Wall", order = 0)]
    public class Wall : Defense
    {
        public AudioClip[] onDestroyClips;

        public void PlayOnDestroySound(DefenseBrain brain)
        {
            brain.PlaySound(onDestroyClips[Random.Range(0, onDestroyClips.Length)]);
        }
    }
}