using Game.Inputs;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Managers
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : SingletonDontDestroy<GameManager>
    {
        //Variables
        public SceneIndexValueToLoad SceneIndexValueToLoad;
        public AudioMixer Mixer;
        public GameObject Player;

        public DefaultInput Inputs { get; private set; }
        public Vector2 MousePosition { get => Mouse.current.position.ReadValue(); }

        private void OnEnable()
        {
            Inputs = new DefaultInput();
            Inputs.UI.Enable();
        }

        private void Start()
        {
            ApplySettings();
        }

        #region Apply Settings
        private void ApplySettings()
        {
            LoadWindowsSetting();
            LoadResolusionSetting();
            LoadSoundSetting();
        }

        #region LoadSettings

        private void LoadWindowsSetting()
        {
            int value = PlayerPrefs.GetInt(PlayerPrefsKeyStorage.WINDOWSETTING, Screen.fullScreen == true ? 1 : 0);

            switch (value)
            {
                case 0:
                    SetWindowSetting(false);
                    break;
                case 1:
                    SetWindowSetting(true);
                    break;
                default:
                    SetWindowSetting(true);
                    break;
            }
        }

        private void LoadResolusionSetting()
        {
            string res = PlayerPrefs.GetString(PlayerPrefsKeyStorage.RESOLUTIONSETTING, $"{Screen.currentResolution.width}x{Screen.currentResolution.height}");

            SetResolutionSetting(res);
        }

        private void LoadSoundSetting()
        {
            float masterValue = PlayerPrefs.GetFloat(PlayerPrefsKeyStorage.MASTERSOUNDSETTING, .5f);
            float sfxValue = PlayerPrefs.GetFloat(PlayerPrefsKeyStorage.SFXSOUNDSETTING, .5f);
            float musicValue = PlayerPrefs.GetFloat(PlayerPrefsKeyStorage.MUSICSOUNDSETTING, .5f);

            SetMasterSetting(masterValue);
            SetSfxSetting(sfxValue);
            SetMusicSetting(musicValue);
        }

        private void LoadQualityLevel()
        {
            int value = PlayerPrefs.GetInt(PlayerPrefsKeyStorage.QUALITYSETTING, 1);
            SetQualityLevel(value);
        }


        #endregion

        #region SetSettings

        private void SetWindowSetting(bool value)
        {
            Screen.fullScreen = value;
        }

        private void SetResolutionSetting(string value)
        {
            string[] tmp = value.Split('x');
            Screen.SetResolution(int.Parse(tmp[0]), int.Parse(tmp[1]), Screen.fullScreen);
        }

        private void SetMasterSetting(float value)
        {
            Mixer.SetFloat("MASTER", Mathf.Log10(value) * 10f);
        }

        private void SetSfxSetting(float value)
        {
            Mixer.SetFloat("SFX", Mathf.Log10(value) * 10f);
        }

        private void SetMusicSetting(float value)
        {
            Mixer.SetFloat("MUSIC", Mathf.Log10(value) * 10f);
        }

        private void SetQualityLevel(int value)
        {
            QualitySettings.SetQualityLevel(value);
        }

        #endregion
        #endregion
    }
}
