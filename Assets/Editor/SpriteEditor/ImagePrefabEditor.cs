using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ImagePrefabEditor{

	private static string[] PrefabPath = { "Assets/Resources/Image" };
	private static string[] ImagePath = { "Assets/Image/Image_DynamicLoad" };

	[MenuItem ("UIEditor/Create ImagePrefab")]
	static private void MakeAtlas()
	{
		string spriteDir = Application.dataPath +"/Resources/Image";

		if(!Directory.Exists(spriteDir))
		{
			Directory.CreateDirectory(spriteDir);
		}

		string[] prefabArr = AssetDatabase.FindAssets("t:Prefab", PrefabPath);
		if (prefabArr == null || prefabArr.Length <= 0) {
			Debug.Log("Prefab Can't Be Found!");
			return;
		}
		string prefabPathName;
		string prefabName;

		string[] imageArr = AssetDatabase.FindAssets("t:Texture", ImagePath);
		if (imageArr == null || imageArr.Length <= 0) {
			Debug.Log("Image Can't Be Found!");
			return;
		}
		string imagePathName;
		string imageName;

		List<string> prefabList = new List<string> ();
		for (int i=0; i<prefabArr.Length; ++i) {
			prefabPathName = AssetDatabase.GUIDToAssetPath (prefabArr [i]);
			prefabName = prefabPathName.Substring(prefabPathName.LastIndexOf('/') + 1);
			prefabName = prefabName.Remove(prefabName.LastIndexOf('.'));
			prefabList.Add(prefabName);
		}

		for (int i=0; i<imageArr.Length; ++i) {
			imagePathName = AssetDatabase.GUIDToAssetPath (imageArr [i]);
			imageName = imagePathName.Substring(imagePathName.LastIndexOf('/') + 1);
			imageName = imageName.Remove(imageName.LastIndexOf('.'));
			if(prefabList.Contains(imageName))
			{
				prefabList.Remove(imageName);
			}
			else
			{
				Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePathName);
				GameObject go = new GameObject(imageName);
				go.AddComponent<SpriteRenderer>().sprite = sprite;
				string prefabPath = "Assets/Resources/Image/" + imageName + ".prefab";
				PrefabUtility.CreatePrefab(prefabPath,go);
				GameObject.DestroyImmediate(go);
				Debug.Log ("Create : " + imageName);
			}
		}

		for(int i=0; i<prefabList.Count; ++i)
		{
			AssetDatabase.DeleteAsset ("Assets/Resources/Image/" + prefabList[i] + ".prefab");
			Debug.Log ("Delete : " + prefabList[i]);
		}
	}
}
