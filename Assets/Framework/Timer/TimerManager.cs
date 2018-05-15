using UnityEngine;
using System;
using System.Collections.Generic;

#if (UNITY_EDITOR && DEBUG)
using System.Diagnostics;
#endif

namespace AKBFramework
{
	public class TimerManager : MonoSingleton<TimerManager>
	{
		private static TimerManager _instance = null;
		public static TimerManager Instance
		{
			get{return _instance;}
		}

		private static GameObject mMainObject = null;
		private static List<Event> mActive = new List<Event>();
		private static List<Event> mPool = new List<Event>();
		private static Event mNewEvent = null;
		private static int mEventCount = 0;
		private static int mEventBatch = 0;
		private static int mEventIterator = 0;

		//一次Update能处理的Event上限
		public static int MaxEventsPerFrame = 500;
		//无参数回调
		public delegate void Callback();
		//有参数回调
		public delegate void ArgCallback(object args);

		public struct Stats
		{
			public int Created;
			public int Inactive;
			public int Active;
		}
			
		public void Update()
		{
			mEventBatch = 0;
			while ((TimerManager.mActive.Count > 0) && mEventBatch < MaxEventsPerFrame)
			{
				if (mEventIterator < 0)
				{
					mEventIterator = TimerManager.mActive.Count - 1;
					break;
				}

				if (mEventIterator > TimerManager.mActive.Count - 1)
					mEventIterator = TimerManager.mActive.Count - 1;

				if (Time.time >= TimerManager.mActive [mEventIterator].DueTime || TimerManager.mActive [mEventIterator].ID == 0)
				{
					TimerManager.mActive [mEventIterator].Execute ();
				}
				else
				{
					if (TimerManager.mActive[mEventIterator].Paused)
						TimerManager.mActive[mEventIterator].DueTime += Time.deltaTime;
					else
						TimerManager.mActive[mEventIterator].LifeTime += Time.deltaTime;
				}

				mEventIterator--;
				mEventBatch++;
			}
		}
			
		public static void AddTimer(float delay, Callback callback, Handle timerHandle = null)
		{ Schedule(delay, callback, null, null, timerHandle, 1, -1.0f); }

		public static void AddTimer(float delay, Callback callback, int iterations, Handle timerHandle = null)
		{ Schedule(delay, callback, null, null, timerHandle, iterations, -1.0f); }

		public static void AddTimer(float delay, Callback callback, int iterations, float interval, Handle timerHandle = null)
		{ Schedule(delay, callback, null, null, timerHandle, iterations, interval); }

		public static void AddTimer(float delay, ArgCallback callback, object arguments, Handle timerHandle = null)
		{ Schedule(delay, null, callback, arguments, timerHandle, 1, -1.0f); }

		public static void AddTimer(float delay, ArgCallback callback, object arguments, int iterations, Handle timerHandle = null)
		{ Schedule(delay, null, callback, arguments, timerHandle, iterations, -1.0f); }

		public static void AddTimer(float delay, ArgCallback callback, object arguments, int iterations, float interval, Handle timerHandle = null)
		{ Schedule(delay, null, callback, arguments, timerHandle, iterations, interval); }

		private static void Schedule(float time, Callback func, ArgCallback argFunc, object args, Handle timerHandle, int iterations, float interval)
		{
			if (func == null && argFunc == null)
			{
				UnityEngine.Debug.LogError("Error: (TimerManager) Aborted event because function is null.");
				return;
			}

			time = Mathf.Max(0.0f, time);
			iterations = Mathf.Max(0, iterations);
			interval = (interval == -1.0f) ? time : Mathf.Max(0.0f, interval);

			mNewEvent = null;
			if (mPool.Count > 0)
			{
				mNewEvent = mPool [0];
				mPool.Remove (mNewEvent);
			}
			else
			{
				mNewEvent = new Event ();
			}

			TimerManager.mEventCount++;
			mNewEvent.ID = TimerManager.mEventCount;

			if (func != null)
			{
				mNewEvent.Function = func;
			}
			else if (argFunc != null)
			{
				mNewEvent.ArgFunction = argFunc;
				mNewEvent.Arguments = args;
			}
			mNewEvent.StartTime = Time.time;
			mNewEvent.DueTime = Time.time + time;
			mNewEvent.Iterations = iterations;
			mNewEvent.Interval = interval;
			mNewEvent.LifeTime = 0.0f;
			mNewEvent.Paused = false;
			TimerManager.mActive.Add(mNewEvent);

			if (timerHandle != null)
			{
				if (timerHandle.Active)
				{
					timerHandle.Cancel ();
				}
				timerHandle.ID = mNewEvent.ID;
			}

			#if (UNITY_EDITOR && DEBUG)
			mNewEvent.StoreCallingMethod();
			EditorRefresh();
			#endif
		}
			
