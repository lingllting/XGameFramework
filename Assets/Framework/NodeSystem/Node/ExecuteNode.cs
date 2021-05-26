namespace XGameFramework
{
    using System;

    public abstract class ExecuteNode : IExecuteNode
    {
        protected Action onBeganCallback = null;
        protected Action onEndedCallback = null;
        private Action _onDisposedCallback = null;

        private bool _onBeginCalled = false;

        #region IExecuteNode Support
        public bool Finished { get; protected set; }
        private bool _disposed = false;

        public void Break()
        {
            Finished = true;
        }
        #endregion

        #region ResetableSupport

        public void Reset()
        {
            Finished = false;
            _onBeginCalled = false;
            _disposed = false;
            OnReset();
        }
        #endregion


        #region IExecutable Support

        public bool Execute(float dt)
        {
            if (!_onBeginCalled)
            {
                _onBeginCalled = true;
                OnBegin();
				onBeganCallback?.Invoke();
            }

            if (!Finished)
            {
                OnExecute(dt);
            }

            if (Finished)
            {                
                OnEnd();
				onEndedCallback?.Invoke();
            }

            return Finished || _disposed;
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
            if (_disposed) return;
            _disposed = true;
            
            onBeganCallback = null;
            onEndedCallback = null;
			_onDisposedCallback?.Invoke();
            _onDisposedCallback = null;
            OnDispose();
        }

        #endregion
    }
}