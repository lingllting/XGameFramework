namespace XGameFramework
{
	using UnityEngine;

	public abstract class MonoSingleton<T> : MonoBehaviour,ISingleton where T : MonoSingleton<T>
	{
		protected static T mInstance = null;
		// Check to see if it's about to be destroyed.
		private static bool s_isShuttingDown = false;
		private static readonly object m_Lock = new object();

		public static T Instance
		{
			get
			{
				if (s_isShuttingDown)
				{
					return null;
				}

				lock (m_Lock)
				{
					if (mInstance == null)
					{
						Initialize();
					}
				}
				return mInstance;
			}
		}

        public static void Initialize(bool isDontDestroy = true)
        {
	        if (mInstance == null) 
	        {
		        var obj = new GameObject("(Singleton) " + typeof(T).Name);
		        DontDestroyOnLoad(obj);
		        mInstance = obj.AddComponent<T>();
		        mInstance.OnSingletonInit();
	        }
        }

        public static bool IsInitialized => mInstance != null;

        public virtual void OnSingletonInit()
		{
			
		}

		public virtual void Dispose()
		{
			if (gameObject != null)
			{
				Destroy(gameObject);
			}
		}

		protected virtual void Awake()
		{
			mInstance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		
		protected virtual void OnDestroy()
		{
			mInstance = null;
		}

		private void OnApplicationQuit()
		{
			s_isShuttingDown = true;
		}
	}
}