		private static void Cancel(TimerManager.Handle handle)
		{

			if (handle == null)
				return;

			if (handle.Active)
			{
				handle.ID = 0;
				return;
			}

		}

		public static void CancelAll()
		{
			for (int t = TimerManager.mActive.Count - 1; t > -1; t--)
			{
				TimerManager.mActive[t].ID = 0;
			}
		}

		public static void CancelAll(string methodName)
		{

			for (int t = TimerManager.mActive.Count - 1; t > -1; t--)
			{
				if (TimerManager.mActive[t].MethodName == methodName)
					TimerManager.mActive[t].ID = 0;
			}

		}

		public static void DestroyAll()
		{
			TimerManager.mActive.Clear();
			TimerManager.mPool.Clear();

			#if (UNITY_EDITOR && DEBUG)
			EditorRefresh();
			#endif

		}

		public static Stats EditorGetStats()
		{
			Stats stats;
			stats.Created = mActive.Count + mPool.Count;
			stats.Inactive = mPool.Count;
			stats.Active = mActive.Count;
			return stats;
		}
			
		public static string EditorGetMethodInfo(int eventIndex)
		{
			if (eventIndex < 0 || eventIndex > mActive.Count - 1)
				return "Argument out of range.";

			return mActive[eventIndex].MethodInfo;
		}

		public static int EditorGetMethodId(int eventIndex)
		{
			if (eventIndex < 0 || eventIndex > mActive.Count - 1)
				return 0;

			return mActive[eventIndex].ID;
		}


		#if (DEBUG && UNITY_EDITOR)
		private static void EditorRefresh()
		{
		}
		#endif

		private class Event
		{
			public int ID;

			public Callback Function = null;
			public ArgCallback ArgFunction = null;
			public object Arguments = null;

			public int Iterations = 1;
			public float Interval = -1.0f;
			public float DueTime = 0.0f;
			public float StartTime = 0.0f;
			public float LifeTime = 0.0f;
			public bool Paused = false;

			#if (DEBUG && UNITY_EDITOR)
			private string mCallingMethod = "";
			#endif

			public void Execute()
			{
				//该事件已经被取消
				if (ID == 0 || DueTime == 0.0f)
				{
					Recycle();
					return;
				}

				if (Function != null)
				{
					Function ();
				}
				else if (ArgFunction != null)
				{
					ArgFunction (Arguments);
				}
				else
				{
					Error("Aborted event because function is null.");
					Recycle();
					return;
				}

				if (Iterations > 0)
				{
					Iterations--;
					if (Iterations < 1)
					{
						Recycle();
						return;
					}
				}
				DueTime = Time.time + Interval;
			}

			private void Recycle()
			{
				ID = 0;
				DueTime = 0.0f;
				StartTime = 0.0f;

				Function = null;
				ArgFunction = null;
				Arguments = null;

				if (TimerManager.mActive.Remove(this))
					mPool.Add(this);

				#if (UNITY_EDITOR && DEBUG)
				EditorRefresh();
				#endif
			}

			private void Destroy()
			{
				TimerManager.mActive.Remove(this);
				TimerManager.mPool.Remove(this);
			}

			#if (UNITY_EDITOR && DEBUG)
			public void StoreCallingMethod()
			{
				StackTrace stackTrace = new StackTrace();

				string result = "";
				string declaringType = "";
				for (int v = 3; v < stackTrace.FrameCount; v++)
				{
					StackFrame stackFrame = stackTrace.GetFrame(v);
					declaringType = stackFrame.GetMethod().DeclaringType.ToString();
					result += " <- " + declaringType + ":" + stackFrame.GetMethod().Name.ToString();
				}

				mCallingMethod = result;

			}
			#endif

			private void Error(string message)
			{
				string msg = "Error: (TimerManager.Event) " + message;
				#if (UNITY_EDITOR && DEBUG)
				msg += MethodInfo;
				#endif
				UnityEngine.Debug.LogError(msg);
			}
		
