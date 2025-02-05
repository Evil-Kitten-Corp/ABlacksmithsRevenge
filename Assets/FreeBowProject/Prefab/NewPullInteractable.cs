using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace FreeBowProject.Prefab
{
    public class NewPullInteractable : XRBaseInteractable
    {
        public event Action<float> PullReleased;
        public event Action<float> PullUpdated;
        public event Action PullStarted;
        public event Action PullEnded;

        public Transform start, end, notch;

        private float pullAmount;
        
        public LineRenderer lineRenderer;
        private IXRSelectInteractor _pullingInteractor;

        public void SetPullInteractor(SelectEnterEventArgs args)
        {
            _pullingInteractor = args.interactorObject;
            PullStarted?.Invoke();
        }

        public void Release()
        {
            PullReleased?.Invoke(pullAmount);
            PullEnded?.Invoke();
            _pullingInteractor = null;
            pullAmount = 0;
            notch.localPosition = new Vector3(notch.localPosition.x, notch.localPosition.y, 0f);

            UpdateStringNotch();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            SetPullInteractor(args);
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected && _pullingInteractor != null)
                {
                    Vector3 pullPos = _pullingInteractor.GetAttachTransform(this).position;
                    float prevPull = pullAmount;
                    pullAmount = CalculatePull(pullPos);

                    if (prevPull != pullAmount)
                    {
                        PullUpdated?.Invoke(pullAmount);
                    }
                    
                    UpdateStringNotch();
                    HandleHaptics();
                }
            }
        }

        private void HandleHaptics()
        {
            if (_pullingInteractor is XRBaseInputInteractor controllerInt)
            {
                controllerInt.SendHapticImpulse(pullAmount, 0.1f);
            }
        }

        private float CalculatePull(Vector3 pullPos)
        {
            Vector3 pullDir = pullPos - start.position;
            Vector3 targetDir = end.position - start.position;
            float maxLength = targetDir.magnitude;
            
            targetDir.Normalize();
            float pullVal = Vector3.Dot(pullDir, targetDir) / maxLength;
            return Mathf.Clamp(pullVal, 0f, 1f);
        }

        public void UpdateStringNotch()
        {
            Vector3 linePos = Vector3.Lerp(start.localPosition, end.localPosition, pullAmount);
            notch.transform.localPosition = linePos;
            lineRenderer.SetPosition(1, linePos);
        }
    }
}
