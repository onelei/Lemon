using System;
using System.Collections.Generic;

namespace Lemon.Framework.FSM
{
    public class Fsm<TState, TTrigger>
    {
        private readonly Dictionary<TState, Dictionary<TTrigger, TState>> _transitions =
            new Dictionary<TState, Dictionary<TTrigger, TState>>();

        public TState CurrentState { get; private set; }

        public Fsm(TState initialState)
        {
            CurrentState = initialState;
        }

        public void AddTransition(TState fromState, TTrigger trigger, TState toState)
        {
            if (!_transitions.TryGetValue(fromState, out var triggers))
            {
                triggers = new Dictionary<TTrigger, TState>();
                _transitions[fromState] = triggers;
            }

            triggers[trigger] = toState;
        }

        public void Fire(TTrigger trigger)
        {
            if (_transitions[CurrentState].TryGetValue(trigger, out var newState))
            {
                CurrentState = newState;
                Console.WriteLine($"Transitioned to state: {CurrentState}");
            }
            else
            {
                Console.WriteLine("Invalid transition!");
            }
        }
    }
}