using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace XGameFramework
{
	public class UIEventListener : EventTrigger
	{
		private GameObject _gameObject;
		public GameObject CachedGameObject
		{
			get
			{
				if (_gameObject == null)
				{
					_gameObject = this.gameObject;
				}
				return _gameObject;
			}
		}
	  
		public Action<GameObject, object[]> onPointerUp;
		public Action<GameObject, object[]> onPointerDown;
		public Action<GameObject, object[]> onClick;
		public Action<GameObject> onBeginDrag;
		public Action<GameObject> onEndDrag;
		protected object[] mParameter;

		public override void OnPointerDown(PointerEventData eventData)
		{
			onPointerDown?.Invoke(CachedGameObject, mParameter);
		}
   
	
		public override void OnPointerUp(PointerEventData eventData)
		{
			onPointerUp?.Invoke(CachedGameObject, mParameter);
		}
		public override void OnPointerClick(PointerEventData eventData)
		{
			onClick?.Invoke(CachedGameObject, mParameter);
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			onBeginDrag?.Invoke(CachedGameObject);
		}
		
		public override void OnEndDrag(PointerEventData eventData)
		{
			onEndDrag?.Invoke(CachedGameObject);
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

		public static void Remove(GameObject go)
		{
			UIEventListener listener = go.GetComponent<UIEventListener>();
			Destroy(listener);
		}
#if DEBUG_UIEVENTLISTENER
		Vector3[] _fourCorners = new Vector3[4];
		private GUIStyle _guiStyle = new GUIStyle();

		bool Inside(Vector2 pos)
		{
			if (pos.x < _fourCorners[0].x)
			{
				return false;
			}

			if (pos.x > _fourCorners[2].x)
			{
				return false;
			}

			if (pos.y < _fourCorners[0].y)
			{
				return false;
			}

			if (pos.y > _fourCorners[2].y)
			{
				return false;
			}

			return true;
		}

		public void OnGUI()
		{
			_guiStyle.fontSize = 20;

			gameObject.GetComponent<RectTransform>().GetWorldCorners(_fourCorners);
			Rect rect = new Rect();
			Vector2 buttonPosition = (_fourCorners[2] + _fourCorners[0]) / 2;
			rect.position = buttonPosition;
			rect.position = new Vector2(rect.position.x, Screen.height - rect.position.y - 200);
			rect.size = _fourCorners[2] - _fourCorners[0];
			Color backupColor = GUI.color;
			GUI.color = Color.black;
			GUI.Label(rect, "B" + buttonPosition.ToString("f0")
				+ "\nM" + ((Vector2)Input.mousePosition).ToString("f0")
				+ "\nI " + Inside(Input.mousePosition)
				+ "\nD " + Input.GetMouseButton(0),
				_guiStyle);
			GUI.color = backupColor;
		}
#endif
	}
}
