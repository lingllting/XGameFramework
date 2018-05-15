namespace AKBFramework
{
    using System;

	/// <summary>
	/// 作为数据结构是一个节点
	/// </summary>
	public interface INode
	{

	}

	public interface IResetable
	{
		void Reset();
	}

	public interface IExecutable<T>
	{
		bool Execute(T arg);
	}

    /// <summary>
    /// 执行节点的基类
    /// </summary>
    public interface IExecuteNode : INode, IExecutable<float>, IDisposable,IResetable
    {
        void Break();
        
        bool Finished { get; }
    }
}