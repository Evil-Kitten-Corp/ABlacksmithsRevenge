using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tools
{
    [RequireComponent(typeof(XRSocketInteractor))]
    public class BowTut : MonoBehaviour
    {
        public Transform start, end;
        public LineRenderer lineString;

        private void Update()
        {
            lineString.SetPosition(1, transform.position);
        }

        private float CalculatePull(Vector3 pullPos)
        {
            Vector3 pullDir = pullPos - start.position;
            Vector3 targetDir = end.position - start.position;
            float maxLength = targetDir.magnitude;
            
            targetDir.Normalize();
            float pullValue = Vector3.Dot(pullDir, targetDir) / maxLength;
            return Mathf.Clamp(pullValue, 0, 1);
        }
    }
}