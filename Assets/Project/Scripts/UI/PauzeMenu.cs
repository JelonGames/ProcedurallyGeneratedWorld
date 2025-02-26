using Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class PauzeMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauzeMenu;
        [SerializeField] private GameObject optionMenu;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button exitButton;

        private void OnEnable()
        {
            GameManager.Instance.Inputs.UI.Cancel.performed += OpenMenu;
        }

        private void Start()
        {
            continueButton.onClick.AddListener(OpenMenu);
            exitButton.onClick.AddListener(Exit);
            optionsButton.onClick.AddListener(OpenOption);
        }

        private void OnDisable()
        {
            GameManager.Instance.Inputs.UI.Cancel.performed -= OpenMenu;
        }

        private void OpenMenu(InputAction.CallbackContext context) => OpenMenu();

        private void OpenMenu()
        {
            pauzeMenu.SetActive(!pauzeMenu.activeInHierarchy);
            
            if (pauzeMenu.activeInHierarchy)
                EventSystem.current.SetSelectedGameObject(continueButton.gameObject);

            if (pauzeMenu.activeInHierarchy)
                GameManager.Instance.Inputs.Player.Disable();
            else
                GameManager.Instance.Inputs.Player.Enable();
        }

        private void OpenOption()
        {
            pauzeMenu.SetActive(false);
            optionMenu.SetActive(true);
        }

        private void Exit()
        {
            GameManager.Instance.SceneIndexValueToLoad.index = 0;
            SceneManager.LoadScene(1);
        }
    }
}
