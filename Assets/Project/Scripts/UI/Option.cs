using Game.Managers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    internal class Options : MonoBehaviour
    {
        [Header("Change Page Buttons")]
        [SerializeField] private Button generalButtonPage;
        [SerializeField] private Button soundButtonPage;

        [Header("Pages")]
        [SerializeField] private GameObject generalPage;
        [SerializeField] private GameObject soundPage;

        private GameObject activePage;

        [Header("General Contorls Page")]
        [SerializeField] private Toggle windowToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("Sound Contorls Page")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;

        [Header("Controls Buttons")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button resetButton;

        [Header("Other")]
        [SerializeField] private GameObject masterPanel;

        private string MASTER_VALUE { get => "MASTER"; }
        private string SFX_VALUE { get => "SFX"; }
        private string MUSIC_VALUE { get => "MUSIC"; }

        private GameManager gameManager;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(backButton.gameObject);
            gameManager = GameManager.Instance;

            ChangePage(0);
            LoadSettins();
            EnableButtons();
        }

        private void OnDisable()
        {
            DisableButtons();
        }

        private void DisableButtons()
        {
            // Page Buttons
            generalButtonPage.onClick.RemoveAllListeners();
            soundButtonPage.onClick.RemoveAllListeners();

            // General Triggers
            windowToggle.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            qualityDropdown.onValueChanged.RemoveAllListeners();

            // Sound Triggers
            masterSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();

            // Control Buttons
            resetButton.onClick.RemoveAllListeners();
            saveButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        private void SaveSettings()
        {
            SaveWindowSetting();
            SaveResolutionSetting();
            SaveSoundSetting();
            SaveQualityLevel();
        }
        
        private void EnableButtons()
        {
            // Page Buttons
            generalButtonPage.onClick.AddListener(() => ChangePage(0));
            soundButtonPage.onClick.AddListener(() => ChangePage(1));

            // General Triggers
            windowToggle.onValueChanged.AddListener(SetWindowSetting);
            resolutionDropdown.onValueChanged.AddListener(SetResolutionSetting);
            qualityDropdown.onValueChanged.AddListener(SetQualityLevel);

            // Sound Triggers
            masterSlider.onValueChanged.AddListener(SetMasterSetting);
            sfxSlider.onValueChanged.AddListener(SetSfxSetting);
            musicSlider.onValueChanged.AddListener(SetMusicSetting);

            // Contol Buttons
            resetButton.onClick.AddListener(ResetSettings);
            saveButton.onClick.AddListener(SaveSettingBtn);
            backButton.onClick.AddListener(BackToMenu);
        }

        private void LoadSettins()
        {
            LoadWindowsSetting();
            LoadResolusionSetting();
            LoadSoundSetting();
            LoadQualityLevel();
        }

        #region LoadSettings

        private void LoadWindowsSetting()
        {
            int value = PlayerPrefs.GetInt(PlayerPrefsKeyStorage.WINDOWSETTING, Screen.fullScreen == true ? 1 : 0);
            windowToggle.isOn = value == 0 ? false : true;
            windowToggle.GraphicUpdateComplete();

            switch (value)
            {
                case 0:
                    SetWindowSetting(false);
                    windowToggle.isOn = false;
                    break;
                case 1:
                    SetWindowSetting(true);
                    windowToggle.isOn = true;
                    break;
                default:
                    SetWindowSetting(true);
                    windowToggle.isOn = true;
                    break;
            }
        }

        private void LoadResolusionSetting()
        {
            string res = PlayerPrefs.GetString(PlayerPrefsKeyStorage.RESOLUTIONSETTING, $"{Screen.currentResolution.width}x{Screen.currentResolution.height}");

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(GetResolutions().ToList());
            
            for (int i = 0; i < resolutionDropdown.options.Count; i++)
            {
                if(string.Equals(res, resolutionDropdown.options[i].text))
                {
                    resolutionDropdown.value = i;
                    break;
                }
            }

            resolutionDropdown.RefreshShownValue();

            SetResolutionSetting(resolutionDropdown.value);
        }

        private void LoadSoundSetting()
        {
            float masterValue = PlayerPrefs.GetFloat(PlayerPrefsKeyStorage.MASTERSOUNDSETTING, .5f);
            float sfxValue = PlayerPrefs.GetFloat(PlayerPrefsKeyStorage.SFXSOUNDSETTING, .5f);
            float musicValue = PlayerPrefs.GetFloat(PlayerPrefsKeyStorage.MUSICSOUNDSETTING, .5f);

            masterSlider.value = masterValue;
            sfxSlider.value = sfxValue;
            musicSlider.value = musicValue;

            //masterSlider.GraphicUpdateComplete();
            //sfxSlider.GraphicUpdateComplete();
            //musicSlider.GraphicUpdateComplete();

            SetMasterSetting(masterValue);
            SetSfxSetting(sfxValue);
            SetMusicSetting(musicValue);
        }

        private void LoadQualityLevel()
        {
            int value = PlayerPrefs.GetInt(PlayerPrefsKeyStorage.QUALITYSETTING, 1);

            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(GetGraficQualityLevels().ToList());
            qualityDropdown.value = value;
            qualityDropdown.RefreshShownValue();

            SetQualityLevel(value);
        }

        #endregion

        #region SetSettings

        private void SetWindowSetting(bool value)
        {
            Screen.fullScreen = value;
        }

        private void SetResolutionSetting(int value)
        {
            string[] tmp = resolutionDropdown.options[value].text.Split('x');
            Screen.SetResolution(int.Parse(tmp[0]), int.Parse(tmp[1]), Screen.fullScreen);
        }

        private void SetMasterSetting(float value)
        {
            gameManager.Mixer.SetFloat(MASTER_VALUE, Mathf.Log10(value) * 10f);
        }

        private void SetSfxSetting(float value)
        {
            gameManager.Mixer.SetFloat(SFX_VALUE, Mathf.Log10(value) * 10f);
        }

        private void SetMusicSetting(float value)
        {
            gameManager.Mixer.SetFloat(MUSIC_VALUE, Mathf.Log10(value) * 10f);
        }

        private void SetQualityLevel(int value)
        {
            QualitySettings.SetQualityLevel(value);
        }

        #endregion

        #region SaveSettings

        private void SaveWindowSetting()
        {
            if (Screen.fullScreen)
                PlayerPrefs.SetInt(PlayerPrefsKeyStorage.WINDOWSETTING, 1);
            else
                PlayerPrefs.SetInt(PlayerPrefsKeyStorage.WINDOWSETTING, 0);
        }

        private void SaveResolutionSetting()
        {
            PlayerPrefs.SetString(PlayerPrefsKeyStorage.RESOLUTIONSETTING, $"{Screen.width}x{Screen.height}");
        }

        private void SaveSoundSetting()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeyStorage.MASTERSOUNDSETTING, masterSlider.value);
            PlayerPrefs.SetFloat(PlayerPrefsKeyStorage.MUSICSOUNDSETTING, musicSlider.value);
            PlayerPrefs.SetFloat(PlayerPrefsKeyStorage.SFXSOUNDSETTING, sfxSlider.value);
        }

        private void SaveQualityLevel()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeyStorage.QUALITYSETTING, qualityDropdown.value);
        }

        #endregion

        #region Get Values to Settings

        private HashSet<string> GetResolutions()
        {
            Resolution[] allResolutions = Screen.resolutions;
            HashSet<string> uniqueReslutions = new HashSet<string>();

            foreach (Resolution res in allResolutions)
            {
                string stringRes = $"{res.width}x{res.height}";
                if (!uniqueReslutions.Contains(stringRes))
                {
                    uniqueReslutions.Add(stringRes);
                }
            }

            return uniqueReslutions;
        }

        private string[] GetGraficQualityLevels() => QualitySettings.names;

        #endregion

        #region Button Methods

        private void ChangePage(int indexPage)
        {
            if(activePage != null)
                activePage.SetActive(false);

            switch (indexPage)
            {
                case 0: // General page
                    generalPage.SetActive(true);
                    activePage = generalPage;
                    break;
                case 1: // Sound page
                    soundPage.SetActive(true);
                    activePage = soundPage;
                    break;
                default:
                    generalPage.SetActive(true);
                    activePage = generalPage;
                    break;
            }
        }

        private void ResetSettings()
        {
            foreach(string key in PlayerPrefsKeyStorage.ALLSETTINGSKEYS)
            {
                PlayerPrefs.DeleteKey(key);
            }

            LoadSettins(); // now load default value settings
        }

        private void SaveSettingBtn()
        {
            SaveSettings();
            BackToMenu();
        }

        private void BackToMenu()
        {
            masterPanel.SetActive(true);

            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);

            this.gameObject.SetActive(false);
        }

        #endregion
    }
}