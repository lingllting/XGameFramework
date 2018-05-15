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
                    mInstance = MonoSingletonCreator.CreateMonoSingleton<T>();
				}

				return mInstance;
			}
		}

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