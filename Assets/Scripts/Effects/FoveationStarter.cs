using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Effects
{
    public class FoveationStarter : MonoBehaviour
    {
        private readonly List<XRDisplaySubsystem> _xrDisplays = new();
        [SerializeField] private float foveationStrength = 0.5f; //1 full strength; .5 medium

        private void Start()
        {
            SubsystemManager.GetSubsystems(_xrDisplays);
            
            if (_xrDisplays.Count == 1)
            {
                _xrDisplays[0].foveatedRenderingLevel = foveationStrength;
            }
        }
    }
}