using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameFramework.StateMachine
{
	public class Machine : State
	{
		public enum MachineState
		{
			Running,
			Paused,
			Stopping,
			Stopped
		}

		protected bool mShowDebugInfo = true;
		protected bool mLogDebugInfo = false;
		[SerializeField]
		public bool debug = false;
		protected MachineState mMachineState = MachineState.Stopped;
		// NOTE: startState --transition--> initState 
		protected State mStartState = new State("startState");
		protected List<Event>[] mEventBuffer = new List<Event>[2] { new List<Event>(), new List<Event>() };
		protected int mCurEventBufferIdx = 0;
		protected int mNextEventBufferIdx = 1;
		protected bool mIsUpdating = false;
		protected List<Transition> mValidTransitions = new List<Transition>();

		public System.Action onStart;
		public System.Action onStop;

		public Machine(): base("machine") 
		{
		}

		public void Restart()
		{
			Stop();
			Start();
		}

		public void Start()
		{
			if (mMachineState == MachineState.Running || mMachineState == MachineState.Paused)
			{
				return;
			}

			mMachineState = MachineState.Running;
			if (onStart != null)
				onStart();

			Event nullEvent = new Event(Event.NULL);
			if (mMode == State.Mode.Exclusive) 
			{
				if (InitState != null)
				{
					EnterStates(nullEvent, InitState, mStartState);
				} 
				else
				{
					Debug.LogError("Error: can't find initial state in " + Name);
				}
			} 
			else 
			{
				for (int i = 0; i < mChildren.Count; ++i) 
				{
					EnterStates(nullEvent, mChildren[i], mStartState);
				}
			}
		}

		public void Stop() 
		{
			if (mMachineState == MachineState.Stopped)
				return;

			if (mIsUpdating) 
			{
				mMachineState = MachineState.Stopping;
			}
			else
			{
				ProcessStop();
			}
		}

		protected void ProcessStop() 
		{
			mEventBuffer[0].Clear();
			mEventBuffer[1].Clear();
			ClearCurrentStatesRecursively();

			if (onStop != null)
				onStop();

			mMachineState = MachineState.Stopped;
		}

		public void Update() 
		{
			if (mMachineState == MachineState.Paused || mMachineState == MachineState.Stopped)
				return;

			mIsUpdating = true;

			if (mMachineState != MachineState.Stopping) 
			{
				int tmp = mCurEventBufferIdx;
				mCurEventBufferIdx = mNextEventBufferIdx;
				mNextEventBufferIdx = tmp;

				//
				bool doStop = false;
				List<Event> eventList = mEventBuffer[mCurEventBufferIdx];
				for (int i = 0; i < eventList.Count; ++i)
				{
					if (HandleEvent(eventList[i])) 
					{
						doStop = true;
						break;
					}
				}
				eventList.Clear();

				if (doStop)
				{
					Stop();
				} 
				else 
				{
					OnAction();
				}
			}

			mIsUpdating = false;

			if (mMachineState == MachineState.Stopping)
			{
				ProcessStop();
			}
		}

		public void Pause() { mMachineState = MachineState.Paused; }
		public void Resume() { mMachineState = MachineState.Running; }

		protected bool HandleEvent(Event _event) 
		{
			OnEvent(_event);
			// 
			mValidTransitions.Clear();
			TestTransitions(ref mValidTransitions, _event);

			if (debug)
			{
				if (mValidTransitions.Count == 0) 
				{
					string output = string.Format("Test transitions failed, event: {0}! Current: ", _event.id);
					foreach (State s in CurrentStates)
					{
						output += (s.Name + " ");
					}
					Debug.LogWarning(output);
				}
			}

			ExitStates(_event, mValidTransitions);
			ExecTransitions(_event, mValidTransitions);
			EnterStates(_event, mValidTransitions);

			if (_event.id == Event.FINISHED) 
			{
				bool canStop = true;
				for (int i = 0; i < CurrentStates.Count; ++i) 
				{
					if ((CurrentStates[i] is FinalState) == false) 
					{
						canStop = false;
						break;
					}
				}
				if (canStop)
				{
					return true;
				}
			}
			return false;
		}

		public void Send(int _eventID, bool _immediately = false) { Send(new Event(_eventID), _immediately); }
		public void Send(Event _event, bool _immediately = false) 
		{
			if (mMachineState == MachineState.Stopped)
				return;

			if (_immediately)
			{
				if (HandleEvent(_event)) 
				{
					Stop();
				} 
				else 
				{
					OnAction();
				}
			}
			else 
			{
				//Debug.Log("Receive event: " + _event.id);
				mEventBuffer[mNextEventBufferIdx].Add(_event);
			}
		}

		protected void EnterStates(Event _event, List<Transition> _transitionList)
		{
			for (int i = 0; i < _transitionList.Count; ++i) 
			{
				Transition transition = _transitionList[i];
				State targetState = transition.Target;
				if (targetState == null)
					targetState = transition.Source;

				if (targetState.Parent != null)
					targetState.Parent.EnterStates(_event, targetState, transition.Source);
			}
		}

		protected void ExitStates(Event _event, List<Transition> _transitionList) 
		{
			for (int i = 0; i < _transitionList.Count; ++i)
			{
				Transition transition = _transitionList[i];

				if (transition.Source.Parent != null)
					transition.Source.Parent.ExitStates(_event, transition.Target, transition.Source);
			}
		}

		protected void ExecTransitions(Event _event, List<Transition> _transitionList) 
		{
			for (int i = 0; i < _transitionList.Count; ++i)
			{
				Transition transition = _transitionList[i];
				transition.OnTransition(_event);
			}
		}

		public void ShowDebugGUI(string _name, GUIStyle _textStyle)
		{
			GUILayout.Label("State Machine (" + _name + ")");
			mShowDebugInfo = GUILayout.Toggle(mShowDebugInfo, "Show States");
			mLogDebugInfo = GUILayout.Toggle(mLogDebugInfo, "Log States");
			if (mShowDebugInfo) 
			{
				ShowDebugInfo(0, true, _textStyle);
			}
		}
	}
}
