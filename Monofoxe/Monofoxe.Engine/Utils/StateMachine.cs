﻿using System.Collections.Generic;

namespace Monofoxe.Engine.Utils
{
	public delegate void StateMachineDelegate<T>(StateMachine<T> caller);

	/// <summary>
	/// Stack-based state machine.
	/// </summary>
	public class StateMachine<T>
	{
		/// <summary>
		/// All the available states.
		/// </summary>
		protected Dictionary<T, StateMachineDelegate<T>> _states;	
		
		protected Dictionary<T, StateMachineDelegate<T>> _enterStateEvents;
		protected Dictionary<T, StateMachineDelegate<T>> _exitStateEvents;
		


		/// <summary>
		/// Stack of active states.
		/// </summary>
		protected Stack<T> _stateStack;

		public T CurrentState => _stateStack.Peek();

		public readonly T DefaultState;


		public StateMachine(T defaultState) 
		{
			_states = new Dictionary<T, StateMachineDelegate<T>>();
			_enterStateEvents = new Dictionary<T, StateMachineDelegate<T>>();
			_exitStateEvents = new Dictionary<T, StateMachineDelegate<T>>();

			_stateStack = new Stack<T>();
			DefaultState = defaultState;
			_stateStack.Push(defaultState);
		}
		

		/// <summary>
		/// Updates state machine and executes current state method.
		/// </summary>
		public virtual void Update() =>
			_states[CurrentState](this);
		

		
		/// <summary>
		/// Empties state stack, but keeps current state.
		/// </summary>
		public virtual void ClearStack()
		{
			var currentState = CurrentState;
			_stateStack.Clear();
			_stateStack.Push(currentState);
		}


		/// <summary>
		/// Empties state stack and resets state to default value.
		/// </summary>
		public virtual void Reset()
		{
			_stateStack.Clear();
			_stateStack.Push(DefaultState);
		}


		
		/// <summary>
		/// Adds new state to a state machine.
		/// </summary>
		public virtual void AddState(
			T stateKey, 
			StateMachineDelegate<T> stateMethod, 
			StateMachineDelegate<T> enterStateEvent = null, 
			StateMachineDelegate<T> exitStateEvent = null
		)
		{
			_states.Add(stateKey, stateMethod);
			if (enterStateEvent != null)
			{
				_enterStateEvents.Add(stateKey, enterStateEvent);
			}
			if (exitStateEvent != null)
			{
				_exitStateEvents.Add(stateKey, exitStateEvent);
			}
		}
		

		/// <summary>
		/// Removes existing state from a state machine.
		/// </summary>
		public virtual void RemoveState(T state) =>
			_states.Remove(state);


		/// <summary>
		/// Pushes new state to a state machine.
		/// 
		/// NOTE: State should already exist in the machine.
		/// </summary>
		public virtual void PushState(T state)
		{
			CallExitEvent(CurrentState);
			_stateStack.Push(state);
			CallEnterEvent(CurrentState);
		}

		
		/// <summary>
		/// Pops current active state from a machine.
		/// </summary>
		public virtual T PopState()
		{
			CallExitEvent(CurrentState);
			var oldState = _stateStack.Pop();
			CallEnterEvent(CurrentState);
			return oldState;
		}
		

		/// <summary>
		/// Replaces current state with a new state. Basically, pop and push together.
		/// </summary>
		public virtual T ChangeState(T state)
		{
			CallExitEvent(CurrentState);
			var oldState = _stateStack.Pop();
			_stateStack.Push(state);
			CallEnterEvent(CurrentState);

			return oldState;
		}

		protected void CallExitEvent(T state)
		{
			if (_exitStateEvents.TryGetValue(state, out StateMachineDelegate<T> exitEvent))
			{
				exitEvent(this);
			}
		}
		protected void CallEnterEvent(T state)
		{
			if (_enterStateEvents.TryGetValue(state, out StateMachineDelegate<T> enterEvent))
			{
				enterEvent(this);
			}
		}

	}
}
