using System;
using System.Reflection;

namespace AKBFramework
{
    public abstract class Singleton<T> : ISingleton where T : class, ISingleton
    {
        protected static T mInstance;

        static object mLock = new object();

        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        Initialize();
                    }
                }

                return mInstance;
            }
        }

        public static void Initialize()
        {
            if (mInstance == null)
            {
                var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                var ctor = Array.Find(constructors, c => c.GetParameters().Length == 0);

                if (ctor == null)
                {
                    throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
                }

                mInstance = ctor.Invoke(null) as T;
                mInstance?.OnSingletonInit();
            }
        }
        public static bool IsInitialized => mInstance != null;

        public virtual void Dispose()
        {
            mInstance = default;
        }

        public virtual void OnSingletonInit()
        {
        }
    }
}