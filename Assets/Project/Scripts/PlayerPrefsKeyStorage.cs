using System.Collections.Generic;

namespace Game
{
    public static class PlayerPrefsKeyStorage
    {
        public static string WINDOWSETTING = "Window_setting";
        public static string RESOLUTIONSETTING = "Resolution_setting";
        public static string QUALITYSETTING = "Quality_setting";
        public static string MASTERSOUNDSETTING = "Master_Sound_setting";
        public static string SFXSOUNDSETTING = "Sfx_Sound_setting";
        public static string MUSICSOUNDSETTING = "Music_Sound_setting";

        public static List<string> ALLSETTINGSKEYS = new List<string>
        {
            WINDOWSETTING,
            RESOLUTIONSETTING,
            QUALITYSETTING,
            MASTERSOUNDSETTING,
            SFXSOUNDSETTING,
            MUSICSOUNDSETTING
        };
    }
}