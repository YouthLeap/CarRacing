using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class OnlineActivityData : SingletonBase<OnlineActivityData> {

	public OnlineActivityData()
	{
		InitData("OnlineActivity");
	}

	public enum UrlType
	{
		PostPhoneInfoUrl,
		GetDataStateInfoUrl,
		GetUserStateUrl,
		PostAwardInfoUrl,
		PostUserInfoUrl
	}

	#region 文件操作.
	
	private string DataFileName;
	
	private static Dictionary<string, object> miniJsonData = new Dictionary<string, object>();			//当前的json数据.
	private static Dictionary<string, object> miniOriginalFileJsonData = new Dictionary<string, object>(); //原始的json数据，防止某个字段缺失的错误.
	
	private void InitData(string fileName)
	{
		string fileContent;
		DataFileName = fileName;

		TextAsset textAsset = Resources.Load ("Data/" + DataFileName) as TextAsset;
			
		fileContent = textAsset.text;
		miniJsonData = Json.Deserialize(fileContent) as Dictionary<string, object>;
		miniOriginalFileJsonData = miniJsonData;
	}
	
	// <summary>
	/// 本地数据保存
	/// </summary>
	public void SaveData()
	{
		FileTool.createORwriteFile (DataFileName, Json.Serialize(miniJsonData));
	}
	#endregion

	#region 原子操作.
	
	private object GetProperty(string keyName)
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
	
	private object GetProperty(string keyName, string secondKeyName)
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
			temp = itemJson[secondKeyName];
			Dictionary<string, object> itemJson2 = miniJsonData[keyName] as Dictionary<string, object>;
			itemJson2[secondKeyName] = temp;
		}
		return temp;
	}
	
	private void SetProperty(string keyName, object value)
	{
		miniJsonData[keyName] = value;
	}
	
	private void SetProperty(string keyName, string secondKeyName, object value)
	{
		Dictionary<string, object> itemJson = miniJsonData[keyName] as Dictionary<string, object>;
		itemJson[secondKeyName] = value;
	}
	
	#endregion

	public string GetUrl(UrlType type)
	{
		return GetProperty(type.ToString()).ToString();
	}
}
