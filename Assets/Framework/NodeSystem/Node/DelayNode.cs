namespace XGameFramework
{
	using System;

	public class DelayNode : ExecuteNode, IPoolable
	{
		private float DelayTime;

		public static DelayNode Allocate(float delayTime, Action onEndCallback = null)
		{
			var retNode = SafeObjectPool<DelayNode>.Instance.Allocate();
			retNode.DelayTime = delayTime;
			retNode.onEndedCallback = onEndCallback;
			return retNode;
		}

		public DelayNode()
		{
		}

		public DelayNode(float delayTime)
		{
			DelayTime = delayTime;
		}

		private float _currentSeconds = 0.0f;

		protected override void OnReset()
		{
			_currentSeconds = 0.0f;
		}

		protected override void OnExecute(float dt)
		{
			_currentSeconds += dt;
			Finished = _currentSeconds >= DelayTime;
		}

		protected override void OnDispose()
		{
			SafeObjectPool<DelayNode>.Instance.Recycle(this);
		}

		public void OnRecycled()
		{
			Reset();
		}

		public bool IsRecycled { get; set; }
	}
}