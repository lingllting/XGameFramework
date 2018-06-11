namespace AKBFramework
{
	using UnityEngine;

	public abstract class MonoSingleton<T> : MonoBehaviour,ISingleton where T : MonoSingleton<T>
	{
		protected static T mInstance = null;

		public static T Instance
		{
			get 
			{
				if (mInstance == null) 
				{
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>(true);
				}

				return mInstance;
			}
		}

        public static void Initialize(bool isDontDestory = true)
        {
            mInstance = MonoSingletonCreator.CreateMonoSingleton<T>(isDontDestory);
        }

        public static bool IsInitialized { get { return mInstance != null; } }

		public virtual void OnSingletonInit()
		{

		}

		public virtual void Dispose()
		{
            if (MonoSingletonCreator.IsUnitTestMode)
			{
				Transform curTrans = transform;
				do
				{
					var parent = curTrans.parent;
					DestroyImmediate(curTrans.gameObject);
					curTrans = parent;
				} while (curTrans != null);

				mInstance = null;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		void Awake()
		{
			mInstance = this as T;
		}
		
		protected virtual void OnDestroy()
		{
			mInstance = null;
		}
	}
}