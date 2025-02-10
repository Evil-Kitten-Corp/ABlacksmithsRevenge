using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {       
        [Header("References")]
        public GameObject pauseMenu;
        public ControllerInputActionManager controllerInputActionManager;

        [Header("Input Actions")]
        public InputActionReference pauseAction;

        private void Start()
        {
            pauseAction.action.performed += OnPauseUnpause;

            GameData.Instance.Pause += () =>
            {
                pauseMenu.SetActive(true);
                controllerInputActionManager.smoothMotionEnabled = false;
                controllerInputActionManager.smoothTurnEnabled = false;
            };

            GameData.Instance.Resume += () =>
            {
                pauseMenu.SetActive(false); 
                controllerInputActionManager.smoothMotionEnabled = true;
                controllerInputActionManager.smoothTurnEnabled = true;
            };
        }

        private void OnPauseUnpause(InputAction.CallbackContext obj)
        {
            GameData.Instance.OnPause();
        }
    }
}