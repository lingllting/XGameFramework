using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AKBFramework.StateMachine
{
	public class Event : System.EventArgs
	{
		static public readonly int UNKNOWN = -1;
		static public readonly int NULL = 0;
		static public readonly int FINISHED = 1;
		public const int USER_FIELD = 1000;

		public int id = UNKNOWN;

		public Event(int _id = -1) 
		{
			id = _id;
		}
	}
}
