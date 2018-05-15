using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace AKBFramework
{
	public enum RFCGroup
	{
		None = 0,
		HidePanel = 1,
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class RFC : Attribute 
	{
		public RFCGroup group = RFCGroup.None;

		public RFC () { }
		public RFC (RFCGroup id) { group = id; }
	}

	public class RFCObject : MonoBehaviour
	{
		//远程调用方法结构体
		struct CachedRFC
		{
			public RFCGroup group;
			public MethodInfo func;
		}
		//远程调用方法列表
		List<CachedRFC> mRFCs = new List<CachedRFC>();
		//所有的LinkObject列表
		static List<RFCObject> mList = new List<RFCObject>();
		//LinkObject字典，用于快速查找
		static Dictionary<string, RFCObject> mDictionary = new Dictionary<string, RFCObject>();

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
		private bool mRemoveInActive;
		public bool RemoveInActive
		{
			get{return mRemoveInActive;}
			set{mRemoveInActive = true;}
		}

		private bool firstStart = false;
		protected virtual void Awake()
		{
			Register();
			RebuildMethodList();
			firstStart = true;
			mRemoveInActive = false;
		}

		protected virtual void OnDestroy () 
		{
			Unregister();
		}

		protected virtual void OnEnableCallBack(){}
		void OnEnable() 
		{
			if (firstStart)
			{
				firstStart = false;
			}
			else
			{
				if (RemoveInActive)
					Register();
				OnEnableCallBack();
			}
		}
		void OnDisable()
		{
			if (RemoveInActive)
				Unregister(); 
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
		/// 构建远程方法列表.
		/// </summary>
		private void RebuildMethodList ()
		{
			mRFCs.Clear();
			MonoBehaviour[] mbs = GetComponents<MonoBehaviour>();

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
					ent.group = tnc.group;
					mRFCs.Add(ent);
				}
			}
		}

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

		public static object SRFC<T>(string methodName, params object[] parameters) where T : RFCObject
		{
			string target = typeof(T).Name;
			RFCObject obj = RFCObject.Find(target);
			if (obj == null) 
			{
				Debug.LogError("Cannot Find The RFCObject: " + target);
				return null;
			}

			for (int i = 0; i < obj.mRFCs.Count; ++i)
			{
				CachedRFC rfc = obj.mRFCs[i];
				if (rfc.func.Name == methodName)
				{
					return rfc.func.Invoke(obj, parameters);
				}
			}

			return null;
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

		public static void SRFC(RFCGroup group, params object[] parameters)
		{
			for (int i = 0; i < mList.Count; ++i)
			{
				RFCObject obj = mList[i];
				for (int j = 0; j < obj.mRFCs.Count; ++j)
				{
					CachedRFC rfc = obj.mRFCs[j];
					if (rfc.group == group)
					{
						rfc.func.Invoke(obj, parameters);
					}
				}
			}
		}

		public void RFC(RFCGroup group, params object[] parameters)
		{
			for (int i = 0; i < mList.Count; ++i)
			{
				RFCObject obj = mList[i];
				for (int j = 0; j < obj.mRFCs.Count; ++j)
				{
					CachedRFC rfc = obj.mRFCs[j];
					if (rfc.group == group)
					{
						rfc.func.Invoke(obj, parameters);
					}
				}
			}
		}

		/// <summary>
		/// 给特定的LinkObject发送消息
		/// </summary>
		/// <param name="name">需要发送的LinkObject的名字.</param>
		/// <param name="parameter">发送的参数.</param>
		public void SendRFC (string name, string type, object parameters)
		{
			RFCObject obj = RFCObject.Find(name);

			if (obj != null)
			{
				obj.OnRFCHandler(this, type, parameters);
			}
			else
			{
				Debug.LogError("Send Message Error: LinkObject does no exist!");
			}
		}

		public static void SendSRFC(string name, string type, object parameters)
		{
			RFCObject obj = RFCObject.Find(name);

			if (obj != null)
			{
				obj.OnRFCHandler(null, type, parameters);
			}
			else
			{
				Debug.LogError("Send Message Error: LinkObject does no exist!");
			}
		}

		/// <summary>
		/// 处理其他Item发来的消息
		/// </summary>
		/// <param name="parameters">消息参数.</param>
		protected virtual void OnRFCHandler(RFCObject fromObj, string type, object parameters)
		{

		}
	}
}
