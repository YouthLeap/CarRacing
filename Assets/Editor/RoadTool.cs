using UnityEngine;
using System.Collections;
using UnityEditor;

public class RoadTool : Editor {

	private static int changeCount = 0;

	[MenuItem("Assets/Replace Road Model #3")]
	private static void ReplaceRoadModel()
	{
		GameObject parentGO = Selection.activeGameObject;
		changeCount = 0;
		ChangeRoadModel (parentGO.transform);
		Debug.Log (parentGO.name);
		Debug.Log (changeCount);
	}

	private static void ChangeRoadModel(Transform parentTran)
	{
		if (parentTran.name.Contains ("cj_fangxiangpai1")) {
			Transform objectParentTran = parentTran.parent;
			//string objectName = parentTran.name;
			string objectName = "cj_fangxiangpai1";
			Vector3 objectPosition = parentTran.localPosition;
			Quaternion objectRotation = parentTran.localRotation;
			Vector3 objectScale = parentTran.localScale;
			MonoBehaviour.DestroyImmediate (parentTran.gameObject);
			GameObject luGO = Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath <GameObject> ("Assets/GGbondModel/daoju/cj_fangxiangpai1.prefab") as GameObject);
			Transform luTran = luGO.transform;
			luTran.parent = objectParentTran;
			luTran.name = objectName;
			luTran.localPosition = objectPosition;
			luTran.localRotation = objectRotation;
			luTran.localScale = objectScale;
			luTran.SetAsFirstSibling ();
			++ changeCount;
		} else {
			for (int i=0; i<parentTran.childCount; ++i)
				ChangeRoadModel (parentTran.GetChild (i));
		}
	}
	
	[MenuItem("Assets/Replace Road Model", true)]
	private static bool ValidateReplaceRoadModel()
	{
		if(Selection.activeObject == null)
			return false;
		return Selection.activeObject.GetType() == typeof(GameObject);
	}
}