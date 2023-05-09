using UnityEngine;
using System.Collections;
using UnityEditor;
using MiniJSON;
using System.IO;
using System;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine.UI;

public class LevelEditor : Editor {

	[MenuItem("LevelEditor/Show Item By Data", false)]
	static void ShowLevelItem()
	{
		ShowItemData ();
	}
	[MenuItem("LevelEditor/Show Item By Data", true)]
	static bool CheckShowLevelItem()
	{
		return Selection.activeGameObject != null;
	}

	[MenuItem("LevelEditor/Show Road By Data", false)]
	static void ShowRoadItem()
	{
		ShowRoadData ();
	}
	[MenuItem("LevelEditor/Show Road By Data", true)]
	static bool CheckShowRoadItem()
	{
		return Selection.activeGameObject != null;
	}

	[MenuItem("LevelEditor/Save Item In Data", false)]
	static void SaveLevelItem()
	{
		SaveItemData ();
	}
	[MenuItem("LevelEditor/Save Item In Data", true)]
	static bool CheckSaveLevelItem()
	{
		return Selection.activeGameObject != null;
	}

	[MenuItem("LevelEditor/Save Road In Data", false)]
	static void SaveRoadItem()
	{
		SaveRoadData ();
	}
	[MenuItem("LevelEditor/Save Road In Data", true)]
	static bool CheckSaveRoadItem()
	{
		return Selection.activeGameObject != null;
	}

	[MenuItem("LevelEditor/Clear", false)]
	static void Clear()
	{
		ClearData ();
	}
	[MenuItem("LevelEditor/Clear", true)]
	static bool CheckClear()
	{
		return Selection.activeGameObject != null;
	}

	static void ClearData()
	{
		Transform parentTran = Selection.activeTransform;
		while (parentTran.childCount > 0) {
			DestroyImmediate (parentTran.GetChild (0).gameObject);
		}
	}

	[MenuItem("LevelEditor/Change Text Font", false)]
	static void ChangeTextFont()
	{
		StartChangeTextFont ();
	}
	[MenuItem("LevelEditor/Change Text Font", true)]
	static bool CheckChangeTextFont()
	{
		return Selection.activeGameObject != null;
	}

	static void ShowRoadData()
	{
		Transform parentTran = Selection.activeTransform;
		Dictionary<string, object> jsonData = Json.Deserialize(ReadFile ("Assets/Resources/Data/RoadItemJsonData.txt")) as Dictionary<string, object>;
		GameObject lineGO;
		Transform lineTran;
		for (int i=0; i<jsonData.Count; ++i) {
			Dictionary<string, object> itemJsonData = new Dictionary<string, object>();
			itemJsonData = Json.Deserialize(jsonData["road" + i].ToString()) as Dictionary<string, object>;
			lineGO = Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath <GameObject> ("Assets/Prefab/UI/UISceneUI/Items/RoadItem.prefab") as GameObject);
			lineGO.SetActive(true);
			lineGO.name = "RoadItem" + i;
			lineTran = lineGO.transform;
			lineTran.parent = parentTran;
			string[] positionArr = itemJsonData["position"].ToString().Split('|');
			string[] rotationArr = itemJsonData["rotation"].ToString().Split('|');
			lineTran.localPosition = new Vector3(float.Parse(positionArr[0]), float.Parse(positionArr[1]), float.Parse(positionArr[2]));
			lineTran.eulerAngles = new Vector3(float.Parse(rotationArr[0]), float.Parse(rotationArr[1]), float.Parse(rotationArr[2]));
			lineTran.localScale = Vector3.one;
		}
	}

	static void ShowItemData()
	{
		Transform parentTran = Selection.activeTransform;
		Dictionary<string, object> jsonData = Json.Deserialize(ReadFile ("Assets/Resources/Data/LevelItemJsonData.txt")) as Dictionary<string, object>;
		GameObject levelGO;
		Transform levelTran;
		for (int i=0; i<jsonData.Count; ++i) {
			levelGO = Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath <GameObject> ("Assets/Prefab/UI/UISceneUI/Items/LevelItem.prefab") as GameObject);
			levelGO.SetActive(true);
			levelGO.name = "LevelItem" + i;
			levelTran = levelGO.transform;
			levelTran.parent = parentTran;
			string[] posArr = jsonData["level" + i].ToString().Split('|');
			levelTran.localPosition = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
			levelTran.localScale = Vector3.one;
		}
	}
	
	static void SaveItemData()
	{
		Transform parentTran = Selection.activeTransform;
		Dictionary<string, object> jsonData = new Dictionary<string, object>();
		Vector3 levelPos;
		string levelPosStr;
		for (int i=0; i<parentTran.childCount; ++i) {
			levelPos = parentTran.GetChild(i).localPosition;
			levelPosStr = levelPos.x + "|" + levelPos.y + "|" + levelPos.z;
			jsonData["level" + i] = levelPosStr;
		}
		CreateOrWriteFile ("Assets/Resources/Data/LevelItemJsonData.txt", Json.Serialize (jsonData));
		AssetDatabase.Refresh ();
		Debug.Log ("Save Finish!");
	}

	static void SaveRoadData()
	{
		Transform parentTran = Selection.activeTransform;
		Dictionary<string, object> jsonData = new Dictionary<string, object>();
		Vector3 roadPos;
		Vector3 roadRotation;
		string positionStr, rotationStr;
		for (int i=0; i<parentTran.childCount; ++i) {
			Dictionary<string, object> itemJsonData = new Dictionary<string, object>();
			roadPos = parentTran.GetChild(i).localPosition;
			roadRotation = parentTran.GetChild(i).localRotation.eulerAngles;
			positionStr = roadPos.x + "|" + roadPos.y + "|" + roadPos.z;
			rotationStr = roadRotation.x + "|" + roadRotation.y + "|" + roadRotation.z;
			itemJsonData["position"] = positionStr;
			itemJsonData["rotation"] = rotationStr;
			jsonData["road" + i] = Json.Serialize (itemJsonData);
		}
		CreateOrWriteFile ("Assets/Resources/Data/RoadItemJsonData.txt", Json.Serialize (jsonData));
		Debug.Log ("Save Finish!");
	}

	static void CreateOrWriteFile (string path, string info)
	{
		FileStream fs = new FileStream (path, FileMode.Create, FileAccess.Write);
		StreamWriter sw = new StreamWriter (fs);
		fs.SetLength (0);	/*清空文件*/
		sw.WriteLine (info);
		sw.Close ();
		sw.Dispose ();
	}

	static string ReadFile (string path)
	{
		string fileContent; 
		StreamReader sr = null;
		try{
			sr = File.OpenText(path);
		}
		catch(Exception e){
			Debug.Log(e.Message);
			return null;
		}
		
		while ((fileContent = sr.ReadLine()) != null) {
			break; 
		}
		sr.Close ();
		sr.Dispose ();
		return fileContent;
	}

	static void StartChangeTextFont()
	{
		Transform parentTran = Selection.activeTransform;
		Transform itemTran;
		Text levelText;
		Font textFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Font/BMFont/BMFont.fontsettings");
		for(int i=0; i<parentTran.childCount; ++i)
		{
			itemTran = parentTran.Find("LevelItem" + i);
			if(itemTran == null)
				return;
			itemTran = itemTran.Find("LevelBtn").Find("LevelText");
			itemTran.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			levelText = itemTran.GetComponent<Text>();
			levelText.font = textFont;
			levelText.fontSize = 0;
			levelText.lineSpacing = 1;
			levelText.supportRichText = false;
		}
	}
}