using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PayBuild;
using MiniJSON;

/// <summary>
/// 转盘抽奖控制类，by有辉
/// </summary>
public class TurnplateControllor : UIBoxBase
{

    public static TurnplateControllor Instance;

    public GameObject goCloseBtn, goStartTurnplateBt, goHuaFeiCheckBt, goHistoricCheckBt;

    #region 变量
    public Transform tranTurnplateCircle, tranFlashPointContainer;
    public EasyFontTextMesh CoinLessHint;
    public EasyFontTextMesh CurCoinText;

    public EasyFontTextMesh TitleText;
    public GameObject DescText, ShenHeDescText;
    public tk2dSprite TurnplateCircleSprite;
    public GameObject LotteryWinnerDisplay, BottomPart;

    private float baseAngle = 3600f;
    private float rotetaTime = 7f;
    private int[] AwardIdRandom = new int[12];
    private int[] AwardIdRandomSum = new int[12];
    private int AwardId;
    private float ShowFlashPointTime = 0.6f;
    private bool flashSpeedUp = false;
    private float perAwardAngle = 45f;
    private int turnPrice = 20000;
    private int coinCount;

    private bool bIsRolling;
    #endregion

    void Disable()
    {
        StopCoroutine("ShowFlashPointIE");
    }

    public void SetTurnTimes()
    {
        int curCoin = PlayerData.Instance.GetItemNum(PlayerData.ItemType.Score);
        MainInterfaceControllor.Instance.SetTurnplateCount(curCoin / turnPrice);
    }

    #region Rewrite the parent class method
    public override void Init()
    {
        Instance = this;

        InitFlashPoint();
        bIsRolling = false;

        InitTimeRecover();
        int count = isCanTurnplate ? 1 : 0;
        MainInterfaceControllor.Instance.SetTurnplateCount(count);

        base.Init();

    }

