using Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    internal class MainMenu : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button StartButton;
        [SerializeField] private Button LoadButton;
        [SerializeField] private Button OptionsButton;
        [SerializeField] private Button ExitButton;

        [Header("Panels")]
        [SerializeField] private GameObject MainPanel;
        [SerializeField] private GameObject OptionPanel;

        private void Start ()
        {
            StartButton.onClick.AddListener(NewGame);
            ExitButton.onClick.AddListener(Exit);

            //TO DO
            //check is save, if player don't have save, load new game
            LoadButton.onClick.AddListener(NewGame);

            OptionsButton.onClick.AddListener(Option);

        }

        #region Buttons Method

        private void NewGame()
        {
            GameManager.Instance.SceneIndexValueToLoad.index = 2;
            SceneManager.LoadScene(1);
        }

        private void Option()
        {
            MainPanel.SetActive(false);
            OptionPanel.SetActive(true);
        }

        private void Exit()
        {
            Application.Quit();
        }

        #endregion
    }
}
