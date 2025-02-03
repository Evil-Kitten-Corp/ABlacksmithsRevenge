using UnityEngine;

namespace Effects
{
    public class FlickeringLight : MonoBehaviour 
    {
        public Light warningLight;
        [SerializeField] private float flickerSpeed = 0.2f;
        
        private bool _isFlickering;

        public void StartFlickering() 
        {
            if (!_isFlickering) 
            {
                _isFlickering = true;
                InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
            }
        }

        public void StopFlickering() 
        {
            _isFlickering = false;
            CancelInvoke(nameof(Flicker));
            
            if (warningLight) 
                warningLight.enabled = false;
        }

        private void Flicker() 
        {
            if (warningLight) 
                warningLight.enabled = !warningLight.enabled;
        }
    }
}