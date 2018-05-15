using UnityEngine;
namespace AKBFramework
{
	public class NodeSystem
	{
		[RuntimeInitializeOnLoadMethod]
		static void InitNodeSystem()
		{
			// cache list			
			
			// cache node
			SafeObjectPool<SequenceNode>.Instance.Init(20, 20);
			SafeObjectPool<DelayNode>.Instance.Init(50, 50);
			SafeObjectPool<EventNode>.Instance.Init(50, 50);
		}
	}
}