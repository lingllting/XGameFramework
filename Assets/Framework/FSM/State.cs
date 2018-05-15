using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKBFramework.StateMachine
{
	public class State
	{
		public enum Mode
		{
			Exclusive,
			Parallel,
		}

		public string Name = "";
		public List<State> CurrentStates = new List<State>();

		protected Mode mMode = Mode.Exclusive;
		//转换列表
		protected List<Transition> mTransitionList = new List<Transition>();
		//子节点列表
		protected List<State> mChildren = new List<State>();

		protected State mParent = null;
		public State Parent
		{
			set 
			{
				if (mParent != value) 
				{
					State oldParent = mParent;

					while (mParent != null) 
					{
						if (mParent == this)
						{
							return;
						}
						mParent = mParent.Parent;
					}

					if (oldParent != null)
					{
						if (oldParent.InitState == this)
							oldParent.InitState = null;
						oldParent.mChildren.Remove(this);
					}

					if (value != null) 
					{
						value.mChildren.Add(this);
						if (value.mChildren.Count == 1)
							value.InitState = this;
					}
					mParent = value;
				}
			}
			get { return mParent; }
		}

		protected Machine mMachine = null;
		public Machine Machine
		{
			get 
			{
				if (mMachine != null)
					return mMachine;

				State last = this;
				State root = Parent;
				while (root != null) 
				{
					last = root;
					root = root.Parent;
				}
				mMachine = last.Machine as Machine;
				return mMachine;
			}
		}

		protected State mInitState = null;
		public State InitState 
		{
			get 
			{ 
				if (mInitState == null && mChildren.Count > 0)
					mInitState = mChildren [0];
				return mInitState; 
			}
			set 
			{
				if (mInitState != value) 
				{
					if (value != null && mChildren.IndexOf(value) == -1) 
					{
						Debug.LogError("error: You must use child state as initial state.");
						mInitState = null;
						return;
					}
					mInitState = value;
				}
			}
		}

		public System.Action<State /* current */, Event> onEvent;

		public State(string _name, State _parent = null)
		{
			Name = _name;
			Parent = _parent;
		}

		public void ClearCurrentStatesRecursively() 
		{
			CurrentStates.Clear();
			for (int i = 0; i < mChildren.Count; ++i) 
			{
				mChildren[i].ClearCurrentStatesRecursively();
			}
		}

		public T Add<T>(State _targetState) where T : Transition, new() 
		{
			T newTranstion = new T();
			newTranstion.Source = this;
			newTranstion.Target = _targetState;
			mTransitionList.Add(newTranstion);
			return newTranstion;
		}

		public T Add<T>(State _targetState, int _id) where T : EventTransition, new() 
		{
			T newTranstion = Add<T>(_targetState);
			newTranstion.EventID = _id;
			return newTranstion;
		}

		public void AddTransitionEvent(State _targetState, int _transitionEvent)
		{
			this.Add<EventTransition>(_targetState, _transitionEvent);
		}

		public void TestTransitions(ref List<Transition> _validTransitions, Event _event) 
		{
			for (int i = 0; i < CurrentStates.Count; ++i) 
			{
				State activeChild = CurrentStates[i];

				bool hasTranstion = false;
				for (int j = 0; j < activeChild.mTransitionList.Count; ++j)
				{
					Transition transition = activeChild.mTransitionList[j];
					if (transition.TestEvent(_event))
					{
						_validTransitions.Add(transition);
						hasTranstion = true;
						break;
					}
				}
				if (!hasTranstion)
				{
					activeChild.TestTransitions(ref _validTransitions, _event);
				}
			}
		}

		public void EnterStates(Event _event, State _toEnter, State _toExit) 
		{
			CurrentStates.Add(_toEnter);
			_toEnter.OnEnter(_toExit, _toEnter, _event);

			if (_toEnter.mChildren.Count != 0) 
			{
				if (_toEnter.mMode == State.Mode.Exclusive) 
				{
					if (_toEnter.InitState != null) 
					{
						_toEnter.EnterStates(_event, _toEnter.InitState, _toExit);
					}
				} 
				else
				{
					for (int i = 0; i < _toEnter.mChildren.Count; ++i) 
					{
						_toEnter.EnterStates(_event, _toEnter.mChildren[i], _toExit);
					}
				}
			}
		}

		public void ExitStates(Event _event, State _toEnter, State _toExit)
		{
			_toExit.ExitAllStates(_event, _toEnter);
			_toExit.OnExit(_toExit, _toEnter, _event);
			CurrentStates.Remove(_toExit);
		}

		protected void ExitAllStates(Event _event, State _toEnter) 
		{
			for (int i = 0; i < CurrentStates.Count; ++i) 
			{
				State activeChild = CurrentStates[i];
				activeChild.ExitAllStates(_event, _toEnter);
				activeChild.OnExit(activeChild, _toEnter, _event);
			}
			CurrentStates.Clear();
		}

		public void OnAction()
		{
			OnStateUpdate ();

			for (int i = 0; i < CurrentStates.Count; ++i)
			{
				CurrentStates[i].OnAction();
			}
		}

		public void OnEvent(Event _event)
		{
			if (onEvent != null)
			{
				onEvent(this, _event);
			}
			for (int i = 0; i < CurrentStates.Count; ++i) 
			{
				CurrentStates[i].OnEvent(_event);
			}
		}

		public void OnEnter(State _from, State _to, Event _event)
		{
			OnStateEnter (_from, _to, _event);
		}

		public void OnExit(State _from, State _to, Event _event) 
		{
			OnStateExit (_from, _to, _event);
		}

		#region Virtual Methods
		protected virtual void OnStateEnter(State _from, State _to, Event _event)
		{
		}

		protected virtual void OnStateUpdate()
		{
		}

		protected virtual void OnStateExit(State _from, State _to, Event _event)
		{
		}
		#endregion

		public int TotalStates() 
		{
			int count = 1;
			for (int i = 0; i < mChildren.Count; ++i) 
			{
				count += mChildren[i].TotalStates();
			}
			return count;
		}

		public void ShowDebugInfo(int _level, bool _active, GUIStyle _textStyle)
		{
			_textStyle.normal.textColor = _active ? Color.green : new Color(0.5f, 0.5f, 0.5f);
			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			GUILayout.Label(new string('\t', _level) + Name, _textStyle, new GUILayoutOption[] { });
			GUILayout.EndHorizontal();

			for (int i = 0; i < mChildren.Count; ++i) 
			{
				State s = mChildren[i];
				s.ShowDebugInfo(_level + 1, CurrentStates.IndexOf(s) != -1, _textStyle);
			}
		}
	}

	public class FinalState : State
	{
		public FinalState(string _name, State _parent = null) : base(_name, _parent) 
		{
		}

		protected override void OnStateEnter(State _from, State _to, Event _event)
		{
			Machine stateMachine = Machine;
			if (stateMachine != null)
			{
				stateMachine.Send(Event.FINISHED);
			}
		}
	}
}

