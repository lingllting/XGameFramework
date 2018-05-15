using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace AKBFramework.UI
{
	public class UIEventListener : CachedMonoBehaviour, IPointerClickHandler
	{
		public Action<GameObject> OnClick;
		private object[] mParameter;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (OnClick != null)
			{
				OnClick(CachedGameObject);
			}
		}

		public static UIEventListener Get(GameObject go, params object[] param)
		{
			UIEventListener listener = go.GetComponent<UIEventListener>();
			if (listener == null)
			{
				listener = go.AddComponent<UIEventListener>();
				listener.mParameter = param;
			}
			return listener;
		}
	}
}