    public override void Show()
    {
        base.Show();
        //PlayerData.Instance.CoinChangeEvent += ChangeCoin;

        InitTimeRecover();
        int count = isCanTurnplate ? 1 : 0;
        MainInterfaceControllor.Instance.SetTurnplateCount(count);

        ShowFlashPoint();

        CoinLessHint.gameObject.SetActive(false);

        if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe
            || PlatformSetting.Instance.PayVersionType == PayVersionItemType.ChangWan
            || PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian)
        {
            TitleText.text = "Lucky turntable";
            DescText.SetActive(false);
            ShenHeDescText.SetActive(true);
            TurnplateCircleSprite.SetSprite("turnplate_cycle_shenhe");
            LotteryWinnerDisplay.SetActive(false);
            BottomPart.SetActive(false);
        }
        else
        {
            TitleText.text = "Lucky turntable";
            DescText.SetActive(true);
            ShenHeDescText.SetActive(false);
            TurnplateCircleSprite.SetSprite("turnplate_cycle");
            LotteryWinnerDisplay.SetActive(true);
            BottomPart.SetActive(true);

            /* xQueen disable
            //Send mb information
            phoneInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), activityTypeData);
            PostData(OnlineActivityData.UrlType.PostPhoneInfoUrl, phoneInfoJson);
            //Whether the data is released and the amount of history
            PostData(OnlineActivityData.UrlType.GetDataStateInfoUrl, phoneInfoJson);
            */
        }
    }

    public override void Hide()
    {
        base.Hide();
        //PlayerData.Instance.CoinChangeEvent -= ChangeCoin;
        UIManager.Instance.HideModule(UISceneModuleType.Turnplate);
    }

    public override void Back()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
        Hide();
    }

    void OnApplicationFocus(bool focusStatus)
    {
        InitTimeRecover();
        int count = isCanTurnplate ? 1 : 0;
        MainInterfaceControllor.Instance.SetTurnplateCount(count);
    }
    
	#endregion

	#region data processing
	void InitProbabilityData()
    {
        //Initialize the random probability value
        int[] probability = TurnplateJsonData.Instance.GetProbability();

        //		Debug.Log("Get probability successful");

        if (iUserState == -1 && CheckProbabilityEnbaled(iArrServerProbability))
        {
            for (int i = 0; i < iArrServerProbability.Length; i++)
            {
                //There are lucky draws using the server to return the probability
                AwardIdRandom[i] = iArrServerProbability[i];
            }
        }
        else
        {
            for (int i = 0; i < probability.Length; i++)
            {
                AwardIdRandom[i] = probability[i];
            }
        }

        for (int i = 0; i < AwardIdRandom.Length; i++)
        {
            AwardIdRandomSum[i] = 0;

            for (int k = 0; k <= i; k++)
            {
                AwardIdRandomSum[i] += AwardIdRandom[k];
            }
        }

        CreateAwardId();
    }

    bool CheckProbabilityEnbaled(int[] probability)
    {
        if (probability == null)
            return false;
        for (int i = 0; i < probability.Length; i++)
        {
            if (probability[i] != 0)
                return true;
        }

        Debug.Log("Invalid background return probability");
        return false;
    }

    int temp_id;
    void CreateAwardId()
    {
        //http://docs.unity3d.com/ScriptReference/Random.Range.html
        //The returned value will never be max unless min equals max.
        temp_id = Random.Range(1, AwardIdRandomSum[AwardIdRandom.Length - 1] + 1);

        for (int i = 0; i < AwardIdRandom.Length; i++)
        {
            if (temp_id <= AwardIdRandomSum[i])
            {
                AwardId = i;
                break;
            }
        }
        bCompleteGetProbability = true;
        Debug.Log("AwardId (0 - 7) : " + AwardId + "  " + "temp_id : " + temp_id + "  " + AwardIdRandomSum[AwardId]);
    }
    #endregion

    #region The turntable uses the countdown
    private int intervalSecond;
    private int TotalTime;
    private int leftUseTime;
    private bool isCanTurnplate = false;
    private int leftSecond;


    public GameObject hitButtonGO;


    private void SetCanTurnplate(bool isCanTurnplate)
    {
        this.isCanTurnplate = isCanTurnplate;

    }

    /// <summary>
    /// Inits the time recover.
    /// </summary>
    private void InitTimeRecover()
    {
        // date refresh
        System.DateTime nowTime = System.DateTime.Now;
        string lastDateStr = PlayerData.Instance.GetLastTurnplateDate();
        int deltaDay = 0;
        if (!string.IsNullOrEmpty(lastDateStr))
        {
            System.DateTime lastDate = System.DateTime.Parse(lastDateStr);
            deltaDay = nowTime.DayOfYear - lastDate.DayOfYear;
        }
        else
        {
            deltaDay = 1;
        }
        /*不是同一天就刷新*/
        if (deltaDay > 0)
        {
            PlayerData.Instance.SetUseTurnplateTime(0);
            PlayerData.Instance.SetLastTurnplateRecoverLT(System.DateTime.Now.Ticks);
            PlayerData.Instance.SetLastTurnplateDate();
        }


        intervalSecond = TurnplateJsonData.Instance.GetIntervalTime();
        TotalTime = TurnplateJsonData.Instance.GetTotalTime();
        leftUseTime = PlayerData.Instance.GetUseTurnplateTime();

        //time refresh
        if (leftUseTime >= TotalTime)
        {
            //have been used
            SetCanTurnplate(false);
            CurCoinText.text = "No turn today";
            return;
        }

        if (isCanTurnplate)
        {
            return;
        }

        long lastTicks = PlayerData.Instance.GetLastTurnplateRecoverLT();
        //Debug.Log ("Init tick "+PlayerData.Instance.GetLastSmashRecoverLT().ToString());
        long nowTicks = System.DateTime.Now.Ticks;
        long nowSecond = SystemTicksToSecond(nowTicks);
        long lastSecond = SystemTicksToSecond(lastTicks);
        int deltaSecond = (int)(nowSecond - lastSecond);

        if (deltaSecond >= intervalSecond)
        {
            //can smash egg
            SetCanTurnplate(true);
            CurCoinText.text = "Starting !";
            MainInterfaceControllor.Instance.SetTurnplateCount(1);
            CancelInvoke("UpdateTimeRecorver");
        }
        else
        {
            //calculate left second
            leftSecond = intervalSecond - deltaSecond;
            //Debug.Log("LeftSecond "+leftSecond);
            MainInterfaceControllor.Instance.SetTurnplateCount(0);
            //updateTimeRecorver
            if (IsInvoking("UpdateTimeRecorver") == false)
            {
                InvokeRepeating("UpdateTimeRecorver", 0, 1);
            }
        }


    }

    private void UpdateTimeRecorver()
    {
        string str = SecondsToTimeStr(leftSecond);
        CurCoinText.text = str;

        if (leftSecond <= 0)
        {
            CancelInvoke("UpdateTimeRecorver");
            SetCanTurnplate(true);
            CurCoinText.text = "Starting !";
            MainInterfaceControllor.Instance.SetTurnplateCount(1);
        }
        --leftSecond;
    }

    string SecondsToTimeStr(int sec)
    {
        string sSecond;
        string sMinute;
        int iSecond;
        int iMinute;

        iSecond = sec % 60;
        if (iSecond < 10)
            sSecond = "0" + iSecond;
        else
            sSecond = iSecond + "";

        iMinute = sec / 60;
        sMinute = iMinute + "";

        return sMinute + ":" + sSecond;
    }

    long SystemTicksToSecond(long tick)
    {
        return (tick / (10000000));
    }

    long SystemSecondToTicks(long second)
    {
        return second * (10000000);
    }

    #endregion


    #region 星点闪烁
    bool flashDoubleNum = true;
    int flashPointCount = 30;
    float flashPointShowAreaHight = 158;
    float circleRadius = 182;
    Vector3 centerPos = new Vector3(0, 5, 0);
    Object flashPointObj;
    Transform flashPointTempTran;
    void ShowFlashPoint()
    {
        StartCoroutine("ShowFlashPointIE");
    }

    void InitFlashPoint()
    {
        if (flashPointObj == null)
            flashPointObj = Resources.Load("UIScene/FlashPoint");

        for (int i = 0; i < flashPointCount; i++)
        {
            string spriteName;
            flashPointTempTran = ((GameObject)Instantiate(flashPointObj)).transform;
            flashPointTempTran.name = "FlashPoint" + i;
            flashPointTempTran.parent = tranFlashPointContainer;
            spriteName = (i % 2 == 0) ? "turnplate_light_pink" : "turnplate_light_yellow";
            flashPointTempTran.GetComponent<tk2dSprite>().SetSprite(spriteName);

            flashPointTempTran.localPosition = CalculateAngleToPoint(centerPos, circleRadius, i * (2 * Mathf.PI / flashPointCount));
        }
    }

    IEnumerator ShowFlashPointIE()
    {
        Vector3 point0Pos = tranFlashPointContainer.GetChild(0).localPosition;
        //		tranFlashPointContainer.GetChild(0).gameObject.SetActive(Mathf.Abs(tranFlashPointContainer.GetChild(0).localPosition.y) < flashPointShowAreaHight);
        for (int i = 0; i < flashPointCount - 1; i++)
        {
            tranFlashPointContainer.GetChild(i).localPosition = tranFlashPointContainer.GetChild(i + 1).localPosition;
            //			tranFlashPointContainer.GetChild(i).gameObject.SetActive(Mathf.Abs(tranFlashPointContainer.GetChild(i).localPosition.y) < flashPointShowAreaHight);
        }
        tranFlashPointContainer.GetChild(flashPointCount - 1).localPosition = point0Pos;
        //		tranFlashPointContainer.GetChild(flashPointCount - 1).gameObject.SetActive(Mathf.Abs(tranFlashPointContainer.GetChild(flashPointCount - 1).localPosition.y) < flashPointShowAreaHight);

        if (bIsRolling)
        {
            if (ShowFlashPointTime > 0.09f && flashSpeedUp)
            {
                ShowFlashPointTime -= 0.065f;
            }
            else if (ShowFlashPointTime < 0.6f && !flashSpeedUp)
            {
                ShowFlashPointTime += 0.035f;
            }
        }
        else
        {
            ShowFlashPointTime = 0.6f;
        }

        yield return new WaitForSeconds(ShowFlashPointTime);
        flashDoubleNum = !flashDoubleNum;

        ShowFlashPoint();
    }


    Vector3 CalculateAngleToPoint(Vector3 center, float radius, float theta)
    {
        Vector3 result = new Vector3(center.x + radius * Mathf.Cos(theta), center.y + radius * Mathf.Sin(theta), 0);
        return result;
    }
    #endregion

    #region 转动转盘
    [HideInInspector]
    public float fRotateSpeed;
    private float fWaitTime = 3f;
    private float fMinRotateSpeed = 9;
    private float fMaxRotateSpeed = 765;
    private bool bCompleteRotate = false;
    private bool bSlowingDown = false;
    private float SlowDownTime = 1.81f;

    public void TurnplateManage()
    {
        SpeedUp();
        StartCoroutine(RotateIE());
    }

    private void SpeedUp()
    {
        fRotateSpeed = fMinRotateSpeed;
        bSlowingDown = false;
        flashSpeedUp = true;
        DOTween.To(() => fRotateSpeed, x => fRotateSpeed = x, fMaxRotateSpeed, 3f).SetEase(Ease.InQuad);
    }
    private void SlowDown()
    {
        bSlowingDown = true;
        flashSpeedUp = false;
        DOTween.To(() => fRotateSpeed, x => fRotateSpeed = x, fMinRotateSpeed, SlowDownTime).SetEase(Ease.Linear);
    }

    IEnumerator RotateIE()
    {
        float fStartTime = Time.time;
        while (true)
        {
            tranTurnplateCircle.Rotate(Time.deltaTime * fRotateSpeed * Vector3.forward);
            yield return 0;

            //获得了概率且转盘不处于减速且转动时间大于了fWaitTime秒且转到了相应的奖品位置，减速
            if (bCompleteGetProbability && (!bSlowingDown) && (Time.time - fStartTime > fWaitTime) && (Mathf.Abs(tranTurnplateCircle.eulerAngles.z - AwardId * perAwardAngle) < 5))
            {
                SlowDown();
            }

            //转速小于12且转盘转到目标角度小于5且是减速状态，停止转动
            if (fRotateSpeed < 12 && bSlowingDown && (Mathf.Abs(tranTurnplateCircle.eulerAngles.z - AwardId * perAwardAngle) < 3))
            {
                DOTween.To(() => tranTurnplateCircle.eulerAngles, x => tranTurnplateCircle.eulerAngles = x, new Vector3(0, 0, AwardId * perAwardAngle), fRotateSpeed).SetSpeedBased(true);
                yield return new WaitForSeconds(0.5f);
                DealWithAwardData(AwardId);
                bIsRolling = false;
                break;
            }
        }
    }

    void StartTurnplate()
    {
        if (bIsRolling)
        {
            Debug.Log("Rolling....");
            return;
        }



        if (isCanTurnplate == false)
        {
            Debug.Log("coinCount: " + coinCount);
            StopCoroutine("SocreLessHintIE");
            StartCoroutine("SocreLessHintIE");
            return;
        }
        MainInterfaceControllor.Instance.SetTurnplateCount(0);
        SetCanTurnplate(false);
        PlayerData.Instance.SetUseTurnplateTime(++leftUseTime);
        PlayerData.Instance.SetLastTurnplateRecoverLT(System.DateTime.Now.Ticks);
        InitTimeRecover();

        bCompleteGetProbability = false;

        if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe)
        {
            iUserState = 0;
            InitProbabilityData();
        }
        else
        {
            //验证玩家抽奖状态
            phoneInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), activityTypeData);
            PostData(OnlineActivityData.UrlType.GetUserStateUrl, phoneInfoJson, InitProbabilityData);
        }

        PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Score, turnPrice);
        bIsRolling = true;
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);


        SetButtonDisable();
        TurnplateManage();
    }

    IEnumerator SocreLessHintIE()
    {
        CoinLessHint.transform.localPosition = new Vector3(10, 10, 0);
        CoinLessHint.gameObject.SetActive(true);

        DOTween.Kill(CoinLessHint.transform);
        CoinLessHint.transform.DOLocalMove(new Vector3(10, 90, 0) + ShowPosV2, 0.3f);
        yield return new WaitForSeconds(1f);
        CoinLessHint.gameObject.SetActive(false);
    }
    #endregion

    #region 奖励显示
    /// <summary>
    ///转盘选中奖励事件处理.
    /// </summary>
    /// <param name="awardId">Award identifier.</param>
    void DealWithAwardData(int awardId)
    {
        TurnplateData.Instance.AddAward(awardId);
        TurnplateAwardControllor.Instance.InitData(awardId, CompleteShowAwardCall);
        UIManager.Instance.ShowModule(UISceneModuleType.TurnplateAward);

        if (PlatformSetting.Instance.PayVersionType != PayVersionItemType.ShenHe)
        {
            //发送奖品信息
            Dictionary<string, object> miniJsonData = Json.Deserialize(awardInfoData) as Dictionary<string, object>;
            //服务端从ID1-8
            miniJsonData["AwardID"] = awardId + 1;
            awardInfoData = Json.Serialize(miniJsonData);

            awardInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), awardInfoData);
            PostData(OnlineActivityData.UrlType.PostAwardInfoUrl, awardInfoJson);
        }
    }

    void CompleteShowAwardCall()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
        SetButtonEnable();

        if (PlatformSetting.Instance.PayVersionType != PayVersionItemType.ShenHe)
        {
            if (TurnplateData.Instance.GetItemType(AwardId).CompareTo("HuaFei") == 0)
            {
                if (string.IsNullOrEmpty(PlayerData.Instance.GetPhoneNumber()))
                {
                    PhoneNumberInputControllor.CommitPhoneNumCallBack += SendAwardInfo;
                    UIManager.Instance.ShowModule(UISceneModuleType.PhoneNumberInput);
                }
                else
                {
                    SendAwardInfo();
                }
            }
        }
    }

    void SendAwardInfo()
    {
        //玩家信息
        Dictionary<string, object> miniJsonData = Json.Deserialize(userInfoData) as Dictionary<string, object>;
        //服务端从ID1-8
        miniJsonData["AwardID"] = AwardId + 1;
        miniJsonData["PhoneNum"] = PlayerData.Instance.GetPhoneNumber();
        userInfoData = Json.Serialize(miniJsonData);

        userInfoJson = CombineJsonData(PlatformSetting.Instance.GetPhoneInfoJsonData(), userInfoData);
        PostData(OnlineActivityData.UrlType.PostUserInfoUrl, userInfoJson);
    }
    #endregion



    #region 按键屏蔽处理
    public void SetButtonEnable()
    {
        goCloseBtn.GetComponent<BoxCollider>().enabled = true;
        goStartTurnplateBt.GetComponent<BoxCollider>().enabled = true;
        goHuaFeiCheckBt.GetComponent<BoxCollider>().enabled = true;
        goHistoricCheckBt.GetComponent<BoxCollider>().enabled = true;

        PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
    }

    public void SetButtonDisable()
    {
        goCloseBtn.GetComponent<BoxCollider>().enabled = false;
        goStartTurnplateBt.GetComponent<BoxCollider>().enabled = false;
        goHuaFeiCheckBt.GetComponent<BoxCollider>().enabled = false;
        goHistoricCheckBt.GetComponent<BoxCollider>().enabled = false;

        PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;
    }

    #endregion

    #region Change of gold coin data
    void ChangeCoin(int coin)
    {
        coinCount = int.Parse(CurCoinText.text);
        DOTween.Kill("TurnplateCoin");
        DOTween.To(() => this.coinCount, x => this.coinCount = x, coin, 1f).OnUpdate(ChangeCoinAnim).SetId("TurnplateCoin");
    }

    void ChangeCoinAnim()
    {
        CurCoinText.text = coinCount.ToString();
    }
    #endregion

    #region  按钮控制
    void CloseBtnOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
        Hide();
        return;
    }

    void StartTurnplateBtOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        StartTurnplate();
        return;
    }

    void HuaFeiCheckBtOnClick()
    {
        return; // xQueen disable

        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        UIManager.Instance.ShowModule(UISceneModuleType.HuaFeiDisplay);
        return;
    }

    void HistoricCheckBtOnClick()
    {
        return; // xQueen disable

        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        UIManager.Instance.ShowModule(UISceneModuleType.HistoricHuaFeiDisplay);
        return;
    }

    void AddCoinBtOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        GiftPackageControllor.Instance.Show(PayType.CoinGift);
        return;
    }
    #endregion

    #region 处理服务器内容

    //在线抽奖权限
    private bool bActivityEnabled;
    private int iUserState;
    private bool bSaveAwardSuccessful;
    private bool bSaveUserInfoSuccessful;
    //	private string sAwardPrize;
    private string fullUrl, data;
    private bool bCompleteGetProbability = false;
    private int[] iArrServerProbability;

    public delegate void WwwCallBack();
    event WwwCallBack wwwCallBack;

    string phoneInfoJson, awardInfoJson, userInfoJson;

    string activityTypeData = "{\"ActivityType\":\"ZhuanPan\"}";
    string awardInfoData = "{\"ActivityType\":\"ZhuanPan\",\"AwardID\":0}";
    string userInfoData = "{\"ActivityType\":\"ZhuanPan\",\"AwardID\":3,\"UserName\":\"\",\"PhoneNum\": \"\", \"Address\":\"\"}";


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

    void PostData(OnlineActivityData.UrlType type, string data, WwwCallBack callBack = null)
    {
        this.fullUrl = OnlineActivityData.Instance.GetUrl(type);
        this.data = data;
        wwwCallBack = callBack;

        switch (type)
        {
            case OnlineActivityData.UrlType.PostPhoneInfoUrl:
                if (!CheckInternetConnect())
                {
                    bActivityEnabled = false;
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case OnlineActivityData.UrlType.GetDataStateInfoUrl:
                if (!CheckInternetConnect())
                {
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case OnlineActivityData.UrlType.GetUserStateUrl:
                if (!bActivityEnabled)
                {
                    iUserState = 1;
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case OnlineActivityData.UrlType.PostAwardInfoUrl:
                if (!bActivityEnabled)
                {
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
            case OnlineActivityData.UrlType.PostUserInfoUrl:
                if (!bActivityEnabled)
                {
                    AnalyzeJsonData("", type);
                    return;
                }
                break;
        }

        StartCoroutine(PostData(type));
    }

    IEnumerator PostData(OnlineActivityData.UrlType type)
    {
        Dictionary<string, string> JsonDic = new Dictionary<string, string>();  // json parser header
        JsonDic.Add("Content-Type", "application/json");

        byte[] post_data;
        post_data = System.Text.UTF8Encoding.UTF8.GetBytes(data);

        Debug.Log("Type: " + type.ToString() + " PostData : " + data);

        WWW www = new WWW(fullUrl, post_data, JsonDic);
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("www error:" + www.error);

            AnalyzeJsonData("", type);
        }
        else
        {
            Debug.Log("www.text: " + www.text); // 返回内容

            AnalyzeJsonData(www.text, type);
        }
    }

    private static Dictionary<string, object> miniJsonData = new Dictionary<string, object>();
    void AnalyzeJsonData(string content, OnlineActivityData.UrlType type)
    {
        if (string.IsNullOrEmpty(content))
            content = "{\"NoJsonContent\":true}";

        //		Debug.Log("analysis content: " + content);
        miniJsonData = Json.Deserialize(content) as Dictionary<string, object>;

        switch (type)
        {
            case OnlineActivityData.UrlType.PostPhoneInfoUrl:
                if (miniJsonData.ContainsKey("ActivityEnabled"))
                    bActivityEnabled = bool.Parse(miniJsonData["ActivityEnabled"].ToString());
                else
                    bActivityEnabled = false;
                break;

            case OnlineActivityData.UrlType.GetDataStateInfoUrl:
                if (miniJsonData.ContainsKey("clean"))
                {
                    //clean=0 表示还没处理，不要清空；1则表示已经处理了，就清空
                    if (int.Parse(miniJsonData["clean"].ToString()) == 1)
                    {
                        PlayerData.Instance.SetHuaFeiAmount(0);
                        if (miniJsonData.ContainsKey("sumPrice"))
                        {
                            PlayerData.Instance.SetHistoricHuaFeiAmount(float.Parse(miniJsonData["sumPrice"].ToString()));
                        }
                    }
                }
                break;

            case OnlineActivityData.UrlType.GetUserStateUrl:
                if (miniJsonData.ContainsKey("UserEnabled"))
                    iUserState = int.Parse(miniJsonData["UserEnabled"].ToString());
                else
                    iUserState = 1;

                if (iUserState == -1 && miniJsonData.ContainsKey("Probability"))
                {
                    List<object> tempProbability = (List<object>)miniJsonData["Probability"];
                    iArrServerProbability = new int[tempProbability.Count];
                    for (int i = 0; i < iArrServerProbability.Length; i++)
                    {
                        iArrServerProbability[i] = int.Parse(tempProbability[i].ToString());
                        //					Debug.Log(i + " : " + iArrServerProbability[i]);
                    }
                }
                Debug.Log("iUserState: " + iUserState);
                break;

            case OnlineActivityData.UrlType.PostAwardInfoUrl:
                if (miniJsonData.ContainsKey("SaveSuccessful"))
                    bSaveAwardSuccessful = bool.Parse(miniJsonData["SaveSuccessful"].ToString());
                else
                    bSaveAwardSuccessful = false;
                break;

            case OnlineActivityData.UrlType.PostUserInfoUrl:
                if (miniJsonData.ContainsKey("Successful"))
                {
                    bSaveUserInfoSuccessful = bool.Parse(miniJsonData["Successful"].ToString());
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
}
