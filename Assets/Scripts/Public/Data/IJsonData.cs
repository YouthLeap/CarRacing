using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

/// <summary>
/// Json数据的读写基类
/// </summary>
public class IJsonData  {

	#region 文件操作.
	private string DataFileName;
	
	protected static Dictionary<string, object> miniJsonData = new Dictionary<string, object>();			//当前的json数据.
	protected static Dictionary<string, object> miniOriginalFileJsonData = new Dictionary<string, object>(); //原始的json数据，防止某个字段缺失的错误.
	
	public void InitData(string fileName)
	{
		string fileContent;
		DataFileName = fileName;
		
		TextAsset textAsset = Resources.Load ("Data/" + DataFileName) as TextAsset;
		
		fileContent = textAsset.text;
		
		miniJsonData = Json.Deserialize(fileContent) as Dictionary<string, object>;
		miniOriginalFileJsonData = miniJsonData;
	}
	#endregion
	
	#region 原子操作.
	
	protected object GetProperty(string keyName)
	{
		object temp;
		try
		{
			temp = miniJsonData[keyName];
		}
		catch
		{
			temp = miniOriginalFileJsonData[keyName];
			miniJsonData[keyName] = temp;
		}
		return temp;
	}
	
	protected object GetProperty(string keyName, string secondKeyName)
	{
		object temp;
		try
		{
			Dictionary<string, object> itemJson = miniJsonData[keyName] as Dictionary<string, object>;
			temp = itemJson[secondKeyName];
		}
		catch
		{
			Dictionary<string, object> itemJson = miniOriginalFileJsonData[keyName] as Dictionary<string, object>;
			if(!itemJson.ContainsKey(secondKeyName))
			{
				itemJson.Add(secondKeyName, "");
			}
			
			temp = itemJson[secondKeyName];
			Dictionary<string, object> itemJson2 = miniJsonData[keyName] as Dictionary<string, object>;
			itemJson2[secondKeyName] = temp;
		}
		return temp;
	}
	
	protected void SetProperty(string keyName, object value)
	{
		miniJsonData[keyName] = value;
	}
	
	protected void SetProperty(string keyName, string secondKeyName, object value)
	{
		Dictionary<string, object> itemJson = miniJsonData[keyName] as Dictionary<string, object>;
		itemJson[secondKeyName] = value;
	}
	#endregion

}
