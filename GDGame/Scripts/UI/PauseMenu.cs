using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Rendering;
using GDEngine.Core.Rendering.UI;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    /// <summary>
    /// Replaced by <see cref="<MenuController"/>. Unused
    /// </summary>
    public class PauseMenu
    {
        #region Fields
        private GameObject _pauseGO;
        private UIMenuPanel _pausePanel;
        private UIText _pauseText;
        private Color _pauseTextColour = Color.White;

        private bool _isVisible = false;
        private ContentDictionary<Texture2D> _textures;
        private SpriteFont _font;
        private Vector2 _screenCentre;
        #endregion

        public PauseMenu(ContentDictionary<Texture2D> textures, SpriteFont font, Vector2 centre)
        {
            _textures = textures;
            _font = font;
            _screenCentre = centre;
            
            Initialise();
        }

        public void Initialise()
        {
            _pauseGO = new GameObject("Pause");
            SceneController.AddToCurrentScene(_pauseGO);

            _pausePanel = new UIMenuPanel();
            _pauseGO.AddComponent(_pausePanel);
            _pausePanel.PanelPosition = _screenCentre;
            _pausePanel.AddButton("Resume", _textures.Get("button"), _font, HandlePauseToggle);
            _pausePanel.PanelPosition = _screenCentre + new Vector2(0,50);
            _pausePanel.AddButton("Quit", _textures.Get("button"), _font, HandlePauseToggle);

            _pauseText = new UIText
            {
                Color = _pauseTextColour,
                Font = _font,
                UniformScale = 2,
                LayerDepth = UILayer.HUD,
                TextProvider = () => LocalisationController.Instance.Get(AppData.LANG_PAUSE_KEY),
                PositionProvider = () => GetTextPos()
            };
            _pauseGO.AddComponent(_pauseText);


            _isVisible = false;
            _pausePanel.IsVisible = _isVisible;
            _pauseText.Enabled = _isVisible;

            EventChannelManager.Instance.InputEvents.OnPauseToggle.Subscribe(HandlePauseToggle);
        }

        private void HandlePauseToggle()
        {
            _isVisible = !_isVisible;
            _pausePanel.IsVisible = _isVisible;
            _pauseText.Enabled = _isVisible;
        }

        private void HandleExit()
        {
            Application.Exit();
        }

        private Vector2 GetTextPos()
        {
            return _screenCentre + new Vector2(0, -200);
        }

        private void HandleButtonClick()
        {
            Debug.WriteLine("Click");
        }
    }
}
