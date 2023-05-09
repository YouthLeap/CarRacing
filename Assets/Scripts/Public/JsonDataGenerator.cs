using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

//using System.Text.RegularExpressions;

public class JsonDataGenerator : MonoBehaviour {
	
	public enum JsonDataName
	{
		PlayerData = 0,
		LiBaoBai = 1,
		LiBaoHei = 2,
		ChangWan = 3,
		ShenHe = 4,
	}

	public enum JsonDataType
	{
		INT,
		FLOAT,
		BOOL,
		STRING
	}

	public string sDataPath, sTypePath;
	public JsonDataName enumCurJsonDataName = JsonDataName.PlayerData, enumInspectorDataName = JsonDataName.ShenHe;
	public string sJsonText, sTypeText;
	public bool bDataHasInit = false;

	public Dictionary<string, object> miniJsonData = new Dictionary<string, object>();
	public Dictionary<string, object> miniTypeData = new Dictionary<string, object>();

	public void InitData()
	{
		#if UNITY_EDITOR

		enumInspectorDataName = enumCurJsonDataName;
		bDataHasInit = true;

		switch(enumCurJsonDataName)
		{
		case JsonDataName.PlayerData:
			sDataPath = "Assets/Resources/Data/PlayerData.txt";
			sTypePath = "Assets/Resources/Data/PlayerData_typeData.txt";
			break;
		case JsonDataName.ChangWan:
			sDataPath = "Assets/Resources/Data/PayVersionData/ChangWan.txt";
			sTypePath = "Assets/Resources/Data/PayVersionData/ChangWan_typeData.txt";
			break;
		case JsonDataName.ShenHe:
			sDataPath = "Assets/Resources/Data/PayVersionData/ShenHe.txt";
			sTypePath = "Assets/Resources/Data/PayVersionData/ShenHe_typeData.txt";
			break;
		case JsonDataName.LiBaoBai:
			sDataPath = "Assets/Resources/Data/PayVersionData/LiBaoBai.txt";
			sTypePath = "Assets/Resources/Data/PayVersionData/LiBaoBai_typeData.txt";
			break;
		case JsonDataName.LiBaoHei:
			sDataPath = "Assets/Resources/Data/PayVersionData/LiBaoHei.txt";
			sTypePath = "Assets/Resources/Data/PayVersionData/LiBaoHei_typeData.txt";
			break;
		}

		TextAsset dataTextAsset = AssetDatabase.LoadAssetAtPath(sDataPath, typeof(TextAsset)) as TextAsset;
		sJsonText = dataTextAsset.text;
		TextAsset typeTextAsset = AssetDatabase.LoadAssetAtPath(sTypePath, typeof(TextAsset)) as TextAsset;
		sTypeText = typeTextAsset.text;

		miniJsonData = Json.Deserialize(sJsonText) as Dictionary<string, object>;
		miniTypeData = Json.Deserialize(sTypeText) as Dictionary<string, object>;

       #endif
	}

	public void SavaData()
	{

#if UNITY_EDITOR
//		if (System.IO.File.Exists (sDataPath)) 
//		{
//			File.Delete (sDataPath);
//		}
//
//		if (System.IO.File.Exists (sTypePath)) 
//		{
//			File.Delete (sTypePath);
//		}

		using(StreamWriter writer = new StreamWriter(sDataPath, false, Encoding.UTF8))
		{
			writer.Write(Json.Serialize(miniJsonData));
			Debug.Log("数据成功保存: " + sDataPath);
		}

		using(StreamWriter writer = new StreamWriter(sTypePath, false, Encoding.UTF8))
		{
			writer.Write(Json.Serialize(miniTypeData));
			Debug.Log("数据成功保存: " + sTypePath);
		}
#endif
	}
}
