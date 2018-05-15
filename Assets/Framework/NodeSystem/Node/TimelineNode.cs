namespace AKBFramework
{
	using System.Linq;
	using System.Collections.Generic;
	using System;

	/// <summary>
	/// 时间轴执行节点
	/// </summary>
	public class TimelineNode : ExecuteNode
	{
		private float mCurTime = 0;

		public Action OnTimelineBeganCallback
		{
			get { return OnBeganCallback; }
			set { OnBeganCallback = value; }
		}

		public Action OnTimelineEndedCallback
		{
			get { return OnEndedCallback; }
			set { OnEndedCallback = value; }
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
			mCurTime = 0.0f;
			//TODO TimelineQueue.ForEach(pair => pair.Node.Reset());
		}

		protected override void OnExecute(float dt)
		{
			mCurTime += dt;

			foreach (var pair in TimelineQueue.Where(pair => pair.Time < mCurTime && !pair.Node.Finished))
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