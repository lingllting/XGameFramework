namespace XGameFramework
{
	using UnityEngine;
	public class NodeSystem
	{
		[RuntimeInitializeOnLoadMethod]
		static void InitNodeSystem()
		{
			// cache node
			SafeObjectPool<SequenceNode>.Instance.Init(20);
			SafeObjectPool<DelayNode>.Instance.Init(20);
			SafeObjectPool<EventNode>.Instance.Init(20);
		}
	}
}