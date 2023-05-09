using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 控制显示隐藏黑色半透明框，by广顺
/// </summary>
public class TranslucentUIMaskManager : MonoBehaviour
{

    enum MaskType
    {
        BlackBG,
        ImageBG,
        Both
    }

    [HideInInspector]
    public static TranslucentUIMaskManager Instance;

    public tk2dSprite blackBg;
    public tk2dSprite imgBG;

    private Renderer blackRenderer, imgRenderer;

    private MaskType maskType = MaskType.ImageBG;
    private int layer;
    private int sortingOrder;
    private Color color;
    private Color hideColor = new Color(1f, 1f, 1f, 0), showColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Vector3 leftScale = Vector3.one, rightScale;
    //分辨率
    private float diffScreenScale = 1;

    public void Init()
    {
        Instance = this;
        gameObject.SetActive(true);
        imgBG.gameObject.SetActive(false);
        blackBg.gameObject.SetActive(false);

        imgRenderer = imgBG.GetComponent<Renderer>();
        blackRenderer = blackBg.GetComponent<Renderer>();

        transform.localPosition = Vector3.zero;

        //分辨率计算
        float oriPercent = 800.0f / 480.0f;
        float curPercent = Screen.width * 1.0f / Screen.height;
        diffScreenScale = (oriPercent > curPercent ? (oriPercent / curPercent) : (curPercent / oriPercent));
    }

