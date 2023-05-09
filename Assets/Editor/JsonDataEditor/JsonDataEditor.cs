using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//自定义TitleGenerator脚本
[CustomEditor(typeof(JsonDataGenerator))] 
//在编辑模式下执行脚本
[ExecuteInEditMode]
public class JsonDataEditor : Editor {

	Dictionary<string, bool> bFoldout = new Dictionary<string, bool>();
	private float fCommantWidth = 100, fTypeWidth = 100, fKeyWidth = 120, fValueWidth = 220;

	public override void OnInspectorGUI (){
		JsonDataGenerator hJsonDataGenerator = (JsonDataGenerator) target;
		hJsonDataGenerator.enumCurJsonDataName = (JsonDataGenerator.JsonDataName)EditorGUILayout.EnumPopup("选择文件", hJsonDataGenerator.enumCurJsonDataName);

		if (GUILayout.Button("初始化数据",  GUILayout.Width(100)))
			hJsonDataGenerator.InitData();

		if(hJsonDataGenerator.enumCurJsonDataName != hJsonDataGenerator.enumInspectorDataName)
			hJsonDataGenerator.bDataHasInit = false;

		EditorGUILayout.Space();

		if(hJsonDataGenerator.bDataHasInit)
		{
			if (GUILayout.Button("保存数据",  GUILayout.Width(100)))
				hJsonDataGenerator.SavaData();

			EditorGUILayout.Space();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("备注", GUILayout.Width(fCommantWidth));
			GUILayout.Label("数据类型", GUILayout.Width(fTypeWidth));
			GUILayout.Label("键", GUILayout.Width(fKeyWidth));
			GUILayout.Label("值", GUILayout.Width(fValueWidth));
			GUILayout.EndHorizontal();
		}
	
		List<string> buffer = new List<string>(hJsonDataGenerator.miniJsonData.Keys);
		foreach(string key in buffer)
		{
			Dictionary<string, object> secondJsonData = hJsonDataGenerator.miniJsonData[key] as Dictionary<string, object>;

			if(secondJsonData != null && secondJsonData.Count > 0)
			{
				if(bFoldout.ContainsKey(key))
				{
				    bFoldout[key] = EditorGUILayout.Foldout(bFoldout[key], key);
				}else
				{
					bFoldout.Add(key, false);
					bFoldout[key] = EditorGUILayout.Foldout(bFoldout[key], key);
				}

				if(bFoldout[key])
				{
					List<string> secondBuffer = new List<string>(secondJsonData.Keys);
					EditorGUI.indentLevel += 1;

					if(!hJsonDataGenerator.miniTypeData.ContainsKey(key))
					{
						hJsonDataGenerator.miniTypeData.Add(key, new Dictionary<string, object>());
					}
					Dictionary<string, object> secondJsonType = hJsonDataGenerator.miniTypeData[key] as Dictionary<string, object>;

					foreach(string secondKey in secondBuffer)
					{
						EditorGUILayout.BeginHorizontal();

						string sSecondComments;
						string[] sArrSecondComments;

						if(!secondJsonType.ContainsKey(secondKey))
						{
							secondJsonType.Add(secondKey, "");
						}
						sSecondComments = secondJsonType[secondKey].ToString();
						sArrSecondComments = sSecondComments.Split('|');

						string sSecondHint, sSecondType; 
						if(sArrSecondComments.Length < 1)
						{
							sSecondHint = "null";
							sSecondType = "";
						}else if(sArrSecondComments.Length < 2)
						{
							sSecondHint = sArrSecondComments[0];
							sSecondType = "";
						}else
						{
							sSecondHint = sArrSecondComments[0];
							sSecondType = sArrSecondComments[1];
						}
						
						secondJsonType[secondKey] = (sSecondHint = EditorGUILayout.TextField(sSecondHint, GUILayout.Width(100))) + "|" + sSecondType;
						
						JsonDataGenerator.JsonDataType enumSecondType;
						
						if(sSecondType.CompareTo("INT") == 0 || sSecondType.CompareTo("FLOAT") == 0 || sSecondType.CompareTo("STRING") == 0 || sSecondType.CompareTo("BOOL") == 0)
						{
							enumSecondType = (JsonDataGenerator.JsonDataType)System.Enum.Parse(typeof(JsonDataGenerator.JsonDataType), sSecondType);
							secondJsonType[secondKey] = sSecondHint + "|" + ((JsonDataGenerator.JsonDataType)EditorGUILayout.EnumPopup(enumSecondType, GUILayout.Width(100))).ToString();
						}else
						{
							enumSecondType = JsonDataGenerator.JsonDataType.STRING;
							secondJsonType[secondKey] = sSecondHint + "|" + ((JsonDataGenerator.JsonDataType)EditorGUILayout.EnumPopup(enumSecondType, GUILayout.Width(100))).ToString();
						}
						//			hJsonDataGenerator.miniTypeData[key] = sHint + "|" + sType;
						
						string sTempSecondValue = secondJsonData[secondKey].ToString();
						switch(enumSecondType)
						{
						case JsonDataGenerator.JsonDataType.STRING:
							GUILayout.Label(secondKey, GUILayout.Width(fKeyWidth));
							secondJsonData[secondKey] = EditorGUILayout.TextField(secondJsonData[secondKey].ToString(), GUILayout.Width(fValueWidth));
							break;
						case JsonDataGenerator.JsonDataType.INT:
							int iValue;
							if(int.TryParse(sTempSecondValue, out iValue))
							{
								GUILayout.Label(secondKey, GUILayout.Width(fKeyWidth));
								secondJsonData[secondKey] = EditorGUILayout.IntField(iValue, GUILayout.Width(fValueWidth));
							}else
							{
								GUILayout.Label(secondKey, GUILayout.Width(fKeyWidth));
								secondJsonData[secondKey] = EditorGUILayout.TextField(secondJsonData[secondKey].ToString(), GUILayout.Width(fValueWidth));
							}
							break;
						case JsonDataGenerator.JsonDataType.FLOAT:
							float fValue;
							if(float.TryParse(sTempSecondValue, out fValue))
							{
								GUILayout.Label(secondKey, GUILayout.Width(fKeyWidth));
								secondJsonData[secondKey] = EditorGUILayout.FloatField(fValue, GUILayout.Width(fValueWidth));
							}else
							{
								GUILayout.Label(secondKey, GUILayout.Width(fKeyWidth));
								secondJsonData[secondKey] = EditorGUILayout.TextField(secondJsonData[secondKey].ToString(), GUILayout.Width(fValueWidth));
							}
							break;
						case JsonDataGenerator.JsonDataType.BOOL:
							bool bValue;
							if(bool.TryParse(sTempSecondValue, out bValue))
							{
								GUILayout.Label(secondKey, GUILayout.Width(fKeyWidth));
								secondJsonData[secondKey] = EditorGUILayout.Toggle(bValue, GUILayout.Width(fValueWidth));
							}else
							{
								GUILayout.Label(secondKey, GUILayout.Width(fKeyWidth));
								secondJsonData[secondKey] = EditorGUILayout.TextField(secondJsonData[secondKey].ToString(), GUILayout.Width(fValueWidth));
							}
							break;
						}

						EditorGUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel -= 1;
				}
				continue;
			}

			EditorGUILayout.BeginHorizontal();

			string sComments;
			string[] sArrComments;
			if(!hJsonDataGenerator.miniTypeData.ContainsKey(key))
			{
				hJsonDataGenerator.miniTypeData.Add(key, "");
			}
			sComments = hJsonDataGenerator.miniTypeData[key].ToString();
			sArrComments = sComments.Split('|');
			string sHint, sType; 
			if(sArrComments.Length < 1)
			{
				sHint = "null";
				sType = "";
			}else if(sArrComments.Length < 2)
			{
				sHint = sArrComments[0];
				sType = "";
			}else
			{
				sHint = sArrComments[0];
				sType = sArrComments[1];
			}

			hJsonDataGenerator.miniTypeData[key] = (sHint = EditorGUILayout.TextField(sHint, GUILayout.Width(100))) + "|" + sType;

			JsonDataGenerator.JsonDataType enumType;

			if(sType.CompareTo("INT") == 0 || sType.CompareTo("FLOAT") == 0 || sType.CompareTo("STRING") == 0 || sType.CompareTo("BOOL") == 0)
			{
				enumType = (JsonDataGenerator.JsonDataType)System.Enum.Parse(typeof(JsonDataGenerator.JsonDataType), sType);
				hJsonDataGenerator.miniTypeData[key] = sHint + "|" + ((JsonDataGenerator.JsonDataType)EditorGUILayout.EnumPopup(enumType, GUILayout.Width(100))).ToString();
			}else
			{
				enumType = JsonDataGenerator.JsonDataType.STRING;
				hJsonDataGenerator.miniTypeData[key] = sHint + "|" + ((JsonDataGenerator.JsonDataType)EditorGUILayout.EnumPopup(enumType, GUILayout.Width(100))).ToString();
			}

			string sTempValue = hJsonDataGenerator.miniJsonData[key].ToString();
			switch(enumType)
			{
			case JsonDataGenerator.JsonDataType.STRING:
				GUILayout.Label(key, GUILayout.Width(fKeyWidth));
				hJsonDataGenerator.miniJsonData[key] = EditorGUILayout.TextField(hJsonDataGenerator.miniJsonData[key].ToString(), GUILayout.Width(fValueWidth));
				break;
			case JsonDataGenerator.JsonDataType.INT:
				int iValue;
				if(int.TryParse(sTempValue, out iValue))
				{
					GUILayout.Label(key, GUILayout.Width(fKeyWidth));
				    hJsonDataGenerator.miniJsonData[key] = EditorGUILayout.IntField(iValue, GUILayout.Width(fValueWidth));
				}else
				{
					GUILayout.Label(key, GUILayout.Width(fKeyWidth));
					hJsonDataGenerator.miniJsonData[key] = EditorGUILayout.TextField(hJsonDataGenerator.miniJsonData[key].ToString(), GUILayout.Width(fValueWidth));
				}
				break;
			case JsonDataGenerator.JsonDataType.FLOAT:
				float fValue;
				if(float.TryParse(sTempValue, out fValue))
				{
					GUILayout.Label(key, GUILayout.Width(fKeyWidth));
					hJsonDataGenerator.miniJsonData[key] = EditorGUILayout.FloatField(fValue, GUILayout.Width(fValueWidth));
				}else
				{
					GUILayout.Label(key, GUILayout.Width(fKeyWidth));
					hJsonDataGenerator.miniJsonData[key] = EditorGUILayout.TextField(hJsonDataGenerator.miniJsonData[key].ToString(), GUILayout.Width(fValueWidth));
				}
				break;
			case JsonDataGenerator.JsonDataType.BOOL:
				bool bValue;
				if(bool.TryParse(sTempValue, out bValue))
				{
					GUILayout.Label(key, GUILayout.Width(fKeyWidth));
					hJsonDataGenerator.miniJsonData[key] = EditorGUILayout.Toggle(bValue, GUILayout.Width(fValueWidth));
				}else
				{
					GUILayout.Label(key, GUILayout.Width(fKeyWidth));
					hJsonDataGenerator.miniJsonData[key] = EditorGUILayout.TextField(hJsonDataGenerator.miniJsonData[key].ToString(), GUILayout.Width(fValueWidth));
				}
				break;
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
	}
}
