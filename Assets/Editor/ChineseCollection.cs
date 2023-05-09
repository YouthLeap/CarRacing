using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class ChineseCollection : Editor {

	//Txt文件夹目录
	private static string TxtPath = Application.dataPath + "/Resources/Data";
	//CSV文件夹目录
	private static string CSVPath = Application.dataPath + "/Resources/Data";
	//Prefabs文件夹目录
	private static string UIPrefabPath = Application.dataPath + "/Prefab";
	//Resouce文件夹目录
	private static string ResoucePrefabPath = Application.dataPath + "/Resources/UIScene";
	//脚本的文件夹目录
	private static string ScriptPath = Application.dataPath + "/Scripts";
	//导出的中文KEY路径
	private static string OutPath = Application.dataPath +"/Txt/out.txt";
	
	private static List<string>Localization = null;
	private static string staticWriteText = "";
	private static string systemFlag = "Assets/";
	[MenuItem("Tools/提取中文")]
	static void ExportChinese()
	{
#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
		systemFlag = "Assets\\";
#endif
#endif
		Localization = new List<string>();
		staticWriteText ="";
		
		//提取Prefab上的中文
		staticWriteText +="----------------Prefab----------------------\n";
		LoadDiectoryPrefab(new DirectoryInfo(UIPrefabPath));
		LoadDiectoryPrefab(new DirectoryInfo(ResoucePrefabPath));
		
		//提取CS中的中文
		staticWriteText +="----------------Script----------------------\n";
		LoadDiectoryCS(new DirectoryInfo(ScriptPath));
		
		//提取CSV上的中文
		staticWriteText +="----------------CSV----------------------\n";
		LoadDiectoryCSV(new DirectoryInfo(CSVPath));

		//提取Txt上的中文
		staticWriteText +="----------------Txt----------------------\n";
		LoadDiectoryTxt(new DirectoryInfo(TxtPath));

		//最终把提取的中文生成出来
		string textPath = OutPath;
		if (System.IO.File.Exists (textPath)) 
		{
			File.Delete (textPath);
		}
		using(StreamWriter writer = new StreamWriter(textPath, false, Encoding.UTF8))
		{
			writer.Write(staticWriteText);
		}
		AssetDatabase.Refresh();
		Debug.Log("提取完成");
	}
	//递归所有Txt
	static public void LoadDiectoryTxt(DirectoryInfo dictoryInfo)
	{
		if(!dictoryInfo.Exists)   return;
		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.txt", SearchOption.AllDirectories);
		foreach (FileInfo files in fileInfos) 
		{
			string path = files.FullName;
			string assetPath =  path.Substring(path.IndexOf(systemFlag));
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
			string text = textAsset.text;
			staticWriteText+=text+"\n";
		}
		
	}
	//递归所有CSV
	static public void LoadDiectoryCSV(DirectoryInfo dictoryInfo)
	{
		if(!dictoryInfo.Exists)   return;
		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.csv", SearchOption.AllDirectories);
		foreach (FileInfo files in fileInfos) 
		{

			if(files.Name =="ItemGroupData.csv" || files.Name =="ItemData.csv" || files.Name =="ItemUnitData.csv" || files.Name =="ItemUnitTypeData.csv")
				continue;
			string path = files.FullName;
			string assetPath =  path.Substring(path.IndexOf(systemFlag));
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
			string text = textAsset.text;
			staticWriteText+=text+"\n";
		}

	}
	//递归所有UI Prefab
	static public  void  LoadDiectoryPrefab(DirectoryInfo dictoryInfo)
	{
		if(!dictoryInfo.Exists)   return;
		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
		foreach (FileInfo files in fileInfos)
		{
			string path = files.FullName;
			string assetPath =  path.Substring(path.IndexOf(systemFlag));
			GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			GameObject instance = GameObject.Instantiate(prefab) as GameObject;
			SearchPrefabString(instance.transform);
			GameObject.DestroyImmediate(instance);
		}
	}
	//递归所有C#代码
	static public  void  LoadDiectoryCS(DirectoryInfo dictoryInfo)
	{
		
		if(!dictoryInfo.Exists)   return;
		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);
		foreach (FileInfo files in fileInfos)
		{
			string path = files.FullName;
			string assetPath =  path.Substring(path.IndexOf(systemFlag));
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
			string text = textAsset.text;
			GetChinese(text);
		}

	}
	static public void GetChinese(string str)
	{
		//用正则表达式提取双引号中的内容。
		Regex reg =  new Regex("\"[^\"]*\"");
		MatchCollection mc = reg.Matches(str);
		foreach(Match m in mc)
		{
			string format = m.Value;
			format = format.Replace("\"","");
			//用正则表达式提取双引号中中文。
			Regex reg1 =   new Regex("[\u4e00-\u9fa5]+");
			MatchCollection mc1 = reg1.Matches(format);
			foreach(Match mt in mc1)
			{
				format = mt.Value;
				if(!Localization.Contains(format) && !string.IsNullOrEmpty(format)){
					Localization.Add(format);
					staticWriteText+=format+"\n";
				}
			}
		}

		//用正则表达式提取enum中的内容。
		MatchCollection mc2 = Regex.Matches(str, @"enum[^\{]*\{([^\}]*)\}", RegexOptions.Singleline);
		foreach(Match m in mc2)
		{
			string content = m.Value;
			//提取任务中心中enum AchievementNames内容
			if(content.Contains("AchievementNames"))
			{
//				Debug.Log(content);
				//去掉注释
				string newStr = Regex.Replace(content, @"//.*$", "", RegexOptions.Multiline);					
//				Debug.Log(newStr);

				string format = newStr;
				//用正则表达式提取中文。
				Regex reg1 =   new Regex("[\u4e00-\u9fa5]+");
				MatchCollection mc1 = reg1.Matches(format);
				foreach(Match mt in mc1)
				{
					format = mt.Value;
					if(!Localization.Contains(format) && !string.IsNullOrEmpty(format)){
//						Debug.Log(format);
						Localization.Add(format);
						staticWriteText+=format+"\n";
					}
				}
			}

		}
	}
	//提取Prefab上的中文
	static public void SearchPrefabString(Transform root)
	{
		foreach(Transform chind in root)
		{
			//因为这里是写例子，所以我用的是UILabel 
			//这里应该是写你用于图文混排的脚本。
			EasyFontTextMesh label = chind.GetComponent<EasyFontTextMesh>();
			if(label != null)
			{
				string text = label.text;
				if(!Localization.Contains(text) && !string.IsNullOrEmpty(text)){
					Localization.Add(text);
					text = text.Replace("\n",@"\n");
					staticWriteText+=text+"\n";
				}
			}
			if(chind.childCount >0)
				SearchPrefabString(chind);
		}
	}
}
