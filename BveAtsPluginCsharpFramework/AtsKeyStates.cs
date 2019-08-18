using System;
using System.Linq;

namespace AtsPlugin
{
    [Serializable]
    public class AtsKeyStates
    {
        [Serializable]
        public class KeyState
        {
            [Serializable]
            public enum StateType
            {
                Up = 0,
                Down,
            }

            private StateType _state = StateType.Up;

            public StateType State { get => _state; }

            
            public bool IsDown
            {
                get
                {
                    return (State == StateType.Down);
                }
            }

            public bool IsUp
            {
                get
                {
                    return (State == StateType.Up);
                }
            }

            internal void SetState(StateType state)
            {
                _state = state;
            }

            internal void CopyFrom(KeyState source)
            {
                _state = source._state;
            }
        }

        private KeyState[] KeyStates { get => _keyStates; }
        private KeyState[] _keyStates = null;


        public AtsKeyStates()
        {
            _keyStates = new KeyState[Enum.GetNames(typeof(AtsKey)).Length];


            for (var i = 0; i < _keyStates.Length; ++i)
            {
                _keyStates[i] = new KeyState();
            }
        }

        public KeyState this[AtsKey keyType]
        {
            get
            {
                var keyIndex = (int)keyType;
                return KeyStates[keyIndex];
            }
        }

        internal void CopyFrom(AtsKeyStates source)
        {
            // NOTE:
            // Called from Run-Time function. Avoid Linq.
            for (var i = 0; i < KeyStates.Length; ++i)
            {
                _keyStates[i].CopyFrom(source._keyStates[i]);
            }
        }
    }
}
