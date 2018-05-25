using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace AKBFramework
{
	public enum RFCType
	{
		None = 0,
		//销毁监听者
		DestroyListener = 1,
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class RFC : Attribute 
	{
		public RFCType Type = RFCType.None;
		public object Param;

		public RFC () { }
		public RFC (RFCType type, object param = null) { Type = type; Param = param;}
	}

	public class RFCObject : MonoBehaviour
	{
		//远程调用方法结构体
		struct CachedRFC
		{
			public RFCType type;
			public object Param;
			public MethodInfo func;
		}

		//远程调用方法列表
		List<CachedRFC> mRFCs = new List<CachedRFC>();
		//所有的RFCObject列表
		static List<RFCObject> mList = new List<RFCObject>();
		//LinkObject字典，用于快速查找
		static Dictionary<string, RFCObject> mDictionary = new Dictionary<string, RFCObject>();

		public MonoBehaviour ProxyTarget{get{ return mProxyTarget;}}
		//代理对象
		MonoBehaviour mProxyTarget = null;

		//自己的名字
		private string _name = null;
		protected virtual string Name
		{
			get
			{
				if (_name == null)
					_name = this.GetType().ToString();
				return _name;
			}
			set{_name = value;}
		}

		//隐藏时移除
		private bool mRemoveInActive = false;
		public bool RemoveInActive
		{
			get{return mRemoveInActive;}
			set{mRemoveInActive = value;}
		}

		#region 静态函数
		/// <summary>
		/// 查找指定的BaseItem
		/// </summary>
		/// <param name="name">Key值.</param>
		static RFCObject Find (string name)
		{
			if (mDictionary == null) return null;
			RFCObject obj = null;
			mDictionary.TryGetValue(name, out obj);
			return obj;
		}

		public static void Destroy()
		{
			for (int i = 0; i < mList.Count; i++)
			{
				mList[i] = null;
			}
			List<string> keys = new List<string>(mDictionary.Keys);
			for (int i = 0; i < keys.Count; i++)
			{
				mDictionary[keys[i]] = null;
			}

			mList.Clear();
			mDictionary.Clear();
		}

//		public static object SRFC<T>(string methodName, params object[] parameters) where T : RFCObject
//		{
//			string target = typeof(T).Name;
//			RFCObject obj = RFCObject.Find(target);
//			if (obj == null) 
//			{
//				Debug.LogError("Cannot Find The RFCObject: " + target);
//				return null;
//			}
//
//			for (int i = 0; i < obj.mRFCs.Count; ++i)
//			{
//				CachedRFC rfc = obj.mRFCs[i];
//				if (rfc.func.Name == methodName)
//				{
//					return rfc.func.Invoke(obj, parameters);
//				}
//			}
//			return null;
//		}
//
//		public static void SRFC(RFCType group, object param, params object[] parameters)
//		{
//			for (int i = 0; i < mList.Count; ++i)
//			{
//				RFCObject obj = mList[i];
//				for (int j = 0; j < obj.mRFCs.Count; ++j)
//				{
//					CachedRFC rfc = obj.mRFCs[j];
//					if (rfc.type == group)
//					{
//						rfc.func.Invoke(obj, parameters);
//					}
//				}
//			}
//		}

		/// <summary>
		/// 手动注册.
		/// </summary>
		/// <param name="target">注册目标.</param>
		public static void ManualRegister(MonoBehaviour target)
		{
			RFCObject rfcObject = target.gameObject.GetOrAddComponent<RFCObject>();
			rfcObject.SetProxy(target);
		}
		#endregion

		protected virtual void Awake()
		{
			Register();
			RebuildMethodList();
		}

		protected virtual void OnDestroy () 
		{
			RFC(RFCType.DestroyListener, this.GetType());
			Unregister();
		}

		void OnEnable() 
		{
			if (RemoveInActive)
			{
				Register();
			}
		}

		void OnDisable()
		{
			if (RemoveInActive)
			{
				Unregister();
			}
		}

		/// <summary>
		/// 注册
		/// </summary>
		void Register ()
		{
			mDictionary[Name] = this;
			if (!mList.Contains (this))
			{
				mList.Add (this);
			}
		}

		/// <summary>
		/// 注销
		/// </summary>
		void Unregister ()
		{
			if (mDictionary != null) mDictionary.Remove(Name);
			if (mList != null) mList.Remove(this);
		}

		/// <summary>
		/// 设置代理对象
		/// </summary>
		/// <param name="target">代理对象.</param>
		public void SetProxy(MonoBehaviour target)
		{
			mProxyTarget = target;
			Name = target.transform.name;
			mRFCs.Clear();
			MethodInfo[] methods = target.GetType().GetMethods(
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Instance);

			for (int b = 0; b < methods.Length; ++b)
			{
				if (methods[b].IsDefined(typeof(RFC), true))
				{
					CachedRFC ent = new CachedRFC();
					ent.func = methods[b];
					RFC tnc = (RFC)ent.func.GetCustomAttributes(typeof(RFC), true)[0];
					ent.type = tnc.Type;
					ent.Param = tnc.Param;
					mRFCs.Add(ent);
				}
			}
		}

		/// <summary>
		/// 构建远程方法列表.
		/// </summary>
		private void RebuildMethodList ()
		{
			mRFCs.Clear();

			MethodInfo[] methods = this.GetType().GetMethods(
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Instance);

			for (int b = 0; b < methods.Length; ++b)
			{
				if (methods[b].IsDefined(typeof(RFC), true))
				{
					CachedRFC ent = new CachedRFC();
					ent.func = methods[b];
					RFC tnc = (RFC)ent.func.GetCustomAttributes(typeof(RFC), true)[0];
					ent.type = tnc.Type;
					ent.Param = tnc.Param;
					mRFCs.Add(ent);
				}
			}
		}

		public void RFC<T>(string methodName, params object[] parameters)
		{
			string target = typeof(T).Name;
			RFCObject obj = RFCObject.Find(target);
			if (obj == null) 
			{
				Debug.LogError("Cannot Find The RFCObject: " + target);
				return;
			}

			for (int i = 0; i < obj.mRFCs.Count; ++i)
			{
				CachedRFC rfc = obj.mRFCs[i];
				if (rfc.func.Name == methodName)
				{
					rfc.func.Invoke(obj, parameters);
				}
			}
		}

		public void RFC(RFCType type, object param, params object[] parameters)
		{
			for (int i = 0; i < mList.Count; ++i)
			{
				RFCObject obj = mList[i];
				for (int j = 0; j < obj.mRFCs.Count; ++j)
				{
					CachedRFC rfc = obj.mRFCs[j];
					if (rfc.type == type && rfc.Param == param)
					{
						if(obj.ProxyTarget != null)
						{
							rfc.func.Invoke(obj.ProxyTarget, parameters);
						}
						else
						{
							rfc.func.Invoke(obj, parameters);
						}
					}
				}
			}
		}
	}
}
