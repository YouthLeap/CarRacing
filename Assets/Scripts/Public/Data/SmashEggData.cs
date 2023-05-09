using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

/// <summary>
/// 砸金蛋数据
/// </summary>
public class SmashEggData : MonoSingletonBase<SmashEggData>
{


    public override void Init()
    {
        InitData("SmashEggData");
    }


    public class AwardItemStruct
    {
        public string name;
        public int rewardId;
        public int rewardCount;
        public int weight;
        public bool isTrueGift;
        public float probability;
    }
    public List<AwardItemStruct> itemList = new List<AwardItemStruct>();
    public List<float> probabiliryList = new List<float>();
    public List<float> serverProbabiliryList = new List<float>();
    public float totalWeight;




    public bool GetIsOpen()
    {
        return bool.Parse(GetProperty("isOpen").ToString());
    }

    public string GetTitle()
    {
        return GetProperty("title").ToString();
    }
    public string GetDes()
    {
        return GetProperty("Des").ToString();
    }

    public int GetIntervalTime()
    {
        string timeArrayStr = GetProperty("IntervalTime").ToString();
        int[] intervalArray = ConvertTool.StringToAnyTypeArray<int>(timeArrayStr, '|');
        int index = PlayerData.Instance.GetUseSmashTime();
        if (index >= intervalArray.Length)
        {
            index = intervalArray.Length - 1;
        }

        return intervalArray[index];
    }
    public int GetTotalTime()
    {
        return int.Parse(GetProperty("TotalTime").ToString());
    }

    public List<AwardItemStruct> GetItemList()
    {

        if (itemList.Count > 1)
        {
            return itemList;
        }

        itemList.Clear();
        totalWeight = 0;
        List<object> objectList = (List<object>)miniJsonData["AwardItem"];
        for (int i = 0; i < objectList.Count; ++i)
        {
            Dictionary<string, object> itemJson = objectList[i] as Dictionary<string, object>;
            AwardItemStruct itemStrct = new AwardItemStruct();

            itemStrct.name = itemJson["name"].ToString();
            itemStrct.rewardId = int.Parse(itemJson["rewardId"].ToString());
            itemStrct.rewardCount = int.Parse(itemJson["rewardCount"].ToString());
            itemStrct.weight = int.Parse(itemJson["weight"].ToString());
            itemStrct.isTrueGift = bool.Parse(itemJson["isTrueGift"].ToString());
            itemStrct.probability = 0;

            if (itemStrct.isTrueGift) { continue; } // xQueen remove true gift

            totalWeight += itemStrct.weight;

            itemList.Add(itemStrct);
        }

        //cal probobiliry
        for (int k = 0; k < itemList.Count; ++k)
        {
            itemList[k].probability = itemList[k].weight / totalWeight;
        }
        return itemList;
    }

    public List<float> GetProbabilityList()
    {
        /*Determine whether to use the server's probability data*/
        if (this.bActivityEnabled && this.iUserState == -1 && this.serverProbabiliryList.Count > 0)
        {
            Debug.Log("User Server");
            return this.serverProbabiliryList;
        }


        if (probabiliryList.Count > 1)
        {
            return probabiliryList;
        }

        if (itemList.Count < 1)
        {
            itemList = GetItemList();
        }

        probabiliryList.Clear();
        for (int i = 0; i < itemList.Count; ++i)
        {
            probabiliryList.Add(itemList[i].probability);
        }
        Debug.Log("User Local");
        return probabiliryList;
    }


    #region 外部调用

    public void RefreshServerData()
    {
        //		phoneInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), activityTypeData);
        //		PostData(UrlType.PostPhoneInfoUrl, phoneInfoJson);
        //		
        //		phoneInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), activityTypeData);
        //		PostData(UrlType.GetUserStateUrl, phoneInfoJson);
    }

