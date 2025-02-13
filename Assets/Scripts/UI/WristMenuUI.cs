using System;
using System.Collections.Generic;
using Data;
using Game_Systems;
using Placement;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class WristMenuUI : MonoBehaviour
    {
        public GameObject menuPanel; 
        public Button turretTabButton;
        public Button cameraTabButton;
        public GameObject turretTab;
        public GameObject cameraTab;
        
        public TextMeshProUGUI manaText; 
        public Transform buttonContainer; 
        public GameObject buttonPrefab;
        public float wristShakeThreshold = 2.0f;

        public InputActionReference input;
        public GameObject placingGameObject;

        private float _lastShakeTime;
        private Vector3 _lastAccel;
        private readonly List<Defense> _buyableUnits = new();
        private ManaManager _manaManager;
        private TurretPlacer _turretPlacer;
        
        private bool _placing;

        private void Start()
        {
            EnemyWave.OnRewardUnlocked += PopulateButtons;
             
            _turretPlacer = FindAnyObjectByType<TurretPlacer>();
            _turretPlacer.OnTurretPlaced += () =>
            {
                placingGameObject.SetActive(false);
                _placing = false;
            };
             
            placingGameObject.SetActive(false);
            
            
            _manaManager = ManaManager.instance;
            LoadBuyableUnits();
            menuPanel.SetActive(false);
            turretTabButton.onClick.AddListener(() => SwitchTab(true));
            cameraTabButton.onClick.AddListener(() => SwitchTab(false));
            PopulateButtons();
        }

        private void Update()
        {
            if (input.action.triggered)
            {
                ToggleMenu();
            }
            
            UpdateManaUI();
        }

        [Obsolete]
        private void DetectWristShake()
        {
            Vector3 accel = Input.acceleration;
            float shakeMagnitude = (accel - _lastAccel).magnitude;
        
            if (shakeMagnitude > wristShakeThreshold && Time.time - _lastShakeTime > 0.5f)
            {
                ToggleMenu();
                _lastShakeTime = Time.time;
            }
            
            _lastAccel = accel;
        }

        private void ToggleMenu()
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }
        
        private void LoadBuyableUnits()
        {
            _buyableUnits.Clear();
            _buyableUnits.AddRange(Resources.LoadAll<Defense>("Data"));
        }

        private void PopulateButtons()
        {
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
        
            foreach (var unit in _buyableUnits)
            {
                if (unit.unlocked)
                {
                    GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
                    Button button = buttonObj.GetComponent<Button>();
                    Image image = buttonObj.transform.GetChild(1).GetChild(0).GetComponentInChildren<Image>();
                    TextMeshProUGUI priceText = buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                
                    image.sprite = unit.icon;
                    button.onClick.AddListener(() => TryPurchaseUnit(unit));
                    priceText.text = unit.manaCost.ToString();
                }
            }
        }

        private void UpdateManaUI()
        {
            if (manaText == null)
            {
                Debug.Log("No mana text ig");
            }

            if (_manaManager == null)
            {
                Debug.Log("No mana manager ig");
            }
            else
            {
                manaText.text = "Mana: " + _manaManager.CurrentMana();
            }
            
            
            
            UpdateButtonStates();
        }

        private void TryPurchaseUnit(Defense unit)
        {
            if (_manaManager.CurrentMana() >= unit.manaCost)
            {
                _turretPlacer.StartPlacing(unit);
                UpdateButtonStates();
                placingGameObject.SetActive(true);
                _placing = true;
            }
        }

        private void UpdateButtonStates()
        {
            foreach (Transform child in buttonContainer)
            {
                TextMeshProUGUI costText = child.GetComponentInChildren<TextMeshProUGUI>();
                int cost = int.Parse(costText.text); 
                costText.color = _manaManager.CurrentMana() >= cost ? Color.white : Color.red;
            }
        }

        private void SwitchTab(bool toTurretTab)
        {
            turretTab.SetActive(toTurretTab);
            cameraTab.SetActive(!toTurretTab);

            switch (toTurretTab)
            {
                case true when _placing:
                    placingGameObject.SetActive(true);
                    break;
                case false when _placing:
                    placingGameObject.SetActive(false);
                    break;
            }
        }
    }
}
