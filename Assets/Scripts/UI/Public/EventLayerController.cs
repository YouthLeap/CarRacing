using UnityEngine;
using System.Collections;

/// <summary>
/// 事件层管理类.
/// </summary>
[RequireComponent(typeof(tk2dUICamera))]
public class EventLayerController : MonoBehaviour
{

    public static EventLayerController Instance;

    private tk2dUICamera UICamera, DialogCamera1, DialogCamera2;

    private GameObject CameraObj;

    void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        if (UICamera == null)
        {
            UICamera = transform.GetComponent<tk2dUICamera>();
        }

        CameraObj = GameObject.Find("DialogCamera1");
        if (DialogCamera1 == null && CameraObj != null)
        {
            DialogCamera1 = CameraObj.GetComponent<tk2dUICamera>();
        }

        CameraObj = GameObject.Find("DialogCamera2");
        if (DialogCamera2 == null && CameraObj != null)
        {
            DialogCamera2 = CameraObj.GetComponent<tk2dUICamera>();
        }
    }

    public void SetEventLayer(UISceneModuleType boxType)
    {
        //print (boxType);
        switch (boxType)
        {
            case UISceneModuleType.MainInterface:
            case UISceneModuleType.LevelSelect:
            case UISceneModuleType.GamePlayerUI:
                SetEventLayer(EventLayer.UI);
                break;
            case UISceneModuleType.Setting:
            case UISceneModuleType.AotemanFamily:
            case UISceneModuleType.DayTask:
            case UISceneModuleType.Complain:
            case UISceneModuleType.LevelInfo:
            case UISceneModuleType.GameReborn:
            case UISceneModuleType.GameEndingScore:
            case UISceneModuleType.Activity:
            case UISceneModuleType.SignIn:
            case UISceneModuleType.DailyMission:
            case UISceneModuleType.ExitGame:
            case UISceneModuleType.DoubleCoin:
            case UISceneModuleType.LockTip:
            case UISceneModuleType.GamePause:
            case UISceneModuleType.GameRank:
            case UISceneModuleType.ThreeStar:
            case UISceneModuleType.FourStar:
            case UISceneModuleType.PassGift:

            case UISceneModuleType.LevelGift:
            case UISceneModuleType.ProtectShield:
                SetEventLayer(EventLayer.FirstDialog);
                break;
            case UISceneModuleType.CharacterDetail:
            case UISceneModuleType.SignInReward:
            case UISceneModuleType.ExchangeCode:
            case UISceneModuleType.Turnplate:
            case UISceneModuleType.SmashEgg:
            case UISceneModuleType.ConvertCenter:
            case UISceneModuleType.ExchangeActivity:
            case UISceneModuleType.LuckyNumbers:
            case UISceneModuleType.GamePlayingActivity:
            case UISceneModuleType.ClearanceRedPaper:
            case UISceneModuleType.Achievement:
                SetEventLayer(EventLayer.SecondDialog);
                break;
            case UISceneModuleType.Shop:
            case UISceneModuleType.Strength:
            case UISceneModuleType.OneKeyToFullLevel:
            case UISceneModuleType.AotemanZhaohuan:
            case UISceneModuleType.ExchangeCodeReward:
            case UISceneModuleType.TurnplateAward:
            case UISceneModuleType.AwardItemBox:
            case UISceneModuleType.DailyMissionReward:
            case UISceneModuleType.HuaFeiDisplay:
            case UISceneModuleType.HistoricHuaFeiDisplay:
            case UISceneModuleType.PhoneNumberInput:
            case UISceneModuleType.GamePassedGfit:
            case UISceneModuleType.DiscountGift:
            case UISceneModuleType.RemoveAds:
            case UISceneModuleType.NewPlayerGift:
            case UISceneModuleType.MonthCardGift:
            case UISceneModuleType.MonthCardGiftReward:
            case UISceneModuleType.CommonGift:
            case UISceneModuleType.CommonGiftAward:
                SetEventLayer(EventLayer.ThirdDialog);
                break;
            case UISceneModuleType.ShopSecondSure:
            case UISceneModuleType.GiftPackage:
            case UISceneModuleType.PlayerLevel:
                SetEventLayer(EventLayer.FourthDialog);
                break;
            case UISceneModuleType.HintInGame:
            case UISceneModuleType.GameResume:
            case UISceneModuleType.GameSkill:
                SetEventLayer(EventLayer.Nothing);
                break;
            case UISceneModuleType.UIGuide:
                SetEventLayer(EventLayer.Guide);
                break;
        }
    }

    public void SetEventLayer(EventLayer layer)
    {
        switch (layer)
        {
            case EventLayer.Nothing:
                UICamera.AssignRaycastLayerMask(0);
                DialogCamera1.AssignRaycastLayerMask(0);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(0);
                break;
            case EventLayer.EveryThing:
                UICamera.AssignRaycastLayerMask(-1);
                DialogCamera1.AssignRaycastLayerMask(-1);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(-1);
                break;
            case EventLayer.UI:

                UICamera.AssignRaycastLayerMask(1 << 5);
                DialogCamera1.AssignRaycastLayerMask(1 << 5);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(1 << 5);
                break;
            case EventLayer.FirstDialog:
                UICamera.AssignRaycastLayerMask(1 << 11);
                DialogCamera1.AssignRaycastLayerMask(1 << 11);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(1 << 11);
                break;
            case EventLayer.SecondDialog:
                UICamera.AssignRaycastLayerMask(1 << 12);
                DialogCamera1.AssignRaycastLayerMask(1 << 12);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(1 << 12);
                break;
            case EventLayer.ThirdDialog:
                UICamera.AssignRaycastLayerMask(1 << 13);
                DialogCamera1.AssignRaycastLayerMask(1 << 13);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(1 << 13);
                break;
            case EventLayer.FourthDialog:
                UICamera.AssignRaycastLayerMask(1 << 14);
                DialogCamera1.AssignRaycastLayerMask(1 << 14);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(1 << 14);
                break;
            case EventLayer.Guide:
                UICamera.AssignRaycastLayerMask(1 << 17);
                DialogCamera1.AssignRaycastLayerMask(1 << 17);
                if (DialogCamera2 != null)
                    DialogCamera2.AssignRaycastLayerMask(1 << 17);
                break;
        }
    }
}

public enum EventLayer
{
    Nothing,
    EveryThing,
    UI,
    FirstDialog,
    SecondDialog,
    ThirdDialog,
    FourthDialog,
    Guide
}