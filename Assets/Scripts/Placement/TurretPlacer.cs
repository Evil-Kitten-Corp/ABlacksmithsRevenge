using System.Linq;
using Brains;
using Data;
using Game_Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Placement
{
    public class TurretPlacer : MonoBehaviour
    {
        public Transform vrController;
        public InputActionReference placeAction;
        
        private GridManager _gridManager;
        private GameObject _previewTurret;
        private Defense _placingTurret;

        private void Start() 
        {
            _gridManager = FindAnyObjectByType<GridManager>();
        }
        
        public void StartPlacing(Defense turret) 
        {
            if (!ManaManager.instance.SpendMana(turret.manaCost))
            {
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
                if (Physics.Raycast(vrController.position, vrController.forward, 
                        out RaycastHit hit, 10f)) 
                {
                    Vector3 closestGridPos = _gridManager.GetClosestGridPosition(hit.point);
                    _previewTurret.transform.position = closestGridPos;
                }

                if (placeAction.action.triggered) 
                {
                    PlaceTurret();
                }
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
                Destroy(_previewTurret);
                _previewTurret = null;
            }
        }
    }
}