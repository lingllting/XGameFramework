namespace AKBFramework
{
    using System;
    using System.Collections.Generic;
    
    public class SimpleObjectCache
    {
        private readonly Dictionary<Type, object> mObjectPools;

        public SimpleObjectCache()
        {
            mObjectPools = new Dictionary<Type, object>();
        }

        public SimpleObjectPool<T> GetObjectPool<T>() where T : new()
        {
            object objectPool;
            var type = typeof(T);
            if (!mObjectPools.TryGetValue(type, out objectPool))
            {
                objectPool = new SimpleObjectPool<T>(() => new T());
                mObjectPools.Add(type, objectPool);
            }

            return ((SimpleObjectPool<T>) objectPool);
        }

        public T Get<T>() where T : new()
        {
            return GetObjectPool<T>().Allocate();
        }

        public void Push<T>(T obj) where T : new()
        {
            GetObjectPool<T>().Recycle(obj);
        }

        public void RegisterCustomObjectPool<T>(SimpleObjectPool<T> simpleObjectPool)
        {
            mObjectPools.Add(typeof(T), simpleObjectPool);
        }

        public void Reset()
        {
            mObjectPools.Clear();
        }
    }
}