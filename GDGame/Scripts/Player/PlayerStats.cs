using GDEngine.Core.Timing;
using System.Diagnostics;

namespace GDGame.Scripts.Player
{
    public class PlayerStats
    {
        #region Fields
        private readonly int _startHealth = 100;
        private readonly float _startingTimeInSeconds = 600f;
        private int _currentHealth;
        private int _orbsCollected;
        private float _timeRemaining;
        private bool _isTimerRunning = false;
        #endregion

        #region Accessors
        public int CurrentHealth => _currentHealth;
        public int OrbsCollected => _orbsCollected;
        public string TimeLeft
        {
            get
            {
                int minutes = (int)(_timeRemaining / 60);
                int seconds = (int)(_timeRemaining % 60);
                return "Time Remaining: " + $"{minutes:D2}:{seconds:D2}"; 
            }
        }

        public bool IsTimeUp => _timeRemaining <= 0;
        #endregion

        #region Constructors
        public PlayerStats() { }
        #endregion

        #region Methods

        /// <summary>
        /// Set the health to the starter health amount and set orbs collected to 0
        /// </summary>
        public void Initialise()
        {
            _currentHealth = _startHealth;
            _orbsCollected = 0;
            _timeRemaining = _startingTimeInSeconds;
            _isTimerRunning = false;
        }

        /// <summary>
        /// Start the timer countdown
        /// </summary>
        public void StartTimer()
        {
            if (!_isTimerRunning)
            {
                
                if (_timeRemaining <= 0 || _timeRemaining >= _startingTimeInSeconds)
                    _timeRemaining = _startingTimeInSeconds;

                _isTimerRunning = true;
            }
        }

        /// <summary>
        /// Remove health from the player by a given amount
        /// </summary>
        /// <param name="amount">Health to remove from players current health</param>
        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;

            if (_currentHealth > 0) return;

            Debug.WriteLine("gg");
        }

        /// <summary>
        /// Increment the players orb collected count by 1
        /// </summary>
        public void HandleOrbCollection()
        {
            _orbsCollected++;
        }

        /// <summary>
        /// Handle the time countdown if timer has been started
        /// </summary>
        public void HandleTimeCountdown()
        {
            if (!_isTimerRunning || _timeRemaining <= 0)
                return;

            _timeRemaining -= Time.DeltaTimeSecs;

            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
            }
        }

        /// <summary>
        /// Add extra time to the timer
        /// </summary>
        /// <param name="seconds">Seconds to add</param>
        public void AddTime(float seconds)
        {
            _timeRemaining += seconds;
        }

        #endregion
    }
}