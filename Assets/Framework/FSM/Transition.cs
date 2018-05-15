using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKBFramework.StateMachine
{
	public class Transition
	{
		public State Source = null;
		public State Target = null;

		public Machine Machine
		{
			get 
			{
				if (Source != null)
					return Source.Machine;
				return null;
			}
		}

		public virtual bool TestEvent(Event _event) 
		{
			return true;
		}

		public virtual void OnTransition(Event _event)
		{
		}
	}

	public class EventTransition : Transition
	{
		public int EventID = -1;

		public EventTransition() { }

		public EventTransition(int _eventID) 
		{
			EventID = _eventID;
		}

		public override bool TestEvent(Event _event) 
		{
			return _event.id == EventID;
		}
	}
}