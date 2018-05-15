namespace AKBFramework 
{
	using System.Linq;
	using System.Collections.Generic;
	
	/// <summary>
	/// 并发执行的协程
	/// </summary>
	public class SpawnNode :ExecuteNode 
	{
		protected List<IExecuteNode> mNodeList = new List<IExecuteNode>();

		protected override void OnReset()
		{
			foreach (var executeNode in mNodeList)
			{
				executeNode.Reset();
			}
		}
		
		protected override void OnExecute(float dt)
		{
			foreach (var node in mNodeList.Where(node => !node.Finished))
			{
				if (node.Execute(dt))
				{
					Finished = mNodeList.All(n => n.Finished);
				}
			}
		}
		
		public SpawnNode(params IExecuteNode[] nodes)
		{
			mNodeList.AddRange (nodes);
		}
		
		protected override void OnDispose()
		{
			foreach (var node in mNodeList)
			{
				node.Dispose();
			}
			mNodeList.Clear();
			mNodeList = null;
		}
	}
}