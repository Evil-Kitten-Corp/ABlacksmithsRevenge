using Placement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tools
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Shovel : MonoBehaviour
    {
        public InputActionReference useAction;
        private GameObject _heldObject;
        private GridManager _gridManager;

        private void Start() 
        {
            _gridManager = FindObjectOfType<GridManager>();
        }

        private void Update() 
        {
            if (useAction.action.triggered && _heldObject != null) 
            {
                RemoveTurret(_heldObject);
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

        void RemoveTurret(GameObject turret) 
        {
            Vector3 turretPos = turret.transform.position;
            _gridManager.ClearOccupied(turretPos);
            Destroy(turret);
        }
    }
}