    public void SendAwardInfo(int awardId)
    {
        Debug.LogError("Disable this feature:" + awardId); // xQueen disable
        return;

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
        miniJsonData["AwardID"] = awardId;
        miniJsonData["PhoneNum"] = PlayerData.Instance.GetPhoneNumber();
        userInfoData = Json.Serialize(miniJsonData);

        userInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), userInfoData);
        PostData(UrlType.PostUserInfoUrl, userInfoJson);


    }
    #endregion


    #region 处理服务器内容

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

    public delegate void WwwCallBack();
    event WwwCallBack wwwCallBack;

    private bool bActivityEnabled = false;
    private int iUserState;
    private bool bSaveAwardSuccessful;
    private bool bSaveUserInfoSuccessful;



    //	private string sAwardPrize;
    private string fullUrl, data;
    private bool bCompleteGetProbability = false;
    private int[] iArrServerProbability;

    string phoneInfoJson, awardInfoJson, userInfoJson;

    string activityTypeData = "{\"ActivityType\":\"SmashEgg\"}";
    string awardInfoData = "{\"ActivityType\":\"SmashEgg\",\"AwardID\":0}";
    string userInfoData = "{\"ActivityType\":\"SmashEgg\",\"AwardID\":3,\"UserName\":\"\",\"PhoneNum\": \"\", \"Address\":\"\"}";


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
        this.fullUrl = GetUrl(type);
        this.data = data;
        wwwCallBack = callBack;

        switch (type)
        {
            case UrlType.PostPhoneInfoUrl:
                if (!CheckInternetConnect())
                {
                    bActivityEnabled = false;
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case UrlType.GetDataStateInfoUrl:
                if (!CheckInternetConnect())
                {
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case UrlType.GetUserStateUrl:
                if (!CheckInternetConnect())
                {
                    iUserState = 1;
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case UrlType.PostAwardInfoUrl:
                if (!bActivityEnabled)
                {
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case UrlType.PostUserInfoUrl:
                if (!bActivityEnabled)
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
        Dictionary<string, string> JsonDic = new Dictionary<string, string>();  // json parser header
        JsonDic.Add("Content-Type", "application/json");

        byte[] post_data;
        post_data = System.Text.UTF8Encoding.UTF8.GetBytes(data);

        //Debug.Log("Type: " + type.ToString() + " PostData : " + data);

        WWW www = new WWW(fullUrl, post_data, JsonDic);
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("www error:" + www.error);

            AnalyzeJsonData("", type);
        }
        else
        {
            //Debug.Log("www.text: " + www.text); // 返回内容

            AnalyzeJsonData(www.text, type);
        }
    }

    private static Dictionary<string, object> resJsonData = new Dictionary<string, object>();
    void AnalyzeJsonData(string content, UrlType type)
    {
        if (string.IsNullOrEmpty(content))
            content = "{\"NoJsonContent\":true}";

        //		Debug.Log("analysis content: " + content);
        resJsonData = Json.Deserialize(content) as Dictionary<string, object>;

        switch (type)
        {
            case UrlType.PostPhoneInfoUrl:
                if (resJsonData.ContainsKey("ActivityEnabled"))
                    bActivityEnabled = bool.Parse(resJsonData["ActivityEnabled"].ToString());
                else
                    bActivityEnabled = false;

                //Debug.Log("PostPhoneInfoUrl "+bActivityEnabled.ToString());
                break;

            case UrlType.GetDataStateInfoUrl:
                if (resJsonData.ContainsKey("clean"))
                {
                    //clean=0 表示还没处理，不要清空；1则表示已经处理了，就清空
                    if (int.Parse(resJsonData["clean"].ToString()) == 1)
                    {
                        PlayerData.Instance.SetHuaFeiAmount(0);
                        if (resJsonData.ContainsKey("sumPrice"))
                        {
                            PlayerData.Instance.SetHistoricHuaFeiAmount(float.Parse(resJsonData["sumPrice"].ToString()));
                        }
                    }
                }
                break;

            case UrlType.GetUserStateUrl:
                if (resJsonData.ContainsKey("UserEnabled"))
                    iUserState = int.Parse(resJsonData["UserEnabled"].ToString());
                else
                    iUserState = 1;

                if (iUserState == -1 && resJsonData.ContainsKey("Probability"))
                {
                    List<object> tempProbability = (List<object>)resJsonData["Probability"];
                    iArrServerProbability = new int[tempProbability.Count];
                    float totalWeight = 0;
                    for (int i = 0; i < iArrServerProbability.Length; i++)
                    {
                        iArrServerProbability[i] = int.Parse(tempProbability[i].ToString());
                        totalWeight += iArrServerProbability[i];
                        //Debug.Log(i + " : " + iArrServerProbability[i]);
                    }

                    serverProbabiliryList.Clear();
                    for (int j = 0; j < iArrServerProbability.Length; ++j)
                    {
                        serverProbabiliryList.Add(iArrServerProbability[j] / totalWeight);
                    }


                }
                //Debug.Log("iUserState: " + iUserState);
                break;

            case UrlType.PostAwardInfoUrl:
                if (resJsonData.ContainsKey("SaveSuccessful"))
                    bSaveAwardSuccessful = bool.Parse(resJsonData["SaveSuccessful"].ToString());
                else
                    bSaveAwardSuccessful = false;
                break;

            case UrlType.PostUserInfoUrl:
                if (resJsonData.ContainsKey("Successful"))
                {
                    bSaveUserInfoSuccessful = bool.Parse(resJsonData["Successful"].ToString());
                }
                else
                    bSaveUserInfoSuccessful = false;

                break;
        }

        if (wwwCallBack != null)
        {
            wwwCallBack();
            wwwCallBack = null;
        }
    }

    #endregion

    #region 文件操作.
    private string DataFileName;

    protected static Dictionary<string, object> miniJsonData = new Dictionary<string, object>();            //当前的json数据.
    protected static Dictionary<string, object> miniOriginalFileJsonData = new Dictionary<string, object>(); //原始的json数据，防止某个字段缺失的错误.

    public void InitData(string fileName)
    {
        string fileContent;
        DataFileName = fileName;

        TextAsset textAsset = Resources.Load("Data/" + DataFileName) as TextAsset;

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
            if (!itemJson.ContainsKey(secondKeyName))
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
