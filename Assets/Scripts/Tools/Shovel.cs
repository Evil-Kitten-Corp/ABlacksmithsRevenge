using System.Linq;
using Brains;
using Placement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tools
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Shovel : MonoBehaviour
    {
        public InputActionReference[] useActions;
        
        public AudioSource audioSource;
        public AudioClip[] useSounds;
        
        private GameObject _heldObject;

        private void Start() 
        {
            FindAnyObjectByType<GridManager>();
        }

        private void Update() 
        {
            if (useActions.Any(x => x.action.triggered) && _heldObject != null) 
            {
                RemoveTurret(_heldObject.GetComponent<DefenseBrain>());
            }
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.CompareTag("Turret")) 
            {
                _heldObject = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other) 
        {
            if (other.gameObject == _heldObject) 
            {
                _heldObject = null;
            }
        }

        void RemoveTurret(DefenseBrain turret)
        {
            turret.TryDestroy();
            audioSource.PlayOneShot(useSounds[Random.Range(0, useSounds.Length)]);
        }
    }
}