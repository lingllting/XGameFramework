using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

/// <summary>
/// 这个类用于动态创建的prefab，或者动态添加脚本的子物件实现动态绑定
/// </summary>
namespace AKBFramework.UI
{
	public class BaseItem : RFCObject 
	{
		//所有的BaseItem列表
		static List<BaseItem> mList = new List<BaseItem>();
		//BaseItem字典，用于快速查找
		static Dictionary<string, BaseItem> mDictionary = new Dictionary<string, BaseItem>();

		static public void Destory()
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

			RFCObject.Destroy();
		}

		/// <summary>
		/// 查找指定的BaseItem
		/// </summary>
		/// <param name="name">Key值.</param>
		static public BaseItem Find (string name)
		{
			if (mDictionary == null) return null;
			BaseItem item = null;
			mDictionary.TryGetValue(name, out item);
			return item;
		}

		/// <summary>
		/// 注册
		/// </summary>
		public void Register ()
		{
			mDictionary[Name] = this;
			mList.Add(this);
		}

		/// <summary>
		/// 注销
		/// </summary>
		void Unregister ()
		{
			if (mDictionary != null) mDictionary.Remove(Name);
			if (mList != null) mList.Remove(this);
		}

		[RFC]
		public void Open()
		{
			this.gameObject.SetActive(true);
			OnOpen();
		}

		[RFC]
		public void Close()
		{
			this.gameObject.SetActive(false);
			OnClose();
		}

		protected virtual void OnOpen()
		{
		}

		protected virtual void OnClose()
		{
		}

		protected override void Awake()
		{
			base.Awake();

			//寻找父节点，并自动添加
			if (transform.parent != null)
			{
				BaseItem parent = transform.parent.GetComponentInParent<BaseItem>();
				if (parent != null)
				{
					parent.AddChild(this);
				}
			}
			//注册
			Register();
		}

		protected override void OnDestroy () 
		{ 
			base.OnDestroy();
			Unregister(); 
		}

		//基类的事件回调，供各个面板复写
		public void OnItemClick(GameObject go)
		{
			string path = go.name;
			//所有的子节点调用
	//		foreach (KeyValuePair<string, BaseItem> pair in _dicBaseItems)
	//		{
	//			pair.Value.OnItemClick(go, path);
	//		}
			//自己调用
			OnItemClick(go, path);
			//父节点调用
	//		if (Parent != null)
	//		{
	//			Parent.OnItemClick(go, path);
	//		}
		}
		public virtual void OnItemClick(GameObject go, string pathInHierarchy){}
		public virtual void OnItemHover(GameObject go,bool bState){}
		public virtual void OnItemPress(GameObject go, bool state) {}
		public virtual void OnItemDrag(GameObject go, Vector2 delta) {}
		public virtual void OnItemDrop(GameObject go, GameObject draggedObject) {}

		//父节点
		private BaseItem mParent = null;
		protected BaseItem Parent
		{
			get{return mParent;}
			set{mParent = value;}
		}
		//父节点名字
		public virtual string ParentName
		{
			get{return mParent.Name;}
		}

		//子节点容器
		protected Dictionary<string, BaseItem> mBaseItemDict = new Dictionary<string, BaseItem>();
		public BaseItem this[string name]
		{
			get
			{
				if (!mBaseItemDict.ContainsKey(name))
				{
					Debug.LogError("没有找到子节点：" + name);
					return null;
				}
				return mBaseItemDict[name];
			}
		}

		/*********增删子节点接口开放**********/
		//添加子节点
		public virtual void AddChild(BaseItem item)
		{
			if (item == null)
				return;
			if (mBaseItemDict.ContainsValue(item))
			{
				Debug.LogError("BaseItem.AddChild Error: " + item.Name + " already exists.");
				return;
			}
			mBaseItemDict.Add(item.Name, item); 
			item.mParent = this;

			Debug.Log(this.Name + " Add Child: " + item.Name);
		}
		/*删除子节点*/
		public virtual void RemoveChild(string key)
		{
			if (!mBaseItemDict.ContainsKey(key))
			{
				Debug.LogError("BaseItem.RemoveChild Error: " + key + " doesn't exist.");
				return;
			}
			mBaseItemDict.Remove(key);
		}
		public virtual void RemoveChild(BaseItem item)
		{
			if (item == null)
				return;
			if (!mBaseItemDict.ContainsValue(item))
			{
				Debug.LogError("BaseItem.RemoveChild Error: " + item.Name + " doesn't exist.");
				return;
			}
			mBaseItemDict.Remove(item.Name);
		}

		/// <summary>
		/// 更新函数
		/// </summary>
		protected virtual void Update()
		{
		}

		/// <summary>
		/// 将面板的Public属性和面板Hierarchy的物件绑定，绑定规则是变量名和物件的名字相同，类型匹配
		/// </summary>
		private void AutoBindField()
		{
			Component[] components = this.gameObject.GetComponentsInChildren<Component>();
			FieldInfo[] fieldInfos = this.GetType().GetFields();

			for (int i = 0; i < fieldInfos.Length; i++)
			{
				if (fieldInfos[i].FieldType.ToString() == "UnityEngine.GameObject")
				{
					Transform tr = this.transform.FindRecursively(fieldInfos[i].Name);
					if (tr == null) continue;
					fieldInfos[i].SetValue(this, tr.gameObject);	
//					if(tr.gameObject.GetComponent<BoxCollider>() != null)
//					{
//						AddEventListener(tr.gameObject);
//					}
				}
				else
				{
					if(fieldInfos[i].GetValue(this) != null)
					{
						continue;
					}

					for(int j = 0; j < components.Length; j++)
					{
						if(fieldInfos[i].Name == components[j].name && fieldInfos[i].FieldType == components[j].GetType())
						{
							fieldInfos[i].SetValue(this, components[j]);
							break;
						}
					}
				}

//				MonoBehaviour mb = fieldInfos[i].GetValue(this) as MonoBehaviour;
//				if(mb == null)
//				{
//					continue;
//				}
//				if(mb.gameObject.GetComponent<BoxCollider>() != null)
//				{
//					AddEventListener(mb.gameObject);
//				}
			}
		}

		/// <summary>
		/// 添加UI监听函数
		/// </summary>
		protected void AddEventListener(GameObject go)
		{
			if(go.GetComponent<UIEventListener>() == null)
			{
				go.AddComponent<UIEventListener>();	
			}

			UIEventListener.Get(go).OnClick = OnItemClick;
		}

		#if UNITY_EDITOR
		void OnValidate()
		{
			AutoBindField();
		}
		#endif
	}
}
