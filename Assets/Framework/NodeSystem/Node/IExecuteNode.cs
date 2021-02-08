namespace AKBFramework
{
    using System;
    
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
	
    public interface IExecuteNode : INode, IExecutable<float>, IDisposable,IResetable
    {
        void Break();
        
        bool Finished { get; }
    }
}