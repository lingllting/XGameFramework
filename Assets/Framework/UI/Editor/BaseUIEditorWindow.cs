using UnityEngine;
using System.Collections;
using UnityEditor;

namespace AKBFramework.UI
{
	public class BaseUIEditorWindow : EditorWindow
	{
		private static BaseUIEditorWindow editor;
		private GameObject UI;
		private BaseItem[] panels;

		[MenuItem ("UI/Editor Window")]
		public static void Init()
		{
			editor = (BaseUIEditorWindow) EditorWindow.GetWindow(typeof(BaseUIEditorWindow), false, "BaseUI");
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			UI = (GameObject)EditorGUILayout.ObjectField("Panel", UI, typeof(GameObject), true);

			if (GUILayout.Button("Search"))
			{
				if (UI == null)
				{
					ShowNotification(new GUIContent("No object selected for searching"));
				}
				else
				{
					panels = UI.GetComponentsInChildren<BaseItem>();
				}
			}
			EditorGUILayout.EndHorizontal();

			if (UI != null)
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Bind"))
				{
					BaseUITools.BindPanel(UI, "AKBFramework");
				}
				if (GUILayout.Button("Remove"))
				{

				}
				EditorGUILayout.EndHorizontal();
			}

			if (panels != null)
				EditorGUILayout.LabelField(new GUIContent("Binded Items: "));
			if (panels != null && panels.Length > 0)
			{
				for (int i = 0; i < panels.Length; i++)
				{
					EditorGUILayout.BeginVertical();
					EditorGUILayout.ObjectField(panels[i], typeof(BaseItem), false);
					EditorGUILayout.EndVertical();
				}
			}
		}
	}
}
