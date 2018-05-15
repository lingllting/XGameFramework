namespace AKBFramework
{
    using UnityEngine;
    using System;
    using UniRx;

    public abstract class ExecuteNodeChain : ExecuteNode, IExecuteNodeChain, IDisposeWhen
    {
        public MonoBehaviour Executer { get; set; }

        protected abstract ExecuteNode mNode { get; }

        public abstract IExecuteNodeChain Append(IExecuteNode node);

        protected override void OnBegin()
        {
            base.OnBegin();

            if (mDisposeWhenOnDestroyed)
            {
                this.AddTo(Executer);
            }
        }

        protected override void OnExecute(float dt)
        {
            if (mDisposeWhenCondition && mDisposeCondition.Invoke())
            {
                Finished = true;
            }
            else
            {
                Finished = mNode.Execute(dt);
            }

            if (Finished && mDisposeWhenFinished)
            {
                Dispose();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Executer = null;
            mDisposeWhenCondition = false;
            mDisposeWhenFinished = false;
            mDisposeWhenOnDestroyed = false;
            mDisposeCondition = null;
			mOnDisposedEvent.InvokeGracefully();
            mOnDisposedEvent = null;
        }

        public IDisposeWhen Begin()
        {
            Executer.ExecuteNode(this);
            return this;
        }

        private bool mDisposeWhenOnDestroyed = false;
        private bool mDisposeWhenFinished = true;
        private bool mDisposeWhenCondition = false;
        private Func<bool> mDisposeCondition;
        private Action mOnDisposedEvent = null;

        public IDisposeEventRegister DisposeWhenGameObjDestroyed()
        {
            mDisposeWhenFinished = false;
            mDisposeWhenOnDestroyed = true;
            return this;
        }

        /// <summary>
        /// Default
        /// </summary>
        /// <returns></returns>
        public IDisposeEventRegister DisposeWhenFinished()
        {
            mDisposeWhenFinished = true;
            return this;
        }

        public IDisposeEventRegister DisposeWhen(Func<bool> condition)
        {
            mDisposeWhenFinished = true;
            mDisposeWhenCondition = true;
            mDisposeCondition = condition;
            return this;
        }

        public void OnDisposed(Action onDisposedEvent)
        {
            mOnDisposedEvent = onDisposedEvent;
        }
    }
}