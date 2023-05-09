using UnityEngine;
using System.Collections;
using PayBuild;


public class RemoveAdsController : UIBoxBase
{

    public GameObject goShenheBG;
    public GameObject goBuyButton, goCancelBtn;
    public EasyFontTextMesh titleText, textHint1, textHint2, textDesc1, textDesc2, textPayBt;
    public tk2dSprite imageCloseBt;

    // public tk2dSprite[] iconSpriteArr;
    // public tk2dTextMesh[] countTextArr;

    public override void Init()
    {
        // titleText.text = PayJsonData.Instance.GetGiftTitle(PayType.DiscountGift);
        // string[] IconArr = PayJsonData.Instance.GetGiftItemsIconArr(PayType.DiscountGift);
        // int[] CountArr = PayJsonData.Instance.GetGiftItemsCountsArr(PayType.DiscountGift);

        //for (int i = 0; i < IconArr.Length; ++i)
        //{
        //    if (i < IconArr.Length)
        //    {
        //        if (IconArr[i].CompareTo("coin_icon") == 0 || IconArr[i].CompareTo("jewel_icon") == 0)
        //            IconArr[i] = IconArr[i].Replace("icon", "2");
        //        iconSpriteArr[i].SetSprite(IconArr[i]);
        //        countTextArr[i].text = "X" + CountArr[i].ToString();
        //    }
        //}

        base.Init();
    }

    public override void Show()
    {
        base.Show();

        PayUIConfigurator.PayUIConfig(PayType.RemoveAds, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goBuyButton.GetComponent<BoxCollider>(), SetCancelBt);
        // CollectInfoEvent.SendGiftEvent(CollectInfoEvent.EventType.Gift_LimitTime, GiftEventType.Show);

        if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe)
        {            
            goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false);
        }
    }

    void SetCancelBt(bool state)
    {
        if (state)
        {
            goCancelBtn.SetActive(true);
            goCancelBtn.transform.localPosition = new Vector3(-88, -135, 0);
            goBuyButton.transform.localPosition = new Vector3(115, -135, 0);
        }
        else
        {
            goCancelBtn.SetActive(false);
            goBuyButton.transform.localPosition = new Vector3(90, -135, 0);
        }
    }

    public override void Hide()
    {
        base.Hide();
        UIManager.Instance.HideModule(UISceneModuleType.RemoveAds);

        //if (PlatformSetting.Instance.IsOpenCommonGiftType && !PlayerData.Instance.GetCommonGiftIsBuy() && preBoxType == UISceneModuleType.LevelInfo)
        //{
        //    UIManager.Instance.ShowModule(UISceneModuleType.CommonGift);
        //}

        ////弹完C包后，显示关卡信息
        //else if (GlobalConst.StartGameGuideEnabled)
        //{
        //    UIGuideControllor.Instance.Show(UIGuideType.LevelInfoGuide);
        //}
    }

    public override void Back()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
        Hide();
    }

    public void CancelOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
        Hide();
    }

    public void CloseOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        Hide();
    }

    public void BuyOnClick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        PayBuildPayManage.Instance.Pay((int)PayType.RemoveAds, GetGiftPayCallBack);

        // CollectInfoEvent.SendGiftEvent(CollectInfoEvent.EventType.Gift_LimitTime, GiftEventType.Click);
    }

    void GetGiftPayCallBack(string result)
    {
        if (result.CompareTo("Success") != 0)
            return;

        //PlayerData.ItemType[] itemType = PayJsonData.Instance.GetGiftItemsTypeArr(PayType.DiscountGift);
        //int[] count = PayJsonData.Instance.GetGiftItemsCountsArr(PayType.DiscountGift);

        //for (int i = 0; i < itemType.Length; i++)
        //{
        //    PlayerData.Instance.AddItemNum(itemType[i], count[i]);
        //}
        Hide();
        PlayerData.Instance.SetRemoveAds(true);
        PlayerData.Instance.SaveData();
        MainInterfaceControllor.Instance.UpdateRemoveAdBtn();

        // CollectInfoEvent.SendGiftEvent(CollectInfoEvent.EventType.Gift_LimitTime, GiftEventType.Pay);
    }
}
