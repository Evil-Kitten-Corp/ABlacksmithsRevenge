using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tools
{
    public class VRBowController : MonoBehaviour
    {
        [SerializeField] private VRBowString bowString;
        private XRGrabInteractable _interactable;

        [SerializeField] private Transform midPointGrabObject, midPointVisualObject, midPointParent;
        [SerializeField] private float bowStringStretchLimit = 0.3f;
        
        private Transform _interactor;

        private float _strength, _previousStrength;

        [SerializeField] private float stringSoundThreshold = 0.001f;
        [SerializeField] private AudioSource audioSource;

        public UnityEvent OnBowPulled;
        public UnityEvent<float> OnBowReleased;

        private void Awake()
        {
            _interactable = midPointGrabObject.GetComponent<XRGrabInteractable>();
        }

        private void Start()
        {
            _interactable.selectEntered.AddListener(PrepareBowString);
            _interactable.selectExited.AddListener(ResetBowString);
        }

        private void Update()
        {
            if (_interactor != null)
            {
                Vector3 midPointLocalSpace = midPointParent.InverseTransformPoint(midPointGrabObject.position);
                
                float midPointLocalZAbs = Mathf.Abs(midPointLocalSpace.z);

                _previousStrength = _strength;
                
                HandleStringPushedBack(midPointLocalSpace);
                HandleStringPulledBack(midPointLocalZAbs, midPointLocalSpace);
                HandlePulling(midPointLocalZAbs, midPointLocalSpace);
                
                bowString.CreateString(midPointVisualObject.position);
            }
        }

        private void HandlePulling(float midPointLocalZAbs, Vector3 midPointLocalSpace)
        {
            if (midPointLocalSpace.z < 0 && midPointLocalZAbs < bowStringStretchLimit)
            {
                if (audioSource.isPlaying == false && _strength <= 0.01f)
                {
                    audioSource.Play();
                }
                
                _strength = Remap(midPointLocalZAbs, 0, bowStringStretchLimit, 0, 1);
                midPointVisualObject.localPosition = new Vector3(0, 0, midPointLocalSpace.z);

                PlayStringPullingSound();
            }
        }

        private void PlayStringPullingSound()
        {
            if (Mathf.Abs(_strength - _previousStrength) > stringSoundThreshold)
            {
                if (_strength < _previousStrength)
                {
                    audioSource.pitch = -1;
                }
                else
                {
                    audioSource.pitch = 1;
                }
                
                audioSource.UnPause();
            }
            else
            {
                audioSource.Pause();
            }
        }

        private float Remap(float value, int fromMin, float fromMax, int toMin, int toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        private void HandleStringPulledBack(float midPointLocalZAbs, Vector3 midPointLocalSpace)
        {
            if (midPointLocalSpace.z < 0 && midPointLocalZAbs >= bowStringStretchLimit)
            {
                audioSource.Pause();
                _strength = 1;
                midPointVisualObject.localPosition = new Vector3(0, 0, -bowStringStretchLimit);
            }
        }

        private void HandleStringPushedBack(Vector3 midPointLocalSpace)
        {
            if (midPointLocalSpace.z >= 0)
            {
                audioSource.pitch = 1;
                audioSource.Stop();
                _strength = 0;
                midPointVisualObject.localPosition = Vector3.zero;
            }
        }

        private void PrepareBowString(SelectEnterEventArgs arg0)
        {
            _interactor = arg0.interactorObject.transform;
            OnBowPulled?.Invoke();
        }

        private void ResetBowString(SelectExitEventArgs arg0)
        {
            OnBowReleased?.Invoke(_strength);
            _strength = 0;
            _previousStrength = 0;
            audioSource.pitch = 1;
            audioSource.Stop();
            
            _interactor = null;
            midPointGrabObject.localPosition = Vector3.zero;
            midPointVisualObject.localPosition = Vector3.zero;
            bowString.CreateString(null);
        }
    }
}