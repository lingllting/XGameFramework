namespace AKBFramework
{
    using System;

    public abstract class ExecuteNode : IExecuteNode
    {
        public Action OnBeganCallback = null;
        public Action OnEndedCallback = null;
        public Action OnDisposedCallback = null;
        
        protected bool mOnBeginCalled = false;

        #region IExecuteNode Support
        public bool Finished { get; protected set; }
        protected bool mDisposed = false;

        public void Break()
        {
            Finished = true;
        }
        #endregion

        #region ResetableSupport

        public void Reset()
        {
            Finished = false;
            mOnBeginCalled = false;
            mDisposed = false;
            OnReset();
        }
        #endregion


        #region IExecutable Support

        public bool Execute(float dt)
        {
            if (!mOnBeginCalled)
            {
                mOnBeginCalled = true;
                OnBegin();
				OnBeganCallback.InvokeGracefully();
            }

            if (!Finished)
            {
                OnExecute(dt);
            }

            if (Finished)
            {                
                OnEnd();
				OnEndedCallback.InvokeGracefully();
            }

            return Finished || mDisposed;
        }

        #endregion

        protected virtual void OnReset()
        {
        }

        protected virtual void OnBegin()
        {
        }

        /// <summary>
        /// finished
        /// </summary>
        protected virtual void OnExecute(float dt)
        {
        }

        protected virtual void OnEnd()
        {
        }

        protected virtual void OnDispose()
        {
        }

        #region IDisposable Support

        public void Dispose()
        {
            if (mDisposed) return;
            mDisposed = true;
            
            OnBeganCallback = null;
            OnEndedCallback = null;
			OnDisposedCallback.InvokeGracefully();
            OnDisposedCallback = null;
            OnDispose();
        }

        #endregion
    }
}