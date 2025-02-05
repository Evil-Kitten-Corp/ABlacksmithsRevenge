using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tools
{
    public class Bow : XRBaseInteractable
    {
        public static event Action<float> PullActionReleased;
        
        [SerializeField] private Transform start;
        [SerializeField] private Transform end;
        [SerializeField] private LineRenderer bowString;

        public Transform cube;

        private float _pullVal;
        private IXRSelectInteractor _interactor;

        public void SetPull(SelectEnterEventArgs args)
        {
            Debug.Log("SetPull");
            _interactor = args.interactorObject;
        }

        public void Release()
        {
            Debug.Log("Release");
            PullActionReleased?.Invoke(_pullVal);
            _interactor = null;
            _pullVal = 0;
            cube.position = new Vector3(cube.localPosition.x, cube.localPosition.y, 0f);
            UpdateString();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected)
                {
                    Debug.Log("Bow: ProcessInteractable");
                    Vector3 pullPos = _interactor.transform.position;
                    _pullVal = CalculatePull(pullPos);
                    UpdateString();
                }
            }
        }

        private void UpdateString()
        {
            Vector3 linePos = Vector3.forward *
                              Mathf.Lerp(start.transform.localPosition.z, end.transform.localPosition.z, _pullVal);
            cube.transform.position = new Vector3(cube.localPosition.x, cube.localPosition.y, linePos.z + .2f);
            bowString.SetPosition(1, linePos);
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