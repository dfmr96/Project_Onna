using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine<T> where T : MonoBehaviour
{
  
        public EnemyState<T> CurrentState { get; private set; }

        public void Initialize(EnemyState<T> startingState)
        {
            CurrentState = startingState;
            CurrentState.EnterState();
        }

        public void ChangeState(EnemyState<T> newState)
        {
            CurrentState?.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }

        public void ChangeStateDirect(EnemyState<T> newState)
        {
            CurrentState = newState;
            CurrentState.EnterState();
        }

        public void ExitState()
        {
            CurrentState?.ExitState();
        }


    }
