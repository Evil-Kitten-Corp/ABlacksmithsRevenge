using Brains;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tools
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Hammer : MonoBehaviour
    {
        public InputActionReference useAction;
        public float repairPower = 20f;
        private GameObject _heldObject;

        private void Update() 
        {
            if (useAction.action.triggered && _heldObject != null) 
            {
                RepairTurret(_heldObject);
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

        void RepairTurret(GameObject turret) 
        {
            DefenseBrain health = turret.GetComponent<DefenseBrain>();
            
            if (health != null) 
            {
                health.Repair(repairPower);
            }
        }
    }
}