using GDEngine.Core.Entities;
using GDGame.Scripts.Systems;

namespace GDGame.Scripts.Traps
    {
    /// <summary>
    /// Base Abstract class all traps derive from.
    /// </summary>
        public abstract class TrapBase
        {
            #region Fields
            protected GameObject _trapGO;
            protected GameObject _trapModelGO;
            private int _trapID;
            #endregion

            #region Constructors
            public TrapBase(int id)
            {
                _trapID = id;
                
            }
            #endregion

            #region Accessors
            public GameObject TrapGO => _trapGO;
            #endregion

            #region Methods
            public abstract void InitTrap();
            public abstract void UpdateTrap();
            public abstract void HandlePlayerHit();
            #endregion
        }
    }
