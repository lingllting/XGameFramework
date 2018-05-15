namespace AKBFramework
{
    using System.Collections.Generic;

    public abstract class Pool<T> : IPool<T>,ICountObserveAble
    {
        #region ICountObserverable
        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }
        #endregion
        
        protected IObjectFactory<T> mFactory;

        protected readonly Stack<T> mCacheStack = new Stack<T>();

        /// <summary>
        /// default is 5
        /// </summary>
        protected int mMaxCount = 12;

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0
                ? mFactory.Create()
                : mCacheStack.Pop();
        }

        public abstract bool Recycle(T obj);
    }
}