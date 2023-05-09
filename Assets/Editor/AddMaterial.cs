using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class AddMaterial : EditorWindow {

	//static Material spMaterial;
	static Material textMaterial;

//	[MenuItem ("MenuTool/AddMaterialForSprite", false)]
	static void AddMaterialForSprite () {

		//set material
		//spMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/SpriteDefault.mat");
		textMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/TextDefault.mat");
		//get prefab List
		List<Transform> prefabList = new List<Transform> ();

		string objPath  = Application.dataPath + "/" + "Prefab/UI/"; 
		DirectoryInfo d = new DirectoryInfo(objPath);
		ArrayList Flst= GetAll(d);

		//look though prefab,find image and sprite
		for (int i=0; i<Flst.Count; ++i) {

			string path= Flst[i].ToString();
			Debug.Log(path);
			Transform trans = AssetDatabase.LoadAssetAtPath<Transform>(path);

			DealTransform(trans);
		
			//PrefabUtility.ReplacePrefab(trans.gameObject, PrefabUtility.GetPrefabParent(trans.gameObject), ReplacePrefabOptions.Default);
			EditorUtility.SetDirty(trans);
			//DestroyImmediate(trans);
		}




	}


	static void DealTransform(Transform tran)
	{

		for (int i=0; i<tran.childCount; ++i) {
			DealTransform(tran.GetChild(i));
		}

//		if (tran.name == "ScrollRect") {
//			return;
//		}

		SpriteRenderer spRender = tran.GetComponent<SpriteRenderer> ();
		if (spRender != null ) {
			if(spRender.material ==null)
			{
				spRender.material = textMaterial;
				Debug.Log("change sp "+textMaterial.name);
			}

		}

		Image img = tran.GetComponent<Image> ();
		if (img != null) {

			if(img.material !=null)
			{
				img.material = textMaterial;
				Debug.Log("change image "+textMaterial.name);
			}
		}

		Text text = tran.GetComponent<Text> ();
		if (text != null ) {
			text.material = textMaterial;
			Debug.Log("change text "+textMaterial.name);
		}


	}


	static ArrayList FileList = new ArrayList();
	static ArrayList GetAll(DirectoryInfo dir)//搜索文件夹中的文件
	{
		FileInfo[] allFile = dir.GetFiles();
		foreach (FileInfo fi in allFile)
		{
			if(fi.Extension == ".prefab")
			{
				string p=fi.FullName;
				p = p.Substring(p.IndexOf("Assets"));
				FileList.Add(p);
			}
		}
		
		DirectoryInfo[] allDir= dir.GetDirectories();
		foreach (DirectoryInfo d in allDir)
		{
			//Debug.LogWarning( d.FullName);
			GetAll(d);
		}
		return FileList;
	}

}