    /*
	event						layer				  sortingOrder
	
	EventLayer.UI     			5						0-20
	EventLayer.FirstDialog 		11					    20-40
	EventLayer.SecondDialog		12						40-60
	EventLayer.ThirdDialog		13						60-80
	EventLayer.FourthDialog 	14						80-100
	*/
    public void Show(UISceneModuleType dialogType, float showTime = 0.3f)
    {
        //遮罩类型设置
        switch (dialogType)
        {
            case UISceneModuleType.MainInterface:
            case UISceneModuleType.LevelSelect:
            case UISceneModuleType.GamePlayerUI:
            case UISceneModuleType.GameSkill:
            case UISceneModuleType.GameResume:
            case UISceneModuleType.PropertyDisplay:
            case UISceneModuleType.Setting:
            case UISceneModuleType.Complain:
            case UISceneModuleType.ExitGame:
            case UISceneModuleType.GameEndingScore:
            case UISceneModuleType.Activity:
            case UISceneModuleType.SignIn:
            case UISceneModuleType.LockTip:
            case UISceneModuleType.DailyMission:
            case UISceneModuleType.CharacterDetail:
            case UISceneModuleType.ExchangeCode:
            case UISceneModuleType.Turnplate:
            case UISceneModuleType.SmashEgg:
            case UISceneModuleType.ConvertCenter:
            case UISceneModuleType.ExchangeActivity:
            case UISceneModuleType.LuckyNumbers:
            case UISceneModuleType.GamePlayingActivity:
            case UISceneModuleType.ClearanceRedPaper:
            case UISceneModuleType.Achievement:
            case UISceneModuleType.Shop:
            case UISceneModuleType.Strength:
            case UISceneModuleType.HuaFeiDisplay:
            case UISceneModuleType.HistoricHuaFeiDisplay:
            case UISceneModuleType.PhoneNumberInput:
            case UISceneModuleType.PlayerLevel:
            case UISceneModuleType.LevelInfo:
            case UISceneModuleType.AotemanZhaohuan:
                maskType = MaskType.ImageBG;
                break;
            case UISceneModuleType.AotemanFamily:
            case UISceneModuleType.DoubleCoin:
            case UISceneModuleType.GamePause:
            case UISceneModuleType.GameRank:
            case UISceneModuleType.GameReborn:
            case UISceneModuleType.ThreeStar:
            case UISceneModuleType.FourStar:
            case UISceneModuleType.PassGift:
            case UISceneModuleType.LevelGift:
            case UISceneModuleType.ProtectShield:
                maskType = MaskType.BlackBG;
                break;
            case UISceneModuleType.UIGuide:
            case UISceneModuleType.OneKeyToFullLevel:
            case UISceneModuleType.GiftPackage:
            case UISceneModuleType.GamePassedGfit:
            case UISceneModuleType.SignInReward:
            case UISceneModuleType.ExchangeCodeReward:
            case UISceneModuleType.DailyMissionReward:
            case UISceneModuleType.TurnplateAward:
            case UISceneModuleType.AwardItemBox:
            case UISceneModuleType.ShopSecondSure:
            case UISceneModuleType.DiscountGift:
            case UISceneModuleType.RemoveAds:
            case UISceneModuleType.NewPlayerGift:
            case UISceneModuleType.MonthCardGift:
            case UISceneModuleType.MonthCardGiftReward:
            case UISceneModuleType.CommonGift:
            case UISceneModuleType.CommonGiftAward:
                maskType = MaskType.Both;
                break;
        }

        //遮罩参数
        switch (dialogType)
        {
            //EventLayer.UI
            case UISceneModuleType.MainInterface:
            case UISceneModuleType.LevelSelect:
            case UISceneModuleType.GamePlayerUI:
            case UISceneModuleType.GameSkill:
            case UISceneModuleType.GameResume:
            case UISceneModuleType.PropertyDisplay:
                //layer = 5;
                sortingOrder = 0;
                color = hideColor;
                break;
            //EventLayer.FirstDialog
            case UISceneModuleType.AotemanFamily:
            case UISceneModuleType.Setting:
            case UISceneModuleType.Complain:
            case UISceneModuleType.LevelInfo:
            case UISceneModuleType.ExitGame:
            case UISceneModuleType.GamePause:
            case UISceneModuleType.GameRank:
            case UISceneModuleType.GameReborn:
            case UISceneModuleType.GameEndingScore:
            case UISceneModuleType.Activity:
            case UISceneModuleType.SignIn:
            case UISceneModuleType.DoubleCoin:
            case UISceneModuleType.LockTip:
            case UISceneModuleType.DailyMission:
            case UISceneModuleType.ThreeStar:
            case UISceneModuleType.FourStar:
            case UISceneModuleType.PassGift:

            case UISceneModuleType.LevelGift:
            case UISceneModuleType.ProtectShield:
                layer = 11;
                sortingOrder = 20;
                color = showColor;
                break;
            //EventLayer.SecondDialog
            case UISceneModuleType.CharacterDetail:
            case UISceneModuleType.SignInReward:
            case UISceneModuleType.ExchangeCode:
            case UISceneModuleType.ExchangeActivity:
            case UISceneModuleType.LuckyNumbers:
            case UISceneModuleType.GamePlayingActivity:
            case UISceneModuleType.ClearanceRedPaper:
            case UISceneModuleType.Turnplate:
            case UISceneModuleType.SmashEgg:
            case UISceneModuleType.ConvertCenter:
            case UISceneModuleType.Achievement:
                layer = 12;
                sortingOrder = 40;
                color = showColor;
                break;
            //EventLayer.ThirdDialog
            case UISceneModuleType.Shop:
            case UISceneModuleType.Strength:
            case UISceneModuleType.OneKeyToFullLevel:
            case UISceneModuleType.AotemanZhaohuan:
            case UISceneModuleType.ExchangeCodeReward:
            case UISceneModuleType.TurnplateAward:
            case UISceneModuleType.AwardItemBox:
            case UISceneModuleType.HuaFeiDisplay:
            case UISceneModuleType.HistoricHuaFeiDisplay:
            case UISceneModuleType.PhoneNumberInput:
            case UISceneModuleType.GamePassedGfit:
            case UISceneModuleType.DailyMissionReward:
            case UISceneModuleType.DiscountGift:
            case UISceneModuleType.RemoveAds:
            case UISceneModuleType.NewPlayerGift:
            case UISceneModuleType.MonthCardGift:
            case UISceneModuleType.MonthCardGiftReward:
            case UISceneModuleType.CommonGift:
            case UISceneModuleType.CommonGiftAward:
                layer = 13;
                sortingOrder = 60;
                color = showColor;
                break;
            //EventLayer.FourthDialog
            case UISceneModuleType.PlayerLevel:
            case UISceneModuleType.ShopSecondSure:
            case UISceneModuleType.GiftPackage:
            case UISceneModuleType.UIGuide:
                layer = 14;
                sortingOrder = 80;
                color = showColor;
                break;
        }

        switch (maskType)
        {
            case MaskType.ImageBG:
                imgBG.gameObject.SetActive(true);
                blackBg.gameObject.SetActive(false);
                imgRenderer.sortingOrder = sortingOrder;
                DOTween.Kill("ImageBgTween");
                DOTween.To(() => imgBG.color, x => imgBG.color = x, color, showTime).SetId("ImageBgTween");
                imgBG.gameObject.layer = layer;
                break;
            case MaskType.BlackBG:
                imgBG.gameObject.SetActive(false);
                blackBg.gameObject.SetActive(true);
                blackRenderer.sortingOrder = sortingOrder;
                DOTween.Kill("BlackBgTween");
                DOTween.To(() => blackBg.color, x => blackBg.color = x, color, showTime).SetId("BlackBgTween");
                blackBg.gameObject.layer = layer;
                break;
            case MaskType.Both:
                imgBG.gameObject.SetActive(true);
                blackBg.gameObject.SetActive(true);
                blackRenderer.sortingOrder = sortingOrder;
                DOTween.Kill("BlackBgTween");
                DOTween.To(() => blackBg.color, x => blackBg.color = x, color, showTime).SetId("BlackBgTween");
                blackBg.gameObject.layer = layer;
                break;
        }
        //分辨率适配缩放
        imgBG.transform.localScale = new Vector3(diffScreenScale, diffScreenScale);
        //		#if UNITY_EDITOR_OSX || UNITY_EDITOR
        //		imgBG.transform.localScale = Vector3.one * 1.2f;
        //		#endif
        leftScale = imgBG.transform.localScale;
        rightScale = new Vector3(-leftScale.x, leftScale.y, leftScale.z);
        if (dialogType == UISceneModuleType.CharacterDetail || dialogType == UISceneModuleType.LevelInfo)
        {
            imgBG.transform.localScale = rightScale;
        }
        else
        {
            imgBG.transform.localScale = leftScale;
        }
    }

    public void SetLayer(int layer)
    {
        blackBg.gameObject.layer = layer;
    }
}