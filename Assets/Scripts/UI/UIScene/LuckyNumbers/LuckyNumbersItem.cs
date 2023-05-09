using UnityEngine;
using System.Collections;

public class LuckyNumbersItem : MonoBehaviour {

	public enum BtnState
	{
		UnFinishState,
		ReciveState
	};
	public ParticleSystem uiJiangLiPs;
	public ParticleSystem uiFangkuangPs;
	public tk2dSprite iconImage;
	public EasyFontTextMesh awardCount;
	public EasyFontTextMesh desc;
	public Transform greenBtnImage;
	public Transform yellowBtnImage;
	public LuckyNumbersType luckyNumbersType;
	private BtnState curBtnState;

	private int AwardType;
	private int AwardCount;


	public void Init(int id)
	{
		AwardType = LuckyNumbersData.Instance.GetAwardType (id);
		iconImage.SetSprite (RewardData.Instance.GetIconName(AwardType));
		AwardCount = LuckyNumbersData.Instance.GetAwardCount (id);
		awardCount.text = "X" + AwardCount;
		desc.text = LuckyNumbersData.Instance.GetDesc (id);
		luckyNumbersType = LuckyNumbersData.Instance.GetLuckyNumbersType (id);
		if (GetBtnState ()) {
			curBtnState = BtnState.ReciveState;
			greenBtnImage.gameObject.SetActive(false);
			yellowBtnImage.gameObject.SetActive(true);
			LuckyNumbersController.Instance.TipCount++;
			ActivityControllor.Instance.TipCount++;
			uiFangkuangPs.gameObject.SetActive(true);
		}
		else
		{
			curBtnState = BtnState.UnFinishState;
			greenBtnImage.gameObject.SetActive(true);
			yellowBtnImage.gameObject.SetActive(false);
		}
	}

	bool GetBtnState()
	{
		switch(luckyNumbersType)
		{
		case LuckyNumbersType.One:
			return PlayerData.Instance.GetLuckyNumbersOneState();
		case LuckyNumbersType.Six:
			return PlayerData.Instance.GetLuckyNumbersSixState();
		case LuckyNumbersType.Eight:
			return PlayerData.Instance.GetLuckyNumbersEightState();
		case LuckyNumbersType.OneSixEight:
			return PlayerData.Instance.GetLuckyNumbersOneSixEightState();
		}
		return false;
	}
	void SetBtnState()
	{
		switch(luckyNumbersType)
		{
		case LuckyNumbersType.One:
			PlayerData.Instance.SetLuckyNumbersOneState(false);
			break;
		case LuckyNumbersType.Six:
			PlayerData.Instance.SetLuckyNumbersSixState(false);
			break;
		case LuckyNumbersType.Eight:
			PlayerData.Instance.SetLuckyNumbersEightState(false);
			break;
		case LuckyNumbersType.OneSixEight:
			PlayerData.Instance.SetLuckyNumbersOneSixEightState(false);
			break;
		}
		curBtnState = BtnState.UnFinishState;
		greenBtnImage.gameObject.SetActive(true);
		yellowBtnImage.gameObject.SetActive(false);
	}

	public void OnClick()
	{
			SetBtnState ();
			PlayerData.Instance.AddItemNum ((PlayerData.ItemType)AwardType, AwardCount);
			LuckyNumbersController.Instance.TipCount--;
			ActivityControllor.Instance.TipCount--;
			uiFangkuangPs.gameObject.SetActive(false);
			uiJiangLiPs.Play();
	}
}
