using UnityEngine;

namespace Effects
{
    public class DefenseVisualState : MonoBehaviour
    {
        public GameObject normalModel;
        public GameObject damagedModel;
        public GameObject sparksEffect;
        public GameObject smokeEffect;
        public FlickeringLight flickeringLight; 
        
        private bool _sparking;
        
        public void UpdateVisualState(float healthPercentage)
        {
            if (damagedModel) 
            {
                normalModel.SetActive(healthPercentage > 0.5f);
                damagedModel.SetActive(healthPercentage <= 0.5f);
            }

            if (smokeEffect) 
            {
                smokeEffect.SetActive(healthPercentage <= 0.5f);
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