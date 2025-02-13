using UnityEngine;

namespace Effects
{
    public class DefenseVisualState : MonoBehaviour
    {
        public GameObject normalModel;
        public GameObject damagedModel;
        public GameObject sparksEffect;
        public ParticleSystem smokeEffect;
        public FlickeringLight flickeringLight; 
        
        private bool _sparking;
        private bool _smokeOn;
        
        public void UpdateVisualState(float healthPercentage)
        {
            if (damagedModel) 
            {
                normalModel.SetActive(healthPercentage > 0.5f);
                damagedModel.SetActive(healthPercentage <= 0.5f);
            }

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

            if (sparksEffect)
            {
                bool shouldSpark = healthPercentage <= 0.25f;

                switch (shouldSpark)
                {
                    case true when !_sparking:
                        sparksEffect.SetActive(true);
                        _sparking = true;
                        InvokeRepeating(nameof(FlickerSparks), 0f, 1f);
                        break;
                    case false when _sparking:
                        sparksEffect.SetActive(false);
                        _sparking = false;
                        CancelInvoke(nameof(FlickerSparks));
                        break;
                }
            }
            
            if (flickeringLight) 
            {
                if (healthPercentage <= 0.25f) 
                {
                    flickeringLight.StartFlickering();
                } 
                else 
                {
                    flickeringLight.StopFlickering();
                }
            }
        }

        private void FlickerSparks() 
        {
            sparksEffect.SetActive(!sparksEffect.activeSelf);
        }
    }
}