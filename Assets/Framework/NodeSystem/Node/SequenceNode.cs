namespace AKBFramework
{
	using System.Collections.Generic;

	/// <summary>
	/// 序列执行节点
	/// </summary>
	public sealed class SequenceNode : ExecuteNode ,IPoolable
	{
		protected readonly List<IExecuteNode> mNodes = new List<IExecuteNode>();
		protected readonly List<IExecuteNode> mExcutingNodes = new List<IExecuteNode>();
		
		public bool Completed = false;

		public int TotalCount
		{
			get { return mExcutingNodes.Count; }
		}

		protected override void OnReset()
		{
			mExcutingNodes.Clear();
			foreach (var node in mNodes)
			{
				node.Reset();
				mExcutingNodes.Add(node);
			}
			Completed = false;
		}

		protected override void OnExecute(float dt)
		{
			if (mExcutingNodes.Count > 0)
			{
				if (mExcutingNodes[0].Execute(dt))
				{
					mExcutingNodes.RemoveAt(0);
				}
			} 
			else
			{
				Finished = true;
				Completed = true;
			}
		}

		public static SequenceNode Allocate(params IExecuteNode[] nodes)
		{
			var retNode = SafeObjectPool<SequenceNode>.Instance.Allocate();
			foreach (var node in nodes)
			{
				retNode.mNodes.Add(node);
				retNode.mExcutingNodes.Add(node);
			}

			return retNode;
		}

		/// <summary>
		/// 不建议使用
		/// </summary>
		public SequenceNode(){}

		public SequenceNode Append(IExecuteNode appendedNode)
		{
			mNodes.Add(appendedNode);
			mExcutingNodes.Add(appendedNode);
			return this;
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			
			SafeObjectPool<SequenceNode>.Instance.Recycle(this);
		}

		void IPoolable.OnRecycled()
		{
			mNodes.ForEach(node => node.Dispose());
			mNodes.Clear();

			mExcutingNodes.Clear();
		}

		bool IPoolable.IsRecycled { get; set; }
	}
}