using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Tools
{
    public class ConstrainedGrabMovement : XRGrabInteractable
    {
        [Header("Movement Bounds")] 
        public Transform start, end;
        public BowStringUpdater bowStringUpdater;

        private IXRSelectInteractor _interactor;
        private Quaternion _initialRotation;
        private Vector3 _initialLocalOffset;

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            // Calculate the offset between the object's position and the interactor's grab position
            _initialLocalOffset = transform.position - args.interactorObject.transform.position;
            _initialRotation = transform.rotation;
            
            _interactor = args.interactorObject;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (isSelected)
            {
                if (_interactor == null) return;

                // Target position based on the interactor's movement
                Vector3 targetPosition = _interactor.transform.position + _initialLocalOffset;

                // Clamp X position within limits
                targetPosition.x = Mathf.Clamp(targetPosition.z, start.position.z, end.position.z);

                // Keep Y and Z the same (no movement in those axes)
                targetPosition.y = transform.position.y;
                targetPosition.x = transform.position.x;

                // Apply the constrained position
                transform.position = targetPosition;
                transform.rotation = _initialRotation;

                bowStringUpdater.CreateString(targetPosition); 
            }
        }
    }
}