namespace AKBFramework
{
    using UnityEngine;
    using System;
    
    public interface IExecuteNodeChain : IExecuteNode
    {
        MonoBehaviour Executer { get; set; }

        IExecuteNodeChain Append(IExecuteNode node);

        IDisposeWhen Begin();
    }
    
    public interface IDisposeWhen : IDisposeEventRegister
    {
        IDisposeEventRegister DisposeWhenGameObjDestroyed();

        IDisposeEventRegister DisposeWhen(Func<bool> condition);

        IDisposeEventRegister DisposeWhenFinished();

    }

    public interface IDisposeEventRegister
    {
        void OnDisposed(Action onDisposedEvent);
    }
}