using System.Linq;
using AYellowpaper;
using Brains;
using Data;
using Game_Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

namespace Placement
{
    public class TurretPlacer : MonoBehaviour
    {
        public InputActionReference[] placeActions;
        public InterfaceReference<IXRRayProvider> rayProvider;
        
        private GridManager _gridManager;
        [Header("DEBUG ONLY")] [SerializeField] private GameObject _previewTurret;
        private Defense _placingTurret;

        private void Start() 
        {
            _gridManager = FindAnyObjectByType<GridManager>();
        }
        
        public void StartPlacing(Defense turret) 
        {
            if (_previewTurret != null)
            {
                Debug.Log("Can't place new turret.");
                return;
            }
            
            if (!ManaManager.instance.SpendMana(turret.manaCost))
            {
                Debug.Log("Not enough mana.");
                return;
            }
            
            _placingTurret = turret;
            _previewTurret = Instantiate(turret.prefab);
            _previewTurret.GetComponentInChildren<Collider>().enabled = false;
            _previewTurret.GetComponentInChildren<Renderer>().material.color = new Color(0, 1, 0, 0.5f);
        }

        private void Update() 
        {
            if (_previewTurret)
            {
                if (rayProvider != null)
                {
                    Vector3 closestGridPos = _gridManager.GetClosestGridPosition(rayProvider.Value.rayEndPoint);
                    _previewTurret.transform.position = closestGridPos;
                }
                
                // if (Physics.Raycast(vrController.position, vrController.forward, 
                //         out RaycastHit hit, 10f)) 
                // {
                //     Vector3 closestGridPos = _gridManager.GetClosestGridPosition(hit.point);
                //     _previewTurret.transform.position = closestGridPos;
                // }

                if (placeActions.Any(input => input.action.triggered))
                {
                    PlaceTurret();
                }
                
                // if (placeAction.action.triggered) 
                // {
                //     PlaceTurret();
                // }
            }
        }

        private void PlaceTurret() 
        {
            Vector3 placementPos = _previewTurret.transform.position;
            
            Collider[] colliders = Physics.OverlapSphere(placementPos, 0.5f);
            
            if (colliders.Any(col => col.CompareTag("Enemy")))
            {
                Debug.Log("Enemy blocking placement!");
                return;
            }
    
            if (!_gridManager.IsPositionOccupied(placementPos)) 
            {
                GameObject myTurret = Instantiate(_placingTurret.prefab, _previewTurret.transform.position, Quaternion.identity);
                myTurret.GetComponent<DefenseBrain>().AssignDefense(_placingTurret);
                _gridManager.SetOccupied(placementPos, myTurret);
                Debug.Log("Purchased: " + _placingTurret.name);
                Destroy(_previewTurret);
                _previewTurret = null;
            }
        }
    }
}