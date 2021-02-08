namespace AKBFramework 
{
	using System.Linq;
	using System.Collections.Generic;
	
	public class SpawnNode :ExecuteNode 
	{
		private List<IExecuteNode> _nodeList = new List<IExecuteNode>();

		protected override void OnReset()
		{
			foreach (var executeNode in _nodeList)
			{
				executeNode.Reset();
			}
		}
		
		protected override void OnExecute(float dt)
		{
			foreach (var node in _nodeList.Where(node => !node.Finished))
			{
				if (node.Execute(dt))
				{
					Finished = _nodeList.All(n => n.Finished);
				}
			}
		}
		
		public SpawnNode(params IExecuteNode[] nodes)
		{
			_nodeList.AddRange (nodes);
		}
		
		protected override void OnDispose()
		{
			foreach (var node in _nodeList)
			{
				node.Dispose();
			}
			_nodeList.Clear();
			_nodeList = null;
		}
	}
}