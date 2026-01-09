using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Entities;
using GDEngine.Core.Rendering;
using GDEngine.Core.Rendering.UI;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Player;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    /// <summary>
    /// Controls the Player HUD which displays stats and messages on the screen.
    /// Created in <see cref="UserInterfaceController"/>.
    /// </summary>
    public class PlayerHUD : IDisposable
    {
        #region Fields
        private PlayerStats _playerStats;
        private SpriteFont _hudFont;
        private Color _hudTextColour = Color.White;
        private List<UIText> _textObjects;
        private bool _isVisible;
        private bool disposedValue;
        private readonly Dictionary<string, Vector2> _hudPositions = new()
        {
            ["top_left"] = new Vector2(200, 100),
            ["top_right"] = new Vector2(1500, 100),
            ["vert_increment"] = new Vector2(0, 50),
            ["hor_increment"] = new Vector2(285, 0),
            ["health"] = new Vector2(200, 230),
            ["orbs"] = new Vector2(200,260),
            ["objective"] = new Vector2(1500, 200),
            ["message"] = new Vector2(800,800)
        };
        #endregion

        #region Constructors
        public PlayerHUD(SpriteFont hudFont, PlayerStats stats)
        {
            _hudFont = hudFont;
            _playerStats = stats;
            _textObjects = new();
            _isVisible = false;

        }
        #endregion

        #region Methods

        /// <summary>
        /// Create a text object to add to the scene.
        /// Takes in a key to fetch the translation for from the localisation controller.
        /// </summary>
        /// <param name="key">Text to display Key</param>
        /// <param name="pos">Position on the screen</param>
        private void CreateText(string key, Vector2 pos)
        {
            var textGO = new GameObject($"Text Object: {key}");
            var uiText = new UIText
            {
                Color = _hudTextColour,
                Font = _hudFont,
                LayerDepth = UILayer.HUD,
                TextProvider = () => LocalisationController.Instance.Get(key),
                PositionProvider = () => pos
            };

            textGO.AddComponent(uiText);  
            _textObjects.Add(uiText);
            SceneController.AddToCurrentScene(textGO);
        }

        /// <summary>
        /// Creates the Orb Stat counter in the top left.
        /// Gets the number from the Player Stats script.
        /// </summary>
        /// <param name="pos">Position on Screen</param>
        private void CreateOrbStat(Vector2 pos)
        {
            var textGO = new GameObject($"Text Object: Orbs");
            var uiText = new UIText
            {
                Color = _hudTextColour,
                Font = _hudFont,
                LayerDepth = UILayer.HUD,
                TextProvider = () => _playerStats.OrbsCollected.ToString(),
                PositionProvider = () => pos
            };

            textGO.AddComponent(uiText);
            _textObjects.Add(uiText);
            SceneController.AddToCurrentScene(textGO);
        }

        /// <summary>
        /// Creates the current health stats in the top left of the screen.
        /// Gets the number from the Player Stats script.
        /// </summary>
        /// <param name="pos">Position on the Screen</param>
        private void CreateHealthStat(Vector2 pos)
        {
            var textGO = new GameObject($"Text Object: Health");
            var uiText = new UIText
            {
                Color = _hudTextColour,
                Font = _hudFont,
                LayerDepth = UILayer.HUD,
                TextProvider = () => _playerStats.CurrentHealth.ToString(),
                PositionProvider = () => pos
            };

            textGO.AddComponent(uiText);
            _textObjects.Add(uiText);
            SceneController.AddToCurrentScene(textGO);
        }

        // <summary>
        /// Creates the timer display in the top right of the screen.
        /// </summary>
        /// <param name="pos">Position on Screen</param>
        private void CreateTimerStat(Vector2 pos)
        {
            var textGO = new GameObject($"Text Object: Timer");
            var uiText = new UIText
            {
                Color = _hudTextColour,
                Font = _hudFont,
                LayerDepth = UILayer.HUD,
                TextProvider = () => _playerStats.TimeLeft,
                PositionProvider = () => pos
            };

            textGO.AddComponent(uiText);
            _textObjects.Add(uiText);
            SceneController.AddToCurrentScene(textGO);
        }

        /// <summary>
        /// Get a Vector2 Position from the Positions Dictionary
        /// </summary>
        /// <param name="key">Position Key</param>
        /// <returns>Keys Vector2 Position</returns>
        /// <exception cref="Exception">Position not found</exception>
        private Vector2 GetPos(string key)
        {
            if (_hudPositions.TryGetValue(key, out var result))
                return result;
            else
                throw new Exception($"{key}: not found in HUD Position Dictionary");
        }

        /// <summary>
        /// Creates the HUD Text in the top left of the screen
        /// </summary>
        private void InitHUDText()
        {
            var startPos = GetPos("top_left");
            var vertIncrement = GetPos("vert_increment");
            var horIncrement = GetPos("hor_increment");

            CreateTimerStat(GetPos("top_right"));
            CreateText(AppData.LANG_HEALTH_KEY, startPos);
            CreateHealthStat(startPos + horIncrement);
            CreateText(AppData.LANG_ORB_KEY, startPos += vertIncrement);
            CreateOrbStat(startPos + horIncrement);
        }
        public void Initialise()
        {
            InitHUDText();
        }
        private void Clear()
        {
            _playerStats = null;
            _textObjects = null;
            _hudFont = null;
                
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing)
                Clear();

            disposedValue = true;
        }

        ~PlayerHUD()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
