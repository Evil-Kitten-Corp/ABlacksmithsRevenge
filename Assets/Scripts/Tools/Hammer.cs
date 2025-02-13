using System.Linq;
using Brains;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tools
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Hammer : MonoBehaviour
    {
        public InputActionReference[] useActions;
        public float repairPower = 20f;
        public float cooldownTime = 10f;
        
        public AudioSource audioSource;
        public AudioClip[] repairSounds;
        
        private GameObject _heldObject;
        private float _nextUseTime;

        private void Update() 
        {
            if (Time.time >= _nextUseTime && useActions.Any(x => x.action.triggered) && _heldObject != null) 
            {
                RepairTurret(_heldObject);
                _nextUseTime = Time.time + cooldownTime;
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
                Debug.Log($"Repaired turret {health.name}");
                audioSource.PlayOneShot(repairSounds[Random.Range(0, repairSounds.Length)]);
            }
        }
    }
}