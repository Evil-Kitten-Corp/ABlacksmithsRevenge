using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tools
{
    public class VRBowController : MonoBehaviour
    {
        [SerializeField] private VRBowString bowString;
        [SerializeField] private Transform midPointGrabObject, midPointVisualObject, midPointParent;
        [SerializeField] private float bowStringStretchLimit = 0.3f;
        [SerializeField] private float stringSoundThreshold = 0.001f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickUpBow;
        [SerializeField] private AudioClip putDownBow;
        
        private XRGrabInteractable _interactable;
        private Transform _interactor;
        private float _strength, _previousStrength;
        
        public UnityEvent onBowPulled;
        public UnityEvent<float> onBowReleased;

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
                
                float midPointLocalZAbs = Mathf.Abs(midPointLocalSpace.x);

                _previousStrength = _strength;
                
                HandleStringPushedBack(midPointLocalSpace);
                HandleStringPulledBack(midPointLocalZAbs, midPointLocalSpace);
                HandlePulling(midPointLocalZAbs, midPointLocalSpace);
                
                bowString.CreateString(midPointVisualObject.position);
            }
        }

        private void HandlePulling(float midPointLocalZAbs, Vector3 midPointLocalSpace)
        {
            if (midPointLocalSpace.x < 0 && midPointLocalZAbs < bowStringStretchLimit)
            {
                if (audioSource.isPlaying == false && _strength <= 0.01f)
                {
                    audioSource.Play();
                }
                
                _strength = Remap(midPointLocalZAbs, 0, bowStringStretchLimit, 0, 1);
                midPointVisualObject.localPosition = new Vector3(midPointLocalSpace.x, 0, 0);

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
            if (midPointLocalSpace.x < 0 && midPointLocalZAbs >= bowStringStretchLimit)
            {
                Debug.Log("Pushed past limit");
                audioSource.Pause();
                _strength = 1;
                midPointVisualObject.localPosition = new Vector3(0, 0, -bowStringStretchLimit);
            }
        }

        private void HandleStringPushedBack(Vector3 midPointLocalSpace)
        {
            if (midPointLocalSpace.x >= 0)
            {
                Debug.Log("Pushed way too much");
                audioSource.pitch = 1;
                audioSource.Stop();
                _strength = 0;
                midPointVisualObject.localPosition = Vector3.zero;
            }
        }

        private void PrepareBowString(SelectEnterEventArgs arg0)
        {
            Debug.Log($"Pulling");
            _interactor = arg0.interactorObject.transform;
            onBowPulled?.Invoke();
        }

        private void ResetBowString(SelectExitEventArgs arg0)
        {
            Debug.Log("Resetting bow");
            onBowReleased?.Invoke(_strength);
            _strength = 0;
            _previousStrength = 0;
            audioSource.pitch = 1;
            audioSource.Stop();
            
            _interactor = null;
            midPointGrabObject.localPosition = Vector3.zero;
            midPointVisualObject.localPosition = Vector3.zero;
            bowString.CreateString(null);
        }

        public void PickUp()
        {
            audioSource.PlayOneShot(pickUpBow);
        }

        public void Drop()
        {
            audioSource.PlayOneShot(putDownBow);
        }
    }
}