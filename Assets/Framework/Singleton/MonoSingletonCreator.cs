using System.Reflection;
using UnityEngine;

namespace AKBFramework
{
	public sealed class MonoSingletonCreator
	{
		/// <summary>
		/// for unit test
		/// </summary>
		private static bool mIsUnitTestMode;

		public static bool IsUnitTestMode
		{
			get { return mIsUnitTestMode; }
			set { mIsUnitTestMode = value; }
		}

		public static T CreateMonoSingleton<T>(bool isDontDestroy) where T : MonoBehaviour, ISingleton
		{
			T instance = null;

			if (instance != null || (!mIsUnitTestMode && !Application.isPlaying)) return instance;
			instance = GameObject.FindObjectOfType(typeof(T)) as T;

			if (instance != null) return instance;
			MemberInfo info = typeof(T);
			var attributes = info.GetCustomAttributes(true);
			foreach (var atribute in attributes)
			{
				var defineAttri = atribute as MonoSingletonPath;
				if (defineAttri == null)
				{
					continue;
				}

                instance = CreateComponentOnGameObject<T>(defineAttri.PathInHierarchy, isDontDestroy);
				break;
			}

			if (instance == null)
			{
				var obj = new GameObject("(Singleton) " + typeof(T).Name);
                if (isDontDestroy)
					Object.DontDestroyOnLoad(obj);
				instance = obj.AddComponent<T>();
			}

			instance.OnSingletonInit();

			return instance;
		}

		private static T CreateComponentOnGameObject<T>(string path, bool dontDestroy) where T : MonoBehaviour
		{
			var obj = FindGameObject(null, path, true, dontDestroy);
			if (obj == null)
			{
				obj = new GameObject("(Singleton) " + typeof(T).Name);
				if (dontDestroy && !mIsUnitTestMode)
				{
					Object.DontDestroyOnLoad(obj);
				}
			}

			return obj.AddComponent<T>();
		}

		static GameObject FindGameObject(GameObject root, string path, bool build, bool dontDestroy)
		{
			if (path == null || path.Length == 0)
			{
				return null;
			}

			string[] subPath = path.Split('/');
			if (subPath == null || subPath.Length == 0)
			{
				return null;
			}

			return FindGameObject(null, subPath, 0, build, dontDestroy);
		}

		static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build, bool dontDestroy)
		{
			GameObject client = null;

			if (root == null)
			{
				client = GameObject.Find(subPath[index]);
			}
			else
			{
				var child = root.transform.Find(subPath[index]);
				if (child != null)
				{
					client = child.gameObject;
				}
			}

			if (client == null)
			{
				if (build)
				{
					client = new GameObject(subPath[index]);
					if (root != null)
					{
						client.transform.SetParent(root.transform);
					}

					if (dontDestroy && index == 0 && !mIsUnitTestMode)
					{
						GameObject.DontDestroyOnLoad(client);
					}
				}
			}

			if (client == null)
			{
				return null;
			}

			return ++index == subPath.Length ? client : FindGameObject(client, subPath, index, build, dontDestroy);
		}
	}
}