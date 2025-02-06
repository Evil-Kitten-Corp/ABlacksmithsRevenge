using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FoveationStarter : MonoBehaviour
{
    List<XRDisplaySubsystem> xrDisplays = new List<XRDisplaySubsystem>();
    [SerializeField] float foveationStrength =.5f; // 1 Full strength; .5 medium

    void Start()
    {
        SubsystemManager.GetSubsystems(xrDisplays);
        if (xrDisplays.Count == 1)
        {
            xrDisplays[0].foveatedRenderingLevel = foveationStrength;

            //xrDisplays[0].foveatedRenderingFlags = XRDisplaySubsystem.FoveatedRenderingFlags.GazeAllowed; //For eye tracking devices
        }
    }
}