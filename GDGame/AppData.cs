using Microsoft.Xna.Framework;

namespace GDGame
{
    /// <summary>
    /// Centralised, game-specific configuration and asset keys.
    /// </summary>
    public static class AppData
    {
        #region Physics
        public static readonly Vector3 GRAVITY = new Vector3(0, -9.81f, 0);
        #endregion

        #region Asset Paths
        public static readonly string CONTENT_ROOT = "Content";
        public static readonly string ASSET_MANIFEST_PATH =
            "assets/data/asset_manifest.json";
        public static readonly string SINGLE_MODEL_SPAWN_PATH =
            "assets/data/single_model_spawn.json";
        public static readonly string MULTI_MODEL_SPAWN_PATH =
            "assets/data/multi_model_spawn.json";
        public static readonly string GROUND_TEXTURE_KEY = "ground_grass";
        public static readonly string GROUND_TEXTURE_PATH =
            "assets/textures/foliage/ground/grass1";
        public static readonly string SKYBOX_BACK_TEXTURE_KEY = "skybox_back";
        public static readonly string SKYBOX_LEFT_TEXTURE_KEY = "skybox_left";
        public static readonly string SKYBOX_RIGHT_TEXTURE_KEY = "skybox_right";
        public static readonly string SKYBOX_FRONT_TEXTURE_KEY = "skybox_front";
        public static readonly string SKYBOX_SKY_TEXTURE_KEY = "skybox_sky";
        public static readonly string ENGLISH_CSV_PATH = "Localisation/english.csv";
        public static readonly string CZECH_CSV_PATH = "Localisation/czech.csv";
        public static readonly string UKRANIAN_CSV_PATH = "Localisation/ukranian.csv";
        #endregion

        #region GameObject Names
        public static readonly string GROUND_NAME = "ground";
        public static readonly string SKY_PARENT_NAME = "SkyParent";
        public static readonly string SKYBOX_BACK_NAME = "back";
        public static readonly string SKYBOX_LEFT_NAME = "left";
        public static readonly string SKYBOX_RIGHT_NAME = "right";
        public static readonly string SKYBOX_FRONT_NAME = "front";
        public static readonly string SKYBOX_SKY_NAME = "sky";
        public static readonly string HUD_NAME = "HUD";
        public static readonly string PLAYER_NAME = "Player";
        public static readonly string CAMERA_NAME = "Cam";
        public static readonly string USER_INTERFACE = "ui";
        public static readonly string INPUT_NAME = "user_input";
        public static readonly string RETICLE_NAME = "reticle";
        public static readonly string TRAP_NAME = "trap";
        public static readonly string FIRE_AUDIO_NAME = "fire_audio";
        public static readonly string CAT_AUDIO_NAME = "cat_audio";
        public static readonly string DUNGEON_AUDIO_NAME = "dungeon_audio";
        public static readonly string CINE_CAM_NAME = "cine_cam";
        public static readonly string MENU_CONTROLLER_NAME = "UI_menu_controller";
        public static readonly string AUDIO_PANEL_NAME = "UI_audio_panel";
        public static readonly string CONTROL_PANEL_NAME = "UI_control_panel";
        public static readonly string MAIN_PANEL_NAME = "UI_main_panel";
        #endregion

        #region Game
        public static readonly string GAME_WINDOW_TITLE = "Medieval Mayhem";
        public static readonly string START_SCENE_NAME = "Start";
        public static readonly string MAIN_SCENE_NAME = "Main Game";
        public static readonly string GAMEPAD_P1_NAME = "Gamepad P1";
        public static readonly string LOCALISATION_KEY_ERROR = "Key Not Found";
        public static readonly string FONT_NAME = "gamefont";
        public static readonly int RENDER_ORDER = 100;
        public static readonly int FORWARD_MOVE_NUM = 0;
        public static readonly int BACKWARD_MOVE_NUM = 1;
        public static readonly int RIGHT_MOVE_NUM = 2;
        public static readonly int LEFT_MOVE_NUM = 3;
        #endregion

        #region Textures
        public static readonly string BUTTON_TEXTURE = "button2";
        public static readonly string HYPHON_TEXTURE = "hyphon";
        public static readonly string TOGGLE_TEXTURE = "toggle";
        public static readonly string MAIN_MENU_TEXTURE = "home";
        public static readonly string AUDIO_MENU_TEXTURE = "audio";
        public static readonly string CONTROL_MENU_TEXTURE = "controls";
        public static readonly string SAND_TEXTURE = "Plane_sand_openPBR_shader1_BaseColor";
        #endregion

        #region Audio
        public static readonly string GAME_INTRO_KEY = "game_intro";
        public static readonly string OOF_KEY = "oof";
        public static readonly string RATTLE_KEY = "rattle";
        public static readonly string MAIN_THEME_KEY = "main_theme";
        public static readonly string MAIN_MUSIC = "test-music";
        public const string UI_CLICK_SOUND_KEY = "ui_click";
        #endregion

        #region User Interface
        public static readonly string PLAY_BUTTON_TEXT = "Play";
        public static readonly string PAUSE_BUTTON_TEXT = "Pause";
        public static readonly string CONTROLS_BUTTON_TEXT = "Controls";
        public static readonly string BACK_BUTTON_TEXT = "Back";
        public static readonly string QUIT_BUTTON_TEXT = "Quit";
        public static readonly string AUDIO_BUTTON_TEXT = "Audio";
        public static readonly string MUSIC_SLIDER_TEXT = "Music";
        public static readonly string SFX_SLIDER_TEXT = "SFX";
        public static readonly string UI_AUDIO_BG = "UI_audio_bg";
        public static readonly string UI_MAIN_BG = "UI_main_bg";
        public static readonly string UI_CONTROLS_BG = "UI_controls_bg";
        #endregion

        #region Localisation Keys
        public static readonly string LANG_PLAY_KEY = "Play";
        public static readonly string LANG_PAUSE_KEY = "Pause";
        public static readonly string LANG_SCORE_KEY = "Score";
        public static readonly string LANG_GAME_OVER_KEY = "GameOver";
        public static readonly string LANG_VICTORY_KEY = "Victory";
        public static readonly string LANG_TIME_KEY = "Time";
        public static readonly string LANG_ORB_KEY = "Orbs";
        public static readonly string LANG_HEALTH_KEY = "Health";
        #endregion

    }
}
