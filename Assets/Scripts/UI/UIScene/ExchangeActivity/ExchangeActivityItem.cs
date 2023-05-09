using UnityEngine;
using System.Collections;

public class ExchangeActivityItem : MonoBehaviour {
    
	public EasyFontTextMesh titleText,priceText,restText,grayBtnText;
	public GameObject GetBtn, GrayBtn;
	private string award="";


	private ExchangeActivityData.DataStruct data;

	public void SetData(ExchangeActivityData.DataStruct data)
	{
		this.data = data;

		int eggCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.ColorEgg);
		if (data.isTrueGift) {
			if(ExchangeActivityData.Instance.GetServerProbability(data.awardId-1)<=0  || ExchangeActivityData.Instance.GetActivityEnable()==false)
			{
				data.rest=0;
			}
			//是否能获得实物的条件
			bool isGetGift = eggCount >= this.data.price && data.rest > 0 && ExchangeActivityData.Instance.GetServerProbability(data.awardId-1)>0  && data.isGet==false;
			if(isGetGift)
			{
				GrayBtn.SetActive (false);
				GetBtn.SetActive(true);
			}else
			{
				GrayBtn.SetActive (true);
				GetBtn.SetActive(false);
			}

			if(data.isGet)
			{
				grayBtnText.text="已领取";
			}else
			{
				grayBtnText.text="未能领取";
			}


		} else {
			if (eggCount >= this.data.price) { /*reward item*/
				GrayBtn.SetActive (false);
				GetBtn.SetActive(true);
			}
		    else {
				GrayBtn.SetActive (true);
				GetBtn.SetActive(false);
			}
		}


		titleText.text = data.name;
		priceText.text = data.price.ToString ();
		restText.text = data.rest.ToString ();
		award = data.award;
	}


	public void OnClick()
	{
		int eggCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.ColorEgg);
		if (eggCount < this.data.price)
			return;

		if (data.isTrueGift) {
			PhoneNumberInputControllor.CommitPhoneNumCallBack += InPutCallBack;
			UIManager.Instance.ShowModule (UISceneModuleType.PhoneNumberInput);
		} else {

			string[] itemData = data.award.Split('*');
			PlayerData.ItemType type = RewardData.Instance.GetItemType( int.Parse(itemData[0]));
			int count = int.Parse(itemData[1]);
			PlayerData.Instance.AddItemNum(type,count);
			PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.ColorEgg,data.price);

			AwardItemBoxControllor.Instance.InitData(int.Parse(itemData[0]),count);
			UIManager.Instance.ShowModule(UISceneModuleType.AwardItemBox);

			this.data.rest--;
			ExchangeActivityData.Instance.SetItemData (data);
			ExchangeActivityControllor.Instance.InitData ();
		}

		ExchangeActivityControllor.Instance.InitData ();
	}

	void InPutCallBack()
	{
		this.data.rest--;
		this.data.isGet = true;
	    data.rest = data.rest < 0 ? 0 : data.rest;

		ExchangeActivityData.Instance.SetItemData (data);
		ExchangeActivityData.Instance.SaveData ();

		PlayerData.Instance.ReduceItemNum (PlayerData.ItemType.ColorEgg, this.data.price);

		ExchangeActivityData.Instance.SendAwardInfo (data.awardId);
		ExchangeActivityControllor.Instance.InitData ();

		ExchangeActivityData.Instance.InitFromServer ();
	}
}
