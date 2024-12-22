using System;
using System.Collections.Generic;

namespace LemonFramework.HFSM
{
    public class HFsm<T>
    {
        private readonly T _owner;
        private readonly Dictionary<Type, IState<T>> _states = new Dictionary<Type, IState<T>>();
        private IState<T> _currentState;

        public HFsm(T owner)
        {
            this._owner = owner;
        }

        // AddState
        public void AddState<TState>(TState state) where TState : IState<T>
        {
            _states.Add(typeof(TState), state);
        }

        // ChangeState
        public void ChangeState<TState>() where TState : IState<T>
        {
            if (_currentState != null)
            {
                _currentState.Exit(_owner);
            }

            Type newStateType = typeof(TState);
            if (_states.TryGetValue(newStateType, out IState<T> newState))
            {
                _currentState = newState;
                _currentState.Enter(_owner);
            }
            else
            {
                throw new ArgumentException($"State {newStateType} not found in the state machine.");
            }
        }

        //Update
        public void Update()
        {
            if (_currentState != null)
            {
                _currentState.Execute(_owner);
            }
        }
    }
}