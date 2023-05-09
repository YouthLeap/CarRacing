using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 游戏场景中的UI
/// </summary>
public class GamePlayerUIControllor : UIBoxBase
{

    public static GamePlayerUIControllor Instance;

    public tk2dTextMesh speedText, rankText, textTime;
    public EasyFontTextMesh textCoinCount, timeDownText, opponentDisText, rankDescText;
    public tk2dClippedSprite pathProgressFront;
    public GameObject imageCarProgerss, pathLenGO, opponentTipsGO, useShieldBtnGO, useSpeedupBtnGO, useFlyBombBtnGO;
    public GameObject inkSprite;
    public tk2dSprite propIconSprite, opponentIcon;
    public GameObject propEffectGO, lockGO;
    public UIItemData SpeedUpItemData, FlyBombItemData, ShieldItemData;
    public GameObject leftBtnGO, rightBtnGO, giftBagGO, pauseBtnGO, pickPropBtnGO;

    public List<Transform> carPointList = new List<Transform>();


    bool isShowTime = false;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        iCoinCount = 0;

        textCoinCount.text = iCoinCount.ToString();
        timeDownText.gameObject.SetActive(false);

        GameData.Instance.CoinChangeEvnet += CoinCountChange;
        PlayerData.Instance.SpeedUpChangeEvent += SpeedUpCountChange;
        PlayerData.Instance.FlyBombChangeEvent += FlyBombCountChange;
        PlayerData.Instance.ShieldChangeEvent += ShieldCountChange;
    }

    #region 重写父类方法
    public override void Init()
    {
        Instance = this;
	    transform.localPosition = ShowPosV2;//Vector3.zero;
        SetPropIcon("");
        isPropLock = false;

        SpeedUpItemData.Init(PlayerData.ItemType.SpeedUp);
        FlyBombItemData.Init(PlayerData.ItemType.FlyBomb);
        ShieldItemData.Init(PlayerData.ItemType.ProtectShield);

        base.Init();
    }

    public override void Show()
    {
        base.Show();
        gameObject.SetActive(true);


        if (CarManager.Instance.gameLevelModel == GameLevelModel.Weedout)
        {
            pathLenGO.SetActive(false);
        }
        else if (CarManager.Instance.gameLevelModel == GameLevelModel.WuJing)
        {
            pathLenGO.SetActive(true);
            pathLenGO.transform.Find("Point").gameObject.SetActive(false);
        }
        else
        {
            pathLenGO.SetActive(true);
        }

        if (PlayerData.Instance.IsWuJinGameMode())
        {
            isShowTime = true;
            textTime.transform.localPosition = new Vector3(-330f, 165f, 0);
            textTime.transform.gameObject.SetActive(true);
        }
        else if (CarManager.Instance.gameLevelModel == GameLevelModel.Weedout)
        {
            isShowTime = true;
            textTime.transform.localPosition = new Vector3(-330f, 165f, 0);
            textTime.transform.gameObject.SetActive(true);
        }
        else
        {
            isShowTime = false;
            textTime.transform.localPosition = new Vector3(-330f, 165f, 0);
            textTime.transform.gameObject.SetActive(false);
        }

        if (PlayerData.Instance.GetCurrentChallengeLevel() == 1)
        {

            pickPropBtnGO.SetActive(false);
            leftBtnGO.SetActive(false);
            rightBtnGO.SetActive(false);
            giftBagGO.SetActive(false);
            pauseBtnGO.SetActive(false);
        }

        if (PlayerData.Instance.GetCurrentChallengeLevel() <= 2)
        {
            useShieldBtnGO.SetActive(false);
            useSpeedupBtnGO.SetActive(false);
            useFlyBombBtnGO.SetActive(false);
        }

        if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian)
        {
            giftBagGO.SetActive(false);
        }
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        GameUIManager.Instance.HideModule(UISceneModuleType.GamePlayerUI);
    }

    public override void Back()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        GameUIManager.Instance.ShowModule(UISceneModuleType.GamePause);
    }

    #endregion

    void Update()
    {
        if (isLeft)
        {
            PlayerCarControl.Instance.carMove.isTurnLeft = true;
            PlayerCarControl.Instance.carMove.isTurnRight = false;
        }
        else if (isRight)
        {
            PlayerCarControl.Instance.carMove.isTurnRight = true;
            PlayerCarControl.Instance.carMove.isTurnLeft = false;
        }
        else
        {
            PlayerCarControl.Instance.carMove.isTurnLeft = false;
            PlayerCarControl.Instance.carMove.isTurnRight = false;
        }
        speedText.text = ((int)(PlayerCarControl.Instance.carMove.speed * 2)).ToString();

        UpdateRank();
        UpdateDistanceProcess();
        UpdateUseTime();
    }

    public void SetPropIcon(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
        {
            propIconSprite.gameObject.SetActive(false);
            propEffectGO.SetActive(false);
            StopCoroutine("IEShowUserPropGuide");
            isPreShowUsePropGuide = false;
        }
        else
        {
            propIconSprite.gameObject.SetActive(true);
            propIconSprite.SetSprite(iconName);
            propEffectGO.SetActive(true);

            if (PlayerData.Instance.GetCurrentChallengeLevel() < 6 && isPreShowUsePropGuide == false)
            {
                StartCoroutine("IEShowUserPropGuide");
            }
        }
    }

    void SpeedUpCountChange(int speedUpCount)
    {
        SpeedUpItemData.SetNumberText(speedUpCount);
    }

    void FlyBombCountChange(int flyBombCount)
    {
        FlyBombItemData.SetNumberText(flyBombCount);
    }

    void ShieldCountChange(int shieldCount)
    {
        ShieldItemData.SetNumberText(shieldCount);
    }

    #region 分数、金币数
    [HideInInspector] public int iCoinCount;
    private int checkRankCount = 0;

    void CoinCountChange(int coinNum)
    {
        DOTween.Kill("CoinCountChange");
        DOTween.To(() => iCoinCount, x => iCoinCount = x, coinNum, 0.8f).OnUpdate(UpdateCoinCount).SetId("CoinCountChange");
    }
    void UpdateCoinCount()
    {
        textCoinCount.text = iCoinCount.ToString();
    }

    void UpdateRank()
    {
        ++checkRankCount;
        if (checkRankCount > 8)
        {
            checkRankCount = 0;
            int rank = CarManager.Instance.GetPlayerRank();
            rankText.text = rank.ToString();
            GameData.Instance.rank = rank;
            switch(rank)
            {
                case 1: rankDescText.text = "st"; break;
                case 2: rankDescText.text = "nd"; break;
                case 3: rankDescText.text = "rd"; break;
                default: rankDescText.text = "th"; break;
            }
        }
    }


    void UpdateDistanceProcess()
    {
        if (CarManager.Instance.gameLevelModel == GameLevelModel.Weedout)
        {
            return;
        }
        //路程进度条
        float pathPercent = PlayerCarControl.Instance.GetPathPercent();
        pathPercent = Mathf.Clamp01(pathPercent);
        float p = 0.2f + 0.8f * pathPercent;
        pathProgressFront.clipTopRight = new Vector2(p, 1);

        //小车辆位置
        Vector3 carIconPos = imageCarProgerss.transform.localPosition;
        carIconPos.x = -6 + pathPercent * (156 + 6);
        imageCarProgerss.transform.localPosition = carIconPos;

        UpdateCarPathPercent();
    }

    void UpdateCarPathPercent()
    {
        if (CarManager.Instance.gameLevelModel == GameLevelModel.WuJing)
        {
            return;
        }

        float totalLen = CarManager.Instance.totalPathLen;
        for (int i = 0; i < CarManager.Instance.carMoveList.Count; ++i)
        {
            CarMove carMove = CarManager.Instance.carMoveList[i];
            float percent = carMove.moveLen / totalLen;
            percent = Mathf.Clamp01(percent);

            Transform pointTran = carPointList[i];
            Vector3 pos = pointTran.localPosition;
            pos.x = -6f + percent * 162f;
            pointTran.localPosition = pos;
        }
        for (int k = CarManager.Instance.carMoveList.Count - 1; k < carPointList.Count; ++k)
        {
            carPointList[k].gameObject.SetActive(false);
        }
    }

    void UpdateUseTime()
    {
        if (isShowTime)
        {
            float time = CarManager.Instance.totalUseTime - CarManager.Instance.playerUseTime;
            textTime.text = SecondsToTimeStr(time);

            if (time < 10 + 2 * Time.deltaTime)
            {
                StartTimeDown();
            }
        }
    }

    string SecondsToTimeStr(float sec)
    {
        string sSecond;
        string sMinute;
        string sMinSec;
        int iSecond;
        int iMinute;

        int minSec = (int)((sec - (int)sec) * 100);
        if (minSec < 10)
            sMinSec = "0" + minSec;
        else
            sMinSec = minSec.ToString();


        int iSec = (int)sec;
        iSecond = iSec % 60;
        if (iSecond < 10)
            sSecond = "0" + iSecond;
        else
            sSecond = iSecond + "";

        iMinute = iSec / 60;
        sMinute = iMinute + "";

        return sMinute + ":" + sSecond + ":" + sMinSec;
    }
    #endregion

    #region 倒计时
    public void StartTimeDown()
    {
        isShowTime = false;
        textTime.gameObject.SetActive(false);
        StopCoroutine("IETimeDown");
        StartCoroutine("IETimeDown");
    }
    public void StopTimeDown()
    {
        isShowTime = true;
        textTime.gameObject.SetActive(true);
        timeDownText.gameObject.SetActive(false);
        StopCoroutine("IETimeDown");
    }

    IEnumerator IETimeDown()
    {
        int preT, curT;
        curT = (int)(CarManager.Instance.totalUseTime - CarManager.Instance.playerUseTime);
        preT = curT;
        timeDownText.gameObject.SetActive(true);
        timeDownText.text = curT.ToString();

        Sequence timeFirstSe = DOTween.Sequence();
        timeFirstSe.Append(timeDownText.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.1f));
        timeFirstSe.Append(timeDownText.transform.DOScale(Vector3.one, 0.4f));
        while (true)
        {
            yield return null;
            curT = (int)(CarManager.Instance.totalUseTime - CarManager.Instance.playerUseTime);

            if (curT != preT)
            {
                if (curT < 0 || curT > 10)
                {
                    StopTimeDown();
                    yield break;
                }
                timeDownText.text = curT.ToString();
                timeDownText.transform.DOKill();
                Sequence timeSe = DOTween.Sequence();
                timeSe.Append(timeDownText.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.1f));
                timeSe.Append(timeDownText.transform.DOScale(Vector3.one, 0.4f));
                AudioManger.Instance.PlaySound(AudioManger.SoundName.Fuelout);

                preT = curT;
            }
        }
    }

    #endregion

    private bool isPropLock = false;
    public void SetPropLock(bool lockFlag)
    {
        isPropLock = lockFlag;
        lockGO.SetActive(lockFlag);
    }

    private bool isPreShowUsePropGuide = false;
    private IEnumerator IEShowUserPropGuide()
    {
        //Debug.Log("IEShowUserPropGuide");
        isPreShowUsePropGuide = true;
        float cal = 0;
        while (cal < 10f)
        {
            cal += Time.deltaTime;
            yield return 0;
            while (GameData.Instance.IsPause)
            {
                yield return 0;
            }
        }

        if (CarManager.Instance.isFinish == false && isPropLock == false && GameData.Instance.IsWin == false)
        {
            GameController.Instance.PauseGame();
            UIGuideControllor.Instance.Show(UIGuideType.GamePlayerUIUseCurPropGuide);
            UIGuideControllor.Instance.ShowBubbleTipByID(15);
        }
        isPreShowUsePropGuide = false;
    }

    public void ShowOpponentTips(int carId, float xPercent, float distance)
    {
        opponentTipsGO.SetActive(true);
        opponentIcon.SetSprite(ModelData.Instance.GetPlayerIcon(carId));
        opponentDisText.text = ((int)distance).ToString() + " m";
        xPercent = Mathf.Clamp01(xPercent);
        float x = xPercent * 120 * 2 - 120f;
        Vector3 pos = opponentTipsGO.transform.localPosition;
        pos.x = x;
        opponentTipsGO.transform.localPosition = pos;
    }
    public void HideOpponentTips()
    {
        opponentTipsGO.SetActive(false);
    }

    #region  按钮控制

    private bool isLeft = false;
    private bool isRight = false;

    public void LeftDown()
    {
        isLeft = true;
        isRight = false;
        if (PlayerCarControl.Instance.carMove.xOffset >= PlayerCarControl.Instance.carMove.maxXOffset - 0.5f)
            return;
        PlayerCarControl.Instance.carMove.animManager.LeftMove();
    }
    public void LeftUp()
    {
        isLeft = false;
        PlayerCarControl.Instance.carMove.animManager.LeftMoveBack();
    }

    public void RightDown()
    {
        isRight = true;
        isLeft = false;
        if (PlayerCarControl.Instance.carMove.xOffset <= PlayerCarControl.Instance.carMove.minXOffset + 0.5f)
            return;
        PlayerCarControl.Instance.carMove.animManager.RightMove();
    }

    public void RightUp()
    {
        isRight = false;
        PlayerCarControl.Instance.carMove.animManager.RightMoveBack();
    }

    public void UsePropOnClick()
    {
        if (isPropLock)
            return;
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        PlayerCarControl.Instance.propCon.UseCurProp();
        SetPropIcon("");
    }

    private void GiftBtnOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        GameController.Instance.PauseGame();

        LevelGiftControllor.Instance.Show(PayType.InnerGameGift, UseFreeFlyBmobProp, false);
        CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Light, "State", "手动弹出", "Level", PlayerData.Instance.GetSelectedLevel().ToString());
    }

    public void UseSpeedUpProp()
    {
        if (SpeedUpItemData.coolFlag)
            return;
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        if (PlayerCarControl.Instance.propCon.isSpeedUp)
            return;
        if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.SpeedUp) <= 0)
        {
            int propCost = int.Parse(BuySkillData.Instance.GetCost((int)PlayerData.ItemType.SpeedUp));
            if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.Jewel) >= propCost)
            {
                PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Jewel, propCost);
            }
            else
            {
                GameData.Instance.IsPause = true;
                GiftPackageControllor.Instance.Show(PayType.JewelGift, UseSpeedUpProp);
                return;
            }
        }
        else
        {
            PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.SpeedUp, 1);
        }
        PlayerCarControl.Instance.propCon.UsePropByType(PropType.SpeedUp);
        SpeedUpItemData.ShowClippedEffect();
        CreatePropManager.Instance.InsertGruop();
        //自定义事件.
        CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Prop_SpeedUp, "选择模式", PlayerData.Instance.GetGameMode(), "选择关卡", PlayerData.Instance.GetSelectedLevel().ToString());
    }
    public void UseFlyBmobProp()
    {
        if (FlyBombItemData.coolFlag)
            return;
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.FlyBomb) <= 0)
        {
            int propCost = int.Parse(BuySkillData.Instance.GetCost((int)PlayerData.ItemType.FlyBomb));
            if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.Jewel) >= propCost)
            {
                PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Jewel, propCost);
            }
            else
            {
                GameData.Instance.IsPause = true;
                GiftPackageControllor.Instance.Show(PayType.JewelGift, UseFlyBmobProp);
                return;
            }
        }
        else
        {
            PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.FlyBomb, 1);
        }
        PlayerCarControl.Instance.propCon.UsePropByType(PropType.FlyBmob);
        FlyBombItemData.ShowClippedEffect();
        //自定义事件.
        CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Prop_FlyBomb, "选择模式", PlayerData.Instance.GetGameMode(), "选择关卡", PlayerData.Instance.GetSelectedLevel().ToString());
    }

    //关卡内礼包购买后触发一次的必杀方法
    void UseFreeFlyBmobProp()
    {
        PlayerCarControl.Instance.propCon.UsePropByType(PropType.FlyBmob);
        FlyBombItemData.ShowClippedEffect();
    }

    private void UseShieldProp()
    {
        if (ShieldItemData.coolFlag)
            return;
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.ProtectShield) <= 0)
        {
            if (PlatformSetting.Instance.PayVersionType != PayVersionItemType.GuangDian)
            {
                GameData.Instance.IsPause = true;
                GameUIManager.Instance.ShowModule(UISceneModuleType.ProtectShield);
                return;
            }
            if (PlayerCarControl.Instance.propCon.isShield)
                return;
            int propCost = int.Parse(BuySkillData.Instance.GetCost((int)PlayerData.ItemType.ProtectShield));
            if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.Jewel) >= propCost)
            {
                PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Jewel, propCost);
            }
            else
            {
                GameData.Instance.IsPause = true;
                GiftPackageControllor.Instance.Show(PayType.JewelGift, UseShieldProp);
                return;
            }
        }
        else
        {
            if (PlayerCarControl.Instance.propCon.isShield)
                return;
            PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.ProtectShield, 1);
        }
        PlayerCarControl.Instance.propCon.UsePropByType(PropType.Shield);
        ShieldItemData.ShowClippedEffect();
        //自定义事件.
        CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Prop_Shield, "选择模式", PlayerData.Instance.GetGameMode(), "选择关卡", PlayerData.Instance.GetSelectedLevel().ToString());
    }

    private void PuaseButtonOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        GameUIManager.Instance.ShowModule(UISceneModuleType.GamePause);
    }
    #endregion
}
