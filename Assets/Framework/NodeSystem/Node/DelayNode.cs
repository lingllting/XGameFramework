namespace AKBFramework
{
	using System;

	/// <summary>
	/// 延时执行节点
	/// </summary>
	public class DelayNode : ExecuteNode, IPoolable
	{
		public float DelayTime;

		public static DelayNode Allocate(float delayTime, Action onEndCallback = null)
		{
			var retNode = SafeObjectPool<DelayNode>.Instance.Allocate();
			retNode.DelayTime = delayTime;
			retNode.OnEndedCallback = onEndCallback;
			return retNode;
		}

		public DelayNode()
		{
		}

		public DelayNode(float delayTime)
		{
			DelayTime = delayTime;
		}

		private float mCurrentSeconds = 0.0f;

		protected override void OnReset()
		{
			mCurrentSeconds = 0.0f;
		}

		protected override void OnExecute(float dt)
		{
			mCurrentSeconds += dt;
			Finished = mCurrentSeconds >= DelayTime;
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