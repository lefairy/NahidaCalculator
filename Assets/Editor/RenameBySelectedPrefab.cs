using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RenameByPrefabHierarchy : EditorWindow
{
	static bool fisrt = false;
	static List<Unmatched> unmatch;
	static Transform temp;
	struct Unmatched
	{
		public Transform obj, parent;
		public Unmatched(Transform obj)
		{
			this.obj = obj;
			this.parent = obj.parent;
		}
	}
	[MenuItem("GameObject/重命名并连接预制件", false, -1)]
	public static void ShowWindow()
	{
		var window = GetWindow<RenameByPrefabHierarchy>("");
		window.minSize = new Vector2(0, 0);
		window.position = new Rect(0, 0, 0, 0);
		fisrt = true;
	}

	void OnGUI()
	{
		if (fisrt)
		{
			fisrt = false;
			EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, string.Empty, 0);
		}

		if (Event.current.commandName == "ObjectSelectorClosed")
		{
			Close();
		}
		else if (Event.current.commandName != "ObjectSelectorUpdated")
		{
			return;
		}

		Object selected = EditorGUIUtility.GetObjectPickerObject();
		if (selected == null)
		{
			return;
		}
		GameObject selectedPrefab = selected as GameObject;
		if (selectedPrefab == null || PrefabUtility.GetPrefabAssetType(selectedPrefab) == PrefabAssetType.NotAPrefab)
		{
			Debug.LogWarning("请选择一个有效的预制体");
			return;
		}

		// 使用组名方便批量撤销
		Undo.IncrementCurrentGroup();
		int undoGroup = Undo.GetCurrentGroup();

		unmatch = new List<Unmatched>();
		temp = new GameObject("TempParent").transform;
		Undo.RegisterCreatedObjectUndo(temp.gameObject, "Create TempParent");

		foreach (GameObject go in Selection.gameObjects)
		{
			Undo.RegisterFullObjectHierarchyUndo(go, "Rename and Connect Prefab");
			// 跳过根对象重命名，只处理子对象
			RenameOnlyChildren(go.transform, selectedPrefab.transform);

			// 重新连接到选中的预制件
			var settings = new ConvertToPrefabInstanceSettings()
			{
				componentsNotMatchedBecomesOverride = true,
				gameObjectsNotMatchedBecomesOverride = true,
				recordPropertyOverridesOfMatches = true,
				changeRootNameToAssetName = true
			};
			PrefabUtility.ConvertToPrefabInstance(go, selectedPrefab, settings, InteractionMode.UserAction);
			RevertUnmatchedUIComponentsToPrefab(go);
		}
		foreach (var unmatched in unmatch)
		{
			Undo.SetTransformParent(unmatched.obj, unmatched.parent, "Restore GameObject");
		}
		Undo.DestroyObjectImmediate(temp.gameObject);
		Undo.CollapseUndoOperations(undoGroup);
		Close();
	}

	private static void RenameOnlyChildren(Transform target, Transform template)
	{
		for (int i = 0; i < target.childCount; i++)
		{
			Transform targetChild = target.GetChild(i);
			if (i < template.childCount)
			{
				Transform templateChild = template.GetChild(i);
				targetChild.name = templateChild.name;
				// 递归处理子级
				RenameOnlyChildren(targetChild, templateChild);
			}
			else
			{
				unmatch.Add(new Unmatched(targetChild));
				Undo.SetTransformParent(targetChild, temp, "Move GameObject");
				i--;
			}
		}
	}
	private static void RevertUnmatchedUIComponentsToPrefab(GameObject go)
	{
		var components = go.GetComponents<Component>();
		foreach (var comp in components)
		{
			if (comp == null) continue;

			if (comp is RectTransform ||
				comp is CanvasRenderer ||
				//comp is LayoutElement ||
				comp is Mask ||
				comp is RectMask2D ||
				comp is active)
			{
				//if (!PrefabUtility.IsPartOfPrefabAsset(comp))
				//{
				//	PrefabUtility.RevertAddedComponent(comp, InteractionMode.UserAction);
				//}
				//else
				//{
				try { PrefabUtility.RevertObjectOverride(comp, InteractionMode.UserAction); }
				catch { PrefabUtility.RevertAddedComponent(comp, InteractionMode.UserAction); }
				//}
			}
		}

		// 递归处理子对象
		for (int i = 0; i < go.transform.childCount; i++)
		{
			RevertUnmatchedUIComponentsToPrefab(go.transform.GetChild(i).gameObject);
		}
	}
}
