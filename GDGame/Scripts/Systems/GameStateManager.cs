using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDGame.Scripts.Events.Channels;

namespace GDGame.Scripts.Systems
{
    public enum GameState { GameActive, GameInactive, GameOver, GameWon }

    /// <summary>
    /// Controls the current state of the game and runs needed events when called 
    /// depending on the current <see cref="GameState"/>.
    /// </summary>
    public class GameStateManager : Component
    {
        #region Fields
        private GameObject _gameStateManagerGO;
        private GameState _gameState;
        #endregion

        #region Constructors
        public GameStateManager() 
        {
            _gameState = GameState.GameInactive;
            _gameStateManagerGO = new GameObject("GSM");
            _gameStateManagerGO.AddComponent(this);
            SceneController.AddToCurrentScene(_gameStateManagerGO);
            EventChannelManager.Instance.PlayerEvents.OnGameStateChange.Subscribe(HandleGameStateChange);
        }
        #endregion

        #region Accessors
        public GameState CurrentState => _gameState;
        #endregion

        #region Methods
        private void HandleGameStateChange(GameState state)
        {
            _gameState = state;
            CheckNewGameState();
        }

        private void CheckNewGameState()
        {
            // TODO: Add Events for each section
            switch (CurrentState)
            {
                case GameState.GameActive:
                    Debug.WriteLine("Game Active");
                    break;
                case GameState.GameInactive:
                    Debug.WriteLine("Game Inactive");
                    break;
                case GameState.GameOver:
                    Debug.WriteLine("Game Over");
                    break;
                case GameState.GameWon:
                    Debug.WriteLine("Game Won");
                    break;
            }
        }
        #endregion

    }
}