			public string MethodName
			{
				get
				{
					if (Function != null)
					{
						if (Function.Method != null)
						{
							if (Function.Method.Name[0] == '<')
								return "delegate";
							else return Function.Method.Name;
						}
					}
					else if (ArgFunction != null)
					{
						if (ArgFunction.Method != null)
						{
							if (ArgFunction.Method.Name[0] == '<')
								return "delegate";
							else return ArgFunction.Method.Name;
						}
					}
					return null;
				}
			}

			public string MethodInfo
			{
				get
				{
					string s = MethodName;
					if (!string.IsNullOrEmpty(s))
					{
						s += "(";
						if (Arguments != null)
						{
							if (Arguments.GetType().IsArray)
							{
								object[] array = (object[])Arguments;
								foreach (object o in array)
								{
									s += o.ToString();
									if (Array.IndexOf(array, o) < array.Length - 1)
										s += ", ";
								}
							}
							else
								s += Arguments;
						}
						s += ")";
					}
					else
						s = "(function = null)";

					#if (DEBUG && UNITY_EDITOR)
					s += mCallingMethod;
					#endif
					return s;
				}
			}

		}

		public class Handle
		{
			private TimerManager.Event mEvent = null;
			private int mId = 0;
			private int mStartIterations = 1;
			private float mFirstDueTime = 0.0f;	

			public bool Paused
			{
				get
				{
					return Active && mEvent.Paused;
				}
				set
				{
					if (Active)
					{
						mEvent.Paused = value;
					}
				}
			}

			public float TimeOfInitiation
			{
				get
				{
					if (Active)
						return mEvent.StartTime;
					else return 0.0f;
				}
			}

			public float TimeOfFirstIteration
			{
				get
				{
					if (Active)
						return mFirstDueTime;
					return 0.0f;
				}
			}
		
			public float TimeOfNextIteration
			{
				get
				{
					if (Active)
						return mEvent.DueTime;
					return 0.0f;
				}
			}

			public float TimeOfLastIteration
			{
				get
				{
					if (Active)
						return Time.time + DurationLeft;
					return 0.0f;
				}
			}

			public float Delay
			{
				get
				{
					return (Mathf.Round((mFirstDueTime - TimeOfInitiation) * 1000.0f) / 1000.0f);
				}
			}

			public float Interval
			{
				get
				{
					if (Active)
						return mEvent.Interval;
					return 0.0f;
				}
			}

			public float TimeUntilNextIteration
			{
				get
				{
					if (Active)
						return mEvent.DueTime - Time.time;
					return 0.0f;
				}
			}

			public float DurationLeft
			{
				get
				{
					if (Active)
						return TimeUntilNextIteration + ((mEvent.Iterations - 1) * mEvent.Interval);
					return 0.0f;
				}
			}

			public float DurationTotal
			{
				get
				{
					if (Active)
					{
						return Delay + ((mStartIterations) * ((mStartIterations > 1) ? Interval : 0.0f));
					}
					return 0.0f;
				}
			}

            //已经存在的时间
			public float Duration
			{
				get
				{
					if (Active)
						return mEvent.LifeTime;
					return 0.0f;
				}
			}

			//总迭代次数
			public int IterationsTotal
			{
				get
				{
					return mStartIterations;
				}
			}

			//剩余迭代次数
			public int IterationsLeft
			{
				get
				{
					if (Active)
						return mEvent.Iterations;
					return 0;
				}
			}

			public int ID
			{
				get
				{
					return mId;
				}
				set
				{
					mId = value;

					if (mId == 0)
					{
						mEvent.DueTime = 0.0f;
						return;
					}

					mEvent = null;
					for (int t = TimerManager.mActive.Count - 1; t > -1; t--)
					{
						if (TimerManager.mActive[t].ID == mId)
						{
							mEvent = TimerManager.mActive[t];
							break;
						}
					}
					if (mEvent == null)
						UnityEngine.Debug.LogError("Error: (TimerManager.Handle) Failed to assign event with Id '" + mId + "'.");

					mStartIterations = mEvent.Iterations;
					mFirstDueTime = mEvent.DueTime;
				}
			}

			public bool Active
			{
				get
				{
					if (mEvent == null || ID == 0 || mEvent.ID == 0)
						return false;
					return mEvent.ID == ID;
				}
			}
		    //方法名
			public string MethodName { get { return mEvent.MethodName; } }
			//方法信息
			public string MethodInfo { get { return mEvent.MethodInfo; } }

			public void Cancel()
			{
				TimerManager.Cancel(this);
			}
				
			public void Execute()
			{
				mEvent.DueTime = Time.time;
			}
		}
	}
}
