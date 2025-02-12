using Brains;
using Data.Structs;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Wall", menuName = "Defense/Wall", order = 0)]
    public class Wall : Defense
    {
        public AudioClip[] onDestroyClips;

        public override void OnDeath(DefenseArgs args)
        {
            PlayOnDestroySound(args.Brain);
        }

        private void PlayOnDestroySound(DefenseBrain brain)
        {
            brain.PlaySound(onDestroyClips[Random.Range(0, onDestroyClips.Length)]);
        }
    }
}