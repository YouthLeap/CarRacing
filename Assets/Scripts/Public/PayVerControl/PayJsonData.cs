using UnityEngine;
using System.Collections.Generic;
using MiniJSON;
using System.Collections;

public class PayJsonData : SingletonBase<PayJsonData>
{

    public PayJsonData()
    {
        InitData(PlayerData.Instance.GetPayVersionType());
    }

    #region 文件操作.

    private static Dictionary<string, object> miniJsonData = new Dictionary<string, object>();          //Current json data.
    private static Dictionary<string, object> miniOriginalFileJsonData = new Dictionary<string, object>(); //The original json data prevents a field from missing errors.
    private string sFilename, sFileContent;

    private void InitData(string fileName)
    {
        string fileContent;
        //if (FileTool.IsFileExists(fileName) && 1 == 0) // xQueen fix for edit text
        if (FileTool.IsFileExists(fileName))
        {
            ///*If there is this data file to directly read the player information*/
            fileContent = FileTool.ReadFile(fileName);
            miniJsonData = Json.Deserialize(fileContent) as Dictionary<string, object>;
            if (miniJsonData == null)
            {
                miniJsonData = Json.Deserialize(DesCode.DecryptDES(fileContent, DesCode.PassWord)) as Dictionary<string, object>;
            }


            ///*Read the initial player file information to prevent certain fields from being there*/
            miniOriginalFileJsonData = Json.Deserialize(((TextAsset)Resources.Load("Data/PayVersionData/" + fileName)).text) as Dictionary<string, object>;
        }
        else
        {
            ///*If there is no player information file (first play), use the initial player information file to create the player information file*/
            TextAsset textAsset = Resources.Load("Data/PayVersionData/" + fileName) as TextAsset;

            fileContent = textAsset.text;
            FileTool.createORwriteFile(fileName, DesCode.EncryptDES(fileContent, DesCode.PassWord));

            miniJsonData = Json.Deserialize(fileContent) as Dictionary<string, object>;
            miniOriginalFileJsonData = miniJsonData;
        }

        sFilename = fileName;
    }


    public void SaveData()
    {
        FileTool.createORwriteFile(sFilename, DesCode.EncryptDES(Json.Serialize(miniJsonData), DesCode.PassWord));
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
            if(keyName == "CharactersGift" && secondKeyName == "Active")
            {
                int x = 1;
            }
            Dictionary<string, object> itemJson = miniJsonData[keyName] as Dictionary<string, object>;
            temp = itemJson[secondKeyName];
        }
        catch
        {
            Debug.LogError("===> keyName:" + keyName + "|secondKeyName:" + secondKeyName);
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

    private void SetProperty(string keyName, object value)
    {
        miniJsonData[keyName] = value;
    }

    private void SetProperty(string keyName, string secondKeyName, object value)
    {
        Dictionary<string, object> itemJson = miniJsonData[keyName] as Dictionary<string, object>;
        itemJson[secondKeyName] = value;
    }

    private object GetUpdateProperty(string keyName)
    {
        object temp;
        try
        {
            temp = updateJsonData[keyName];
        }
        catch
        {
            temp = null;
        }
        return temp;
    }

    private object GetUpdateProperty(string keyName, string secondKeyName)
    {
        object temp;
        try
        {
            Dictionary<string, object> itemJson = updateJsonData[keyName] as Dictionary<string, object>;
            temp = itemJson[secondKeyName];
        }
        catch
        {
            temp = null;
        }
        return temp;
    }
    private object GetVersionProperty(string keyName)
    {
        object temp;
        try
        {
            temp = versionJsonData[keyName];
        }
        catch
        {
            temp = null;
        }
        return temp;
    }

    private object GetVersionProperty(string keyName, string secondKeyName)
    {
        object temp;
        try
        {
            Dictionary<string, object> itemJson = versionJsonData[keyName] as Dictionary<string, object>;
            temp = itemJson[secondKeyName];
        }
        catch
        {
            temp = null;
        }
        return temp;
    }

    private void SetVersionProperty(string keyName, object value)
    {
        versionJsonData[keyName] = value;
    }

    private void SetVersionProperty(string keyName, string secondKeyName, object value)
    {
        Dictionary<string, object> itemJson = versionJsonData[keyName] as Dictionary<string, object>;
        itemJson[secondKeyName] = value;
    }
    #endregion

    #region 数据更新
    private static Dictionary<string, object> updateJsonData = new Dictionary<string, object>();
    private static Dictionary<string, object> versionJsonData = new Dictionary<string, object>();

    public void RefreshJsonData()
    {
        InitData(PlayerData.Instance.GetPayVersionType());
    }

    private bool IsValidJsonData(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
            return false;

        try
        {
            updateJsonData = Json.Deserialize(jsonData) as Dictionary<string, object>;
            if (updateJsonData == null || updateJsonData.Count == 0)
                return false;
        }
        catch
        {
            return false;
        }

        //判断Key是否对应有效的value
        foreach (string key in updateJsonData.Keys)
        {
            Dictionary<string, object> temp = updateJsonData[key] as Dictionary<string, object>;
            if (temp != null && temp.Count > 0)
            {
                foreach (string secondKey in temp.Keys)
                {
                    if (temp[secondKey] == null)
                    {
                        Debug.Log(key + ":" + secondKey + "格式错误");
                        return false;
                    }
                }
            }
            else if (GetUpdateProperty(key) == null)
            {
                Debug.Log(key + "格式错误");
                return false;
            }
        }

        if (updateJsonData["Version"] == null)
        {
            Debug.Log("缺少版本字段");
            return false;
        }

        return true;
    }

    public void UpdateJsonData(string jsonData)
    {
        if (!IsValidJsonData(jsonData))
            return;

        string versionName = updateJsonData["Version"].ToString();
        Debug.Log(versionName);
        if (FileTool.IsFileExists(versionName))
        {
            versionJsonData = Json.Deserialize(FileTool.ReadFile(versionName)) as Dictionary<string, object>;
        }
        else
        {
            versionJsonData = Json.Deserialize(((TextAsset)Resources.Load("Data/PayVersionData/" + versionName)).text) as Dictionary<string, object>;
        }

        foreach (string key in updateJsonData.Keys)
        {
            Dictionary<string, object> temp = updateJsonData[key] as Dictionary<string, object>;
            if (temp != null && temp.Count > 0)
            {
                foreach (string secondKey in temp.Keys)
                {
                    if (GetVersionProperty(key, secondKey) != null)
                    {
                        if (temp[secondKey].ToString().CompareTo(GetVersionProperty(key, secondKey).ToString()) != 0)
                        {
                            //Debug.Log("New Value: " + GetUpdateProperty(key, secondKey).ToString() + " Old Value: " + GetVersionProperty(key, secondKey).ToString());
                            SetVersionProperty(key, secondKey, GetUpdateProperty(key, secondKey));
                        }
                    }
                }
            }
            else if (GetVersionProperty(key) != null)
            {
                if (GetUpdateProperty(key).ToString().CompareTo(GetVersionProperty(key).ToString()) != 0)
                {
                    //Debug.Log("New Value: " + GetUpdateProperty(key).ToString() + " Old Value: " + GetVersionProperty(key).ToString());
                    SetVersionProperty(key, GetUpdateProperty(key));
                }
            }
        }

        FileTool.createORwriteFile(versionName, Json.Serialize(versionJsonData));
    }

    #endregion

    #region 数据读写
    #region 总控制
    public bool GetIsJsonCtrlBt()
    {
        return bool.Parse(GetProperty("JsonCtrlBt").ToString());
    }

    public void SetIsJsonCtrlBt(bool state)
    {
        SetProperty("JsonCtrlBt", state);
    }

    public bool GetBtNeedShowCost()
    {
        return bool.Parse(GetProperty("BtNeedShowCost").ToString());
    }

    public void SetBtNeedShowCost(bool state)
    {
        SetProperty("BtNeedShowCost", state);
    }

    public bool GetHintTextDependOnBt()
    {
        return bool.Parse(GetProperty("HintTextDependOnBt").ToString());
    }

    public void SetHintTextDependOnBt(bool state)
    {
        SetProperty("HintTextDependOnBt", state);
    }

    public int GetFreeRebornTimes()
    {
        return (int)GetProperty("FreeRebornTimes");
    }

    public void SetFreeRebornTimes(int times)
    {
        SetProperty("FreeRebornTimes", times);
    }
    #endregion

    #region 礼包开关
    public bool GetIsActivedState(string payType)
    {
        return bool.Parse(GetProperty(payType, "Active").ToString());
    }

    public void SetIsActivedState(string payType, bool state)
    {
        SetProperty(payType, "Active", state);
    }

    public bool GetIsActivedState(PayType payType)
    {
        return GetIsActivedState(payType.ToString());
    }

    public bool GetIsAsReplenishGift(string payType)
    {
        return bool.Parse(GetProperty(payType, "AsReplenishGift").ToString());
    }

    public void SetIsAsReplenishGift(string payType, bool state)
    {
        SetProperty(payType, "AsReplenishGift", state);
    }

    public bool GetIsAsReplenishGift(PayType payType)
    {
        return GetIsAsReplenishGift(payType.ToString());
    }
    #endregion

    #region 标题
    public string GetGiftTitle(string payType)
    {
        return GetProperty(payType, "Title").ToString();
    }

    public void SetGiftTitle(string payType, string title)
    {
        SetProperty(payType, "Title", title);
    }

    public string GetGiftTitle(PayType payType)
    {
        return GetGiftTitle(payType.ToString());
    }
    #endregion

    #region 自动弹出参数
    public int GetParameterA(string payType)
    {
        return int.Parse(GetProperty(payType, "ParameterA").ToString());
    }

    public void SetParameterA(string payType, string a)
    {
        SetProperty(payType, "ParameterA", a);
    }

    public int GetParameterA(PayType payType)
    {
        return GetParameterA(payType.ToString());
    }

    public bool GetLevelLimitActiveState(string payType)
    {
        return bool.Parse(GetProperty(payType, "LevelLimitActive").ToString());
    }

    public void SetLevelLimitActive(string payType, bool state)
    {
        SetProperty(payType, "LevelLimitActive", state);
    }

    public bool GetLevelLimitActiveState(PayType payType)
    {
        return GetLevelLimitActiveState(payType.ToString());
    }

    public int GetMinLevelToShow(string payType)
    {
        //		Debug.Log(payType + " " + GetProperty(payType, "MinLevelToShow"));
        return int.Parse(GetProperty(payType, "MinLevelToShow").ToString());
    }

    public void SetMinLevelToShow(string payType, string a)
    {
        SetProperty(payType, "MinLevelToShow", a);
    }

    public int GetMinLevelToShow(PayType payType)
    {
        return GetMinLevelToShow(payType.ToString());
    }

    public int GetMaxRoleLevelToShow(string payType)
    {
        return int.Parse(GetProperty(payType, "MaxRoleLevelToShow").ToString());
    }

    public void SetMaxRoleLevelToShow(string payType, string a)
    {
        SetProperty(payType, "MaxRoleLevelToShow", a);
    }

    public int GetMaxRoleLevelToShow(PayType payType)
    {
        return GetMaxRoleLevelToShow(payType.ToString());
    }

    public bool GetDateLimitActiveState(string payType)
    {
        return bool.Parse(GetProperty(payType, "DateLimitActive").ToString());
    }

    public void SetDateLimitActiveState(string payType, bool state)
    {
        SetProperty(payType, "DateLimitActive", state);
    }

    public bool GetDateLimitActiveState(PayType payType)
    {
        return GetDateLimitActiveState(payType.ToString());
    }

    public int[] GetSignInDaysToShow(string payType)
    {
        return ConvertTool.StringToAnyTypeArray<int>(GetProperty(payType, "SignInDaysToShow").ToString(), '|');
    }

    public void SetSignInDaysToShow(string payType, int[] days)
    {
        SetProperty(payType, ConvertTool.AnyTypeArrayToString<int>(days, "|"));
    }

    public int[] GetSignInDaysToShow(PayType payType)
    {
        return GetSignInDaysToShow(payType.ToString());
    }
    #endregion

    #region 关闭、取消按钮
    public string GetCloseBtSprite(string payType)
    {
        return GetProperty(payType, "CloseBtSprite").ToString();
    }

    public void SetCloseBtSprite(string payType, string sprite)
    {
        SetProperty(payType, "CloseBtSprite", sprite);
    }

    public string GetCloseBtSprite(PayType payType)
    {
        return GetCloseBtSprite(payType.ToString());
    }

    public Vector3 GetCloseBtScale(string payType)
    {
        float scale = float.Parse(GetProperty(payType, "CloseBtScale").ToString());

        return new Vector3(scale, scale, scale);
    }

    public void SetCloseBtScale(string payType, float scale)
    {
        SetProperty(payType, "CloseBtScale", scale);
    }

    public Vector3 GetCloseBtScale(PayType payType)
    {
        return GetCloseBtScale(payType.ToString());
    }

    public bool GetNeedShowCancelBt(string payType)
    {
        return bool.Parse(GetProperty(payType, "ShowCancelBt").ToString());
    }

    public void SetNeedShowCancelBt(string payType, bool state)
    {
        SetProperty(payType, "ShowCancelBt", state);
    }

    public bool GetNeedShowCancelBt(PayType payType)
    {
        return GetNeedShowCancelBt(payType.ToString());
    }

    #endregion

    #region 缩放立刻购买点击区域

    public Vector3 GetNowBuyOnClickArea(PayType payType)
    {
        return ConvertTool.StringToVector3(GetProperty(payType.ToString(), "NowBuyOnClickArea").ToString() + "*1", '*', false);
    }

    #endregion

    #region 提示文字1
    public bool GetHintText1ActiveState(string payType)
    {
        return bool.Parse(GetProperty(payType, "HintText1Active").ToString());
    }

    public void SetHintText1ActiveState(string payType, bool state)
    {
        SetProperty(payType, "HintText1Active", state);
    }

    public bool GetHintText1ActiveState(PayType payType)
    {
        return GetHintText1ActiveState(payType.ToString());
    }

    public string GetHintText1(string payType)
    {
        return GetProperty(payType, "HintText1").ToString();
    }

    public void SetHintText1(string payType, string text)
    {
        SetProperty(payType, "HintText1", text);
    }

    public string GetHintText1(PayType payType)
    {
        return GetHintText1(payType.ToString());
    }

    public int GetHint1FontSize(string payType)
    {
        return (int)GetProperty(payType, "Hint1FontSize");
    }

    public void SetHint1FontSize(string payType, float size)
    {
        SetProperty(payType, "Hint1FontSize", size);
    }

    public int GetHint1FontSize(PayType payType)
    {
        return GetHint1FontSize(payType.ToString());
    }

    public Color GetHintText1Color(string payType)
    {
        string colorStr = GetProperty(payType, "Hint1Color").ToString();

        float[] colorParameter = ConvertTool.StringToAnyTypeArray<float>(colorStr, '|');

        return new Color(colorParameter[0] / 255, colorParameter[1] / 255, colorParameter[2] / 255, colorParameter[3] / 255);
    }

    public void SetHintText1Color(string payType, Color color)
    {
        string tempStr = "";
        tempStr += (color.r * 255).ToString() + '|';
        tempStr += (color.g * 255).ToString() + '|';
        tempStr += (color.b * 255).ToString() + '|';
        tempStr += (color.a * 255).ToString();

        SetProperty(payType, "Hint1Color", tempStr);
    }

    public Color GetHintText1Color(PayType payType)
    {
        return GetHintText1Color(payType.ToString());
    }

    public Color GetHintText1ShadowColor(string payType)
    {
        string colorStr = GetProperty(payType, "Hint1ShadowColor").ToString();

        float[] colorParameter = ConvertTool.StringToAnyTypeArray<float>(colorStr, '|');

        return new Color(colorParameter[0] / 255, colorParameter[1] / 255, colorParameter[2] / 255, colorParameter[3] / 255);
    }

    public void SetHintText1ShadowColor(string payType, Color color)
    {
        string tempStr = "";
        tempStr += (color.r * 255).ToString() + '|';
        tempStr += (color.g * 255).ToString() + '|';
        tempStr += (color.b * 255).ToString() + '|';
        tempStr += (color.a * 255).ToString();

        SetProperty(payType, "Hint1ShadowColor", tempStr);
    }

    public Color GetHintText1ShadowColor(PayType payType)
    {
        return GetHintText1ShadowColor(payType.ToString());
    }


    public Color GetHintText1OutlineColor(string payType)
    {
        string colorStr = GetProperty(payType, "Hint1OutlineColor").ToString();

        float[] colorParameter = ConvertTool.StringToAnyTypeArray<float>(colorStr, '|');

        return new Color(colorParameter[0] / 255, colorParameter[1] / 255, colorParameter[2] / 255, colorParameter[3] / 255);
    }

    public void SetHintText1OutlineColor(string payType, Color color)
    {
        string tempStr = "";
        tempStr += (color.r * 255).ToString() + '|';
        tempStr += (color.g * 255).ToString() + '|';
        tempStr += (color.b * 255).ToString() + '|';
        tempStr += (color.a * 255).ToString();

        SetProperty(payType, "Hint1OutlineColor", tempStr);
    }

    public Color GetHintText1OutlineColor(PayType payType)
    {
        return GetHintText1OutlineColor(payType.ToString());
    }
    #endregion

    #region 提示文字2
    public bool GetHintText2ActiveState(string payType)
    {
        return bool.Parse(GetProperty(payType, "HintText2Active").ToString());
    }

    public void SetHintText2Active(string payType, bool state)
    {
        SetProperty(payType, "HintText2Active", state);
    }

    public bool GetHintText2ActiveState(PayType payType)
    {
        return GetHintText2ActiveState(payType.ToString());
    }

    public string GetHintText2(string payType)
    {
        return GetProperty(payType, "HintText2").ToString();
    }

    public void SetHintText2(string payType, string text)
    {
        SetProperty(payType, "HintText2", text);
    }

    public string GetHintText2(PayType payType)
    {
        return GetHintText2(payType.ToString());
    }

    public int GetHint2FontSize(string payType)
    {
        return (int)GetProperty(payType, "Hint2FontSize");
    }

    public void SetHint2FontSize(string payType, float size)
    {
        SetProperty(payType, "Hint2FontSize", size);
    }

    public int GetHint2FontSize(PayType payType)
    {
        return GetHint2FontSize(payType.ToString());
    }

    public Color GetHintText2Color(string payType)
    {
        string colorStr = GetProperty(payType, "Hint2Color").ToString();

        float[] colorParameter = ConvertTool.StringToAnyTypeArray<float>(colorStr, '|');

        return new Color(colorParameter[0] / 255, colorParameter[1] / 255, colorParameter[2] / 255, colorParameter[3] / 255);
    }

    public void SetHintText2Color(string payType, Color color)
    {
        string tempStr = "";
        tempStr += (color.r * 255).ToString() + '|';
        tempStr += (color.g * 255).ToString() + '|';
        tempStr += (color.b * 255).ToString() + '|';
        tempStr += (color.a * 255).ToString();

        SetProperty(payType, "Hint2Color", tempStr);
    }

    public Color GetHintText2Color(PayType payType)
    {
        return GetHintText2Color(payType.ToString());
    }

    public Color GetHintText2ShadowColor(string payType)
    {
        string colorStr = GetProperty(payType, "Hint2ShadowColor").ToString();

        float[] colorParameter = ConvertTool.StringToAnyTypeArray<float>(colorStr, '|');

        return new Color(colorParameter[0] / 255, colorParameter[1] / 255, colorParameter[2] / 255, colorParameter[3] / 255);
    }

    public void SetHintText2ShadowColor(string payType, Color color)
    {
        string tempStr = "";
        tempStr += (color.r * 255).ToString() + '|';
        tempStr += (color.g * 255).ToString() + '|';
        tempStr += (color.b * 255).ToString() + '|';
        tempStr += (color.a * 255).ToString();

        SetProperty(payType, "Hint2ShadowColor", tempStr);
    }

    public Color GetHintText2ShadowColor(PayType payType)
    {
        return GetHintText1ShadowColor(payType.ToString());
    }


    public Color GetHintText2OutlineColor(string payType)
    {
        string colorStr = GetProperty(payType, "Hint2OutlineColor").ToString();

        float[] colorParameter = ConvertTool.StringToAnyTypeArray<float>(colorStr, '|');

        return new Color(colorParameter[0] / 255, colorParameter[1] / 255, colorParameter[2] / 255, colorParameter[3] / 255);
    }

    public void SetHintText2OutlineColor(string payType, Color color)
    {
        string tempStr = "";
        tempStr += (color.r * 255).ToString() + '|';
        tempStr += (color.g * 255).ToString() + '|';
        tempStr += (color.b * 255).ToString() + '|';
        tempStr += (color.a * 255).ToString();

        SetProperty(payType, "Hint2OutlineColor", tempStr);
    }

    public Color GetHintText2OutlineColor(PayType payType)
    {
        return GetHintText1OutlineColor(payType.ToString());
    }
    #endregion

    #region 描述文字1
    public bool GetDescText1ActiveState(string payType)
    {
        return bool.Parse(GetProperty(payType, "DescText1Active").ToString());
    }

    public void SetDescText1Active(string payType, bool state)
    {
        SetProperty(payType, "DescText1Active", state);
    }

    public bool GetDescText1ActiveState(PayType payType)
    {
        return GetDescText1ActiveState(payType.ToString());
    }

    public string GetDescText1(string payType)
    {
        return GetProperty(payType, "DesctText1").ToString();
    }

    public void SetDescText1(string payType, string text)
    {
        SetProperty(payType, "DescText1", text);
    }

    public string GetDescText1(PayType payType)
    {
        return GetDescText1(payType.ToString());
    }
    #endregion

    #region 描述文字2
    public bool GetDescText2ActiveState(string payType)
    {
        return bool.Parse(GetProperty(payType, "DescText2Active").ToString());
    }

    public void SetDescText2Active(string payType, bool state)
    {
        SetProperty(payType, "DescText2Active", state);
    }

    public bool GetDescText2ActiveState(PayType payType)
    {
        return GetDescText2ActiveState(payType.ToString());
    }

    public string GetDescText2(string payType)
    {
        return GetProperty(payType, "DesctText2").ToString();
    }

    public void SetDescText2(string payType, string text)
    {
        SetProperty(payType, "DescText2", text);
    }

    public string GetDescText2(PayType payType)
    {
        return GetDescText2(payType.ToString());
    }
    #endregion

    #region 购买按钮
    public string GetButtonText(string payType)
    {
        return GetProperty(payType, "ButtonText").ToString();
    }

    public void SetButtonText(string payType, string text)
    {
        SetProperty(payType, "ButtonText", text);
    }

    public string GetButtonText(PayType payType, float cost = 0)
    {
        return GetButtonText(payType.ToString());
    }
    #endregion

    #region 礼包内容
    public string GetContent(string payType)
    {
        return GetProperty(payType, "Content").ToString();
    }

    public void SetContent(string payType, string content)
    {
        SetProperty(payType, "Content", content);
    }

    public string GetContent(PayType payType)
    {
        return GetContent(payType.ToString());
    }

    public PlayerData.ItemType[] GetGiftItemsTypeArr(PayType payType)
    {
        string content = GetContent(payType);
        string[] arr = content.Split('|');
        PlayerData.ItemType[] typeArr = new PlayerData.ItemType[arr.Length];

        for (int i = 0; i < arr.Length; i++)
        {
            typeArr[i] = RewardData.Instance.GetItemType(int.Parse(arr[i].Split('*')[0]));
        }

        return typeArr;
    }

    public string[] GetGiftItemsIconArr(PayType payType)
    {
        string content = GetContent(payType);
        string[] arr = content.Split('|');

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = RewardData.Instance.GetIconName(int.Parse(arr[i].Split('*')[0]));
        }

        return arr;
    }

    public int[] GetGiftItemsCountsArr(PayType payType)
    {
        string content = GetContent(payType);
        string[] arr = content.Split('|');

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr[i].Split('*')[1];
        }

        return ConvertTool.StringToAnyTypeArray<int>(ConvertTool.AnyTypeArrayToString<string>(arr, "|"), '|');
    }

    public string[] GetGiftItemsNameArr(PayType payType)
    {
        string content = GetContent(payType);
        string[] arr = content.Split('|');

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = RewardData.Instance.GetName(int.Parse(arr[i].Split('*')[0]));
        }

        return arr;
    }
    #endregion

    #region 显示关卡
    public int[] GetLevelsArrayToShow(string payType)
    {
        if (!GetIsActivedState(payType))
            return null;
        string[] levelAndDaoJuModelType = ConvertTool.StringToAnyTypeArray<string>(GetProperty(payType, "LevelToShow").ToString(), '|');
        int[] level = new int[levelAndDaoJuModelType.Length];

        for (int i = 0; i < levelAndDaoJuModelType.Length; i++)
            level[i] = int.Parse(levelAndDaoJuModelType[i].Split('*')[0]);

        return level;
    }

    public void SetLevelsArrayToShow(string payType, string arr)
    {
        SetProperty(payType, "LevelToShow", arr);
    }

    public int[] GetLevelsArrayToShow(PayType payType)
    {
        return GetLevelsArrayToShow(payType.ToString());
    }
    #endregion

    #region 显示次数
    public int GetGiftShowTimes(string payType)
    {
        return int.Parse(GetProperty(payType, "ShowTimes").ToString());
    }

    public void SetGiftShowTimes(string payType, string times)
    {
        SetProperty(payType, "ShowTimes", times);
    }

    public int GetGiftShowTimes(PayType payType)
    {
        return GetGiftShowTimes(payType.ToString());
    }
    #endregion

    #region 扣费代码
    public bool GetUsePropsState(string payType)
    {
        return bool.Parse(GetProperty(payType, "UseProps").ToString());
    }

    public void SetUsePropsState(string payType, bool state)
    {
        SetProperty(payType, "UseProps", state);
    }

    public bool GetUsePropsState(PayType payType)
    {
        return GetUsePropsState(payType.ToString());
    }

    public string GetPropsCost(string payType)
    {
        return GetProperty(payType, "PropsCost").ToString();
    }

    public void SetPropsCost(string payType, string cost)
    {
        SetProperty(payType, "PropsCost", cost);
    }

    public string GetPropsCost(PayType payType)
    {
        return GetPropsCost(payType.ToString());
    }

    public PlayerData.ItemType GetPropsCostType(string payType)
    {
        return RewardData.Instance.GetItemType(int.Parse(GetPropsCost(payType).Split('*')[0]));
    }

    public PlayerData.ItemType GetPropsCostType(PayType payType)
    {
        return GetPropsCostType(payType.ToString());
    }

    public int GetPropsCostCount(string payType)
    {
        string count = GetPropsCost(payType).Split('*')[1];
        return int.Parse(count);
    }

    public int GetPropsCostCount(PayType payType)
    {
        return GetPropsCostCount(payType.ToString());
    }

    public string GetPropsCostIcon(string payType)
    {
        return RewardData.Instance.GetIconName(int.Parse(GetPropsCost(payType).Split('*')[0]));
    }

    public string GetPropsCostIcon(PayType payType)
    {
        return GetPropsCostIcon(payType.ToString());
    }
    #endregion

    #region 礼包ID
    public int GetGiftID(string payType)
    {
        return int.Parse(GetProperty(payType, "GiftID").ToString());
    }

    public void SetGiftID(string payType, int id)
    {
        SetProperty(payType, "GiftID", id);
    }

    public int GetGiftID(PayType payType)
    {
        return GetGiftID(payType.ToString());
    }
    #endregion
    #endregion
}
