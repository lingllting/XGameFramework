namespace AKBFramework.Example
{
	using UnityEngine;

	public class ExtensionExample : MonoBehaviour
	{
		private void Start()
		{
			QuickStart();
			CSharpExtensions();
			UnityExtensions();
		}

		private static void QuickStart()
		{
			// traditional style
			var playerPrefab = Resources.Load<GameObject>("no prefab don't run");
			var playerObj = Instantiate(playerPrefab);

			playerObj.transform.SetParent(null);
			playerObj.transform.localRotation = Quaternion.identity;
			playerObj.transform.localPosition = Vector3.left;
			playerObj.transform.localScale = Vector3.one;
			playerObj.layer = 1;
			playerObj.layer = LayerMask.GetMask("Default");

			Debug.Log("playerPrefab instantiated");

			// Extension's Style,same as above 
			Resources.Load<GameObject>("playerPrefab")
				.Instantiate()
				.transform
				.Parent(null)
				.LocalRotationIdentity()
				.LocalPosition(Vector3.left)
				.LocalScaleIdentity()
				.Layer(1)
				.Layer("Default")
				.ApplySelfTo(_ => { Debug.Log("playerPrefab instantiated"); });
		}

		private static void CSharpExtensions()
		{
			ClassExtention.Example();
			FuncOrActionOrEventExtension.Example();
			GenericExtention.Example();
			IEnumerableExtension.Example();
			IOExtension.Example();
			OOPExtension.Example();
			ReflectionExtension.Example();
			StringExtention.Example();
		}

		private static void UnityExtensions()
		{
			BehaviourExtension.Example();
			CameraExtension.Example();
			ColorExtension.Example();
			GameObjectExtension.Example();
			GraphicExtension.Example();
			ImageExtension.Example();
			ObjectExtension.Example();
			UnityActionExtension.Example();
			
			#region RectTransform

			#endregion

			#region Selectable

			#endregion

			#region Toggle

			#endregion
		}
	}
}