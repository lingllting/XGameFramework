namespace AKBFramework
{
    using System.Collections.Generic;

    public abstract class Pool<T> : IPool<T>
    {
        #region ICountObserverable
        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount => mCacheStack.Count;

        #endregion
        
        protected readonly Stack<T> mCacheStack = new Stack<T>();

        /// <summary>
        /// default is 5
        /// </summary>
        protected int mMaxCount = 12;

        public abstract T Allocate();

        public abstract bool Recycle(T obj);
    }
}