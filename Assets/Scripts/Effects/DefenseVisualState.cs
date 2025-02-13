using UnityEngine;

namespace Effects
{
    public class DefenseVisualState : MonoBehaviour
    {
        public ParticleSystem smokeEffect;
        
        private bool _smokeOn;
        
        public void UpdateVisualState(float healthPercentage)
        {
            if (smokeEffect) 
            {
                if (healthPercentage <= 0.5f)
                {
                    if (!_smokeOn)
                    {
                        smokeEffect.Play();
                        _smokeOn = true;
                    }
                }
                else
                {
                    if (_smokeOn)
                    {
                        smokeEffect.Stop();
                        _smokeOn = false;
                    }
                }
            }
        }
    }
}