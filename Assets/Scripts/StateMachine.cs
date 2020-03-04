using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoatChase
{
    public class StateMachine
    {
        private Dictionary<Type, BaseState> states;

        public BaseState CurrentState { get; private set; }
        public event EventHandler<EventArgs> StateChanged;

        public void Initialize(Dictionary<Type, BaseState> states, Type initialState)
        {
            this.states = states;
            CurrentState = states[initialState];
            CurrentState.OnEnter();
        }

        public void Update()
        {
            var nextState = CurrentState.Update();

            if (nextState != null && nextState != CurrentState.GetType()){ ChangeState(nextState); }
        }

        private void ChangeState(Type nextState)
        {
            CurrentState = states[nextState];
            StateChanged?.Invoke(CurrentState, EventArgs.Empty);
            CurrentState.OnEnter();
        }
    }
}
