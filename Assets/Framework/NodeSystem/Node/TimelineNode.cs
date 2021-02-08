namespace AKBFramework
{
	using System.Linq;
	using System.Collections.Generic;
	using System;
	public class TimelineNode : ExecuteNode
	{
		private float _curTime = 0;

		public Action OnTimelineBeganCallback
		{
			get { return onBeganCallback; }
			set { onBeganCallback = value; }
		}

		public Action OnTimelineEndedCallback
		{
			get { return onEndedCallback; }
			set { onEndedCallback = value; }
		}

		public Action<string> OnKeyEventsReceivedCallback = null;

		public class TimelinePair
		{
			public float Time;
			public IExecuteNode Node;

			public TimelinePair(float time, IExecuteNode node)
			{
				Time = time;
				Node = node;
			}
		}

		/// <summary>
		/// refator 2 one list? all in one list;
		/// </summary>
		public Queue<TimelinePair> TimelineQueue = new Queue<TimelinePair>();

		protected override void OnReset()
		{
			_curTime = 0.0f;
			//TODO TimelineQueue.ForEach(pair => pair.Node.Reset());
		}

		protected override void OnExecute(float dt)
		{
			_curTime += dt;

			foreach (var pair in TimelineQueue.Where(pair => pair.Time < _curTime && !pair.Node.Finished))
			{
				if (pair.Node.Execute(dt))
				{
					Finished = TimelineQueue.Where(timetinePair => !timetinePair.Node.Finished).Count() == 0;
				}
			}
		}

		public TimelineNode(params TimelinePair[] pairs)
		{
			foreach (var pair in pairs)
			{
				TimelineQueue.Enqueue(pair);
			}
		}

		public void Append(TimelinePair pair)
		{
			TimelineQueue.Enqueue(pair);
		}

		public void Append(float time, IExecuteNode node)
		{
			TimelineQueue.Enqueue(new TimelinePair(time, node));
		}

		protected override void OnDispose()
		{
			foreach (var timelinePair in TimelineQueue)
			{
				timelinePair.Node.Dispose();
			}

			TimelineQueue.Clear();
			TimelineQueue = null;
		}
	}
}