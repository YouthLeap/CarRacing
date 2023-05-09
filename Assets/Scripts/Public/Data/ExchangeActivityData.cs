using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

/// <summary>
///活动彩蛋数据
/// </summary>
public class ExchangeActivityData : MonoBehaviour {


	private static ExchangeActivityData _instance =null;
	public static ExchangeActivityData Instance {
		get{
			if(_instance == null)
			{
				GameObject go = new GameObject();
				go.name = "(Singleton)ExchangeActivityData";
				DontDestroyOnLoad(go);
				_instance = go.AddComponent<ExchangeActivityData>();
				_instance.InitData("ExchangeActivityData");

			}
			return _instance;
		}
	}



	#region 文件操作.
	
	private string DataFileName;
	
	private static Dictionary<string, object> miniJsonData = new Dictionary<string, object>();			//当前的json数据.
	private static Dictionary<string, object> miniOriginalFileJsonData = new Dictionary<string, object>(); //原始的json数据，防止某个字段缺失的错误.
	
	private void InitData(string fileName)
	{
		string fileContent;
		DataFileName = fileName;

		if (FileTool.IsFileExists (fileName)) {
			///*如果存在此数据文件就直接读取玩家信息*/
			fileContent = FileTool.ReadFile (fileName);
			miniJsonData = Json.Deserialize (DesCode.DecryptDES(fileContent, DesCode.PassWord)) as Dictionary<string, object>;
			///*读取初始玩家文件信息，防止某些字段没有*/
			miniOriginalFileJsonData = Json.Deserialize (((TextAsset)Resources.Load ("Data/" + DataFileName )).text) as Dictionary<string, object>;
		} else {
			TextAsset textAsset = Resources.Load ("Data/" + DataFileName) as TextAsset;
			fileContent = textAsset.text;
			miniJsonData = Json.Deserialize(fileContent) as Dictionary<string, object>;
			miniOriginalFileJsonData = miniJsonData;
			SaveData();
		}
	}
	
	// <summary>
	/// 本地数据保存
	/// </summary>
	public void SaveData()
	{
		FileTool.createORwriteFile (DataFileName, DesCode.EncryptDES (Json.Serialize(miniJsonData), DesCode.PassWord));
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

    public string GetTimeString()
	{
		return GetProperty("DataTime").ToString();
	}

    public List<DataStruct> GetItemList()
	{
		List<DataStruct> itemList = new List<DataStruct> ();

		for (int i=1; i<=4; ++i) {
			DataStruct item=new DataStruct();
			item.isTrueGift = bool.Parse(GetProperty("Item"+i,"IsTrueGift").ToString());
			item.awardId = int.Parse(GetProperty("Item"+i,"AwardId").ToString());
			item.name = GetProperty("Item"+i,"Name").ToString();
			item.price = int.Parse(GetProperty("Item"+i,"Price").ToString() );
			item.rest = int.Parse( GetProperty("Item"+i,"Rest").ToString());
			item.award = GetProperty("Item"+i,"Award").ToString();
			item.isGet = bool.Parse(GetProperty("Item"+i,"IsGet").ToString());
			itemList.Add(item);
		}
		return itemList;
	}



	public class DataStruct{
		public bool isTrueGift;
		public int awardId;
		public string name;
		public int price;
		public int rest;
		public string award;
		public bool isGet;
	}

	public enum UrlType
	{
		PostPhoneInfoUrl,
		GetDataStateInfoUrl,
		GetUserStateUrl,
		PostAwardInfoUrl,
		PostUserInfoUrl
	}
	public string GetUrl(UrlType type)
	{
		return GetProperty(type.ToString()).ToString();
	}

	#region  外部调用
	
	public bool GetActivityEnable()
	{
		if (this.bActivityEnabled && iUserState == -1) {
			return true;
		}
		return false;
	}
	public float GetProbability()
	{
		if (iArrServerProbability!=null && this.iArrServerProbability.Length>1)
		{
			return iArrServerProbability[3]/100f;
		}
		return 0;
	}

	public int GetServerProbability(int index)
	{
		if (iArrServerProbability == null || index > iArrServerProbability.Length - 1) {
			return 0;
		}
		return iArrServerProbability [index];
	}

	public void SetItemData(DataStruct data)
	{
		SetProperty ("Item" + data.awardId, "IsGet", data.isGet);
	}

	public void InitFromServer()
	{
        // Debug.Log("Disable InitFromServer");
        return;

		phoneInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), activityTypeData);
		PostData(UrlType.PostPhoneInfoUrl, phoneInfoJson);

		phoneInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), activityTypeData);
		PostData(UrlType.GetUserStateUrl, phoneInfoJson);
	}

	public  void SendAwardInfo(int awardId)
	{

		//发送奖品信息
		Dictionary<string, object> awardJsonData = Json.Deserialize(awardInfoData) as Dictionary<string, object>;
		//服务端从ID1-8
		awardJsonData["AwardID"] = awardId;
		awardInfoData = Json.Serialize(awardJsonData);
		awardInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), awardInfoData);
		PostData(UrlType.PostAwardInfoUrl, awardInfoJson);

		//玩家信息
		Dictionary<string, object> miniJsonData = Json.Deserialize(userInfoData) as Dictionary<string, object>;
		//服务端从ID1-8
		miniJsonData["AwardID"] =  awardId;
		miniJsonData["PhoneNum"] = PlayerData.Instance.GetPhoneNumber();
		userInfoData = Json.Serialize(miniJsonData);
		
		userInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), userInfoData);
		PostData(UrlType.PostUserInfoUrl, userInfoJson);


	}

	#endregion

	#region 处理服务器内容
	public delegate void WwwCallBack();
	event WwwCallBack wwwCallBack;
	
	private bool bActivityEnabled=false;
	private int  iUserState;
	private bool bSaveAwardSuccessful;
	private bool bSaveUserInfoSuccessful;



	//	private string sAwardPrize;
	private string fullUrl, data;
	private bool bCompleteGetProbability = false;
	private int[] iArrServerProbability;

	string phoneInfoJson, awardInfoJson, userInfoJson;

	string activityTypeData = "{\"ActivityType\":\"shouji\"}";
	string awardInfoData    = "{\"ActivityType\":\"shouji\",\"AwardID\":0}";
	string userInfoData     = "{\"ActivityType\":\"shouji\",\"AwardID\":3,\"UserName\":\"\",\"PhoneNum\": \"\", \"Address\":\"\"}";
	
	
	string CombineJsonData(string firstJson, string secondJson)
	{
		string tempJson1 = firstJson, tempJson2 = secondJson;
		tempJson1 = tempJson1.Substring(0, tempJson1.Length - 1);
		tempJson2 = tempJson2.Substring(1, tempJson2.Length - 1);
		
		return tempJson1 + "," + tempJson2;
	}
	
	bool CheckInternetConnect()
	{
		//		return false;
		return Application.internetReachability != NetworkReachability.NotReachable;
	}
	
	void PostData(UrlType type, string data, WwwCallBack callBack = null)
	{
		this.fullUrl =GetUrl(type);
		this.data = data;
		wwwCallBack = callBack;
		
		switch(type)
		{
		case UrlType.PostPhoneInfoUrl:
			if(!CheckInternetConnect())
			{
				bActivityEnabled = false;
				AnalyzeJsonData("", type);
				return;
			}
			break;
		case UrlType.GetDataStateInfoUrl:
			if(!CheckInternetConnect())
			{
				AnalyzeJsonData("", type);
				return;
			}
			break;
		case UrlType.GetUserStateUrl:
			if(!CheckInternetConnect())
			{
				iUserState = 1;
				AnalyzeJsonData("", type);
				return;
			}
			break;
		case UrlType.PostAwardInfoUrl:
			if(!bActivityEnabled)
			{
				AnalyzeJsonData("", type);
				return;
			}
			break;
		case UrlType.PostUserInfoUrl:
			if(!bActivityEnabled)
			{
				AnalyzeJsonData("", type);
				return;
			}
			break;
		}
		StartCoroutine(PostData(type));
	}
	
	IEnumerator PostData(UrlType type)
	{
		Dictionary<string,string> JsonDic = new Dictionary<string, string> ();  // json parser header
		JsonDic.Add("Content-Type", "application/json");
		
		byte[] post_data;  
		post_data = System.Text.UTF8Encoding.UTF8.GetBytes(data);  
		
		Debug.Log("[fullUrl: " + fullUrl + "] [Type: " + type.ToString() + "] [PostData : " + data + "]");
		
		WWW www = new WWW(fullUrl,post_data,JsonDic);
		yield return www;
		
		if (www.error != null) 
		{
			Debug.LogError ("www error:" + www.error);
			
			AnalyzeJsonData("", type);
		}else
		{
			//Debug.Log("www.text: " + www.text); // 返回内容
			
			AnalyzeJsonData(www.text, type);
		}
	}
	
	private static Dictionary<string, object> resJsonData = new Dictionary<string, object>();
	void AnalyzeJsonData(string content, UrlType type)
	{
		if(string.IsNullOrEmpty(content))
			content = "{\"NoJsonContent\":true}";
		
		//		Debug.Log("analysis content: " + content);
		resJsonData = Json.Deserialize(content) as Dictionary<string, object>;
		
		switch(type)
		{
		case UrlType.PostPhoneInfoUrl:
			if(resJsonData.ContainsKey("ActivityEnabled"))
				bActivityEnabled = bool.Parse(resJsonData["ActivityEnabled"].ToString());
			else
				bActivityEnabled = false;

			//Debug.Log("PostPhoneInfoUrl "+bActivityEnabled.ToString());
			break;
			
		case UrlType.GetDataStateInfoUrl:
			if(resJsonData.ContainsKey("clean"))
			{
				//clean=0 表示还没处理，不要清空；1则表示已经处理了，就清空
				if(int.Parse(resJsonData["clean"].ToString()) == 1)
				{
					PlayerData.Instance.SetHuaFeiAmount(0);
					if(resJsonData.ContainsKey("sumPrice"))
					{
						PlayerData.Instance.SetHistoricHuaFeiAmount(float.Parse(resJsonData["sumPrice"].ToString()));
					}
				}
			}
			break;
			
		case UrlType.GetUserStateUrl:
			if(resJsonData.ContainsKey("UserEnabled"))
				iUserState = int.Parse(resJsonData["UserEnabled"].ToString());
			else
				iUserState = 1;
			
			if(iUserState == -1 && resJsonData.ContainsKey("Probability"))
			{
				List<object> tempProbability = (List<object>)resJsonData["Probability"];
				iArrServerProbability = new int[tempProbability.Count];
				for(int i = 0; i < iArrServerProbability.Length; i ++)
				{
					iArrServerProbability[i] = int.Parse(tempProbability[i].ToString());
					//Debug.Log(i + " : " + iArrServerProbability[i]);
				}
			}
			//Debug.Log("iUserState: " + iUserState);
			break;
			
		case UrlType.PostAwardInfoUrl:
			if(resJsonData.ContainsKey("SaveSuccessful"))
				bSaveAwardSuccessful = bool.Parse(resJsonData["SaveSuccessful"].ToString());
			else
				bSaveAwardSuccessful = false;
			break;
			
		case UrlType.PostUserInfoUrl:
			if(resJsonData.ContainsKey("Successful"))
			{
				bSaveUserInfoSuccessful = bool.Parse(resJsonData["Successful"].ToString());
			}
			else
				bSaveUserInfoSuccessful = false;
			
			break;
		}
		
		if(wwwCallBack != null)
		{
			wwwCallBack();
			wwwCallBack = null;
		}
	}
	 
	#endregion
}
