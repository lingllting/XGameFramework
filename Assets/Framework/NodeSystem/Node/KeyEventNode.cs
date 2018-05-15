namespace AKBFramework 
{
	public class KeyEventNode : EventNode
	{
		private TimelineNode mTimelineNode;
		private string mKeyEventName;

		public KeyEventNode(string keyEventName, TimelineNode timelineNode)
		{
			mTimelineNode = timelineNode;
			mKeyEventName = keyEventName;
		}

		protected override void OnExecute(float dt)
		{
			mTimelineNode.OnKeyEventsReceivedCallback(mKeyEventName);
			Finished = true;
		}

		protected override void OnDispose()
		{
			mTimelineNode = null;
			mKeyEventName = null;
		}
	}
}

