namespace AKBFramework 
{
	public class KeyEventNode : EventNode
	{
		private TimelineNode _timelineNode;
		private string _keyEventName;

		public KeyEventNode(string keyEventName, TimelineNode timelineNode)
		{
			_timelineNode = timelineNode;
			_keyEventName = keyEventName;
		}

		protected override void OnExecute(float dt)
		{
			_timelineNode.OnKeyEventsReceivedCallback(_keyEventName);
			Finished = true;
		}

		protected override void OnDispose()
		{
			_timelineNode = null;
			_keyEventName = null;
		}
	}
}

