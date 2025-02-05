using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tools
{
    public class BowStringController : MonoBehaviour
    {
        [SerializeField] private BowStringUpdater bowString;
        [SerializeField] private Transform midPointGrabObject;
        
        private XRGrabInteractable _grabInteractable;
        private Transform _interactor;

        private void Awake()
        {
            _grabInteractable = midPointGrabObject.GetComponent<XRGrabInteractable>();
        }

        private void Start()
        {
            _grabInteractable.selectEntered.AddListener(PrepareBowString);
            _grabInteractable.selectExited.AddListener(ResetBowString);
        }

        private void PrepareBowString(SelectEnterEventArgs args)
        {
            _interactor = args.interactorObject.transform;
        }

        private void ResetBowString(SelectExitEventArgs args)
        {
            _interactor = null;
            midPointGrabObject.localPosition = Vector3.zero; 
            bowString.CreateString();
        }

        private void Update()
        {
            if (_interactor != null)
            {
                bowString.CreateString(midPointGrabObject.transform.position);
            }
        }
    }
}