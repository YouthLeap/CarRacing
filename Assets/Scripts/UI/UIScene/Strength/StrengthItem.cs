using UnityEngine;
using System.Collections;
using DG.Tweening;

public class StrengthItem : MonoBehaviour {

	public GameObject goBuyButton;
	public Transform tranIconForAnimation;
	public EasyFontTextMesh CountText;
	public tk2dSprite CostTypeImage;
	public EasyFontTextMesh CostCountText,CountTipsText;

	[HideInInspector]public int iItemID;
	private int count;
	private int costCount;
	private PlayerData.ItemType itemType;

	public void Init()
	{
	}

	public void SetCountText(int count)
	{
		this.count = count;
		CountText.text = "x" + count;
		CountTipsText.text = count.ToString();
	}

	public void SetItemType(PlayerData.ItemType itemType)
	{
		this.itemType = itemType;
//		string typeName;
//		if (itemType == PlayerData.ItemType.Jewel) {
//			CostTypeImage.transform.localPosition = new Vector3 (7, 0, 0);
//		} else {
//			if(this.costCount > 10000)
//				CostTypeImage.transform.localPosition = new Vector3(7, 0, 0);
//			else
//				CostTypeImage.transform.localPosition = new Vector3(0, 0, 0);
//		}
	}

	public void SetCostTypeImage(string typeName)
	{
		CostTypeImage.SetSprite (typeName);
	}

	public void SetCostCountText(int costCount)
	{
		this.costCount = costCount;
		CostCountText.text = costCount.ToString ();
	}

	public void ShowIconAnimation()
	{
		tranIconForAnimation.gameObject.SetActive(true);
		tranIconForAnimation.localPosition = new Vector3(-130.4f, 1, 0);
		tranIconForAnimation.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		DOTween.Kill("IconAnimation");
		tranIconForAnimation.DOLocalMove(new Vector3(-367, 181 + 85 * (iItemID - 1), 0), 0.6f).SetId("IconAnimation").OnComplete(IconAnimationCall);
	}

	void IconAnimationCall()
	{
		tranIconForAnimation.gameObject.SetActive(false);
		tranIconForAnimation.localPosition = new Vector3(-100, 1, 0);
		tranIconForAnimation.localScale = new Vector3(0.7f, 0.7f, 0.7f);
	}

	public void BuyButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		if (PlayerData.Instance.GetItemNum (this.itemType) < this.costCount) {
			if(this.itemType == PlayerData.ItemType.Coin)
			{
				GiftPackageControllor.Instance.Show (PayType.CoinGift, GiftBoxCallBack);
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Coin,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
			}
			else
			{
				GiftPackageControllor.Instance.Show (PayType.JewelGift, GiftBoxCallBack);
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Jewel,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
			}
		} else {
			SetPlayerData ();
		}
		return;
	}

	private void SetPlayerData ()
	{
		ShowIconAnimation();
		PlayerData.Instance.ReduceItemNum (this.itemType, this.costCount);
		PlayerData.Instance.AddItemNum (PlayerData.ItemType.Strength, this.count);
		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Strength_Buy, "体力数量", this.count.ToString ());
	}

	private void GiftBoxCallBack()
	{
		SetPlayerData ();
	}
}
