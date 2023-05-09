using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ExchangeCodeControllor : UIBoxBase {

	public GameObject goCloseButton, goExchangeButton;
	public EasyFontTextMesh CodeText;
	public EasyFontTextMesh HintText;
	
	public override void Init ()
	{
		base.Init();
	}
	
	public override void Show ()
	{		
		base.Show ();
	}
	
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.ExchangeCode);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	private bool CheckInputWordIsOK(char word)
	{
		if((int)word >= (int)'a' && (int)word <= (int)'z')
			return true;
		
		if((int)word >= (int)'0' && (int)word <= (int)'9')
			return true;
		
		return false;
	}

	void ShowDisableKeyHint()
	{
		HintText.text = "兑换码无效或已被使用";
		HintText.gameObject.SetActive (true);
//		HintText.SetColor(new Color (46/255.0f, 207/255.0f, 241/255.0f, 1));

		StopCoroutine ("HideDisableKeyHintIE");
		StartCoroutine("HideDisableKeyHintIE");
	}
	
	void HideDisableKeyHint()
	{
		HintText.gameObject.SetActive (false);
//		HintText.SetColor(new Color (46/255.0f, 207/255.0f, 241/255.0f, 0));
	}
	
	IEnumerator HideDisableKeyHintIE()
	{
		yield return new WaitForSeconds(0.8f);
		HideDisableKeyHint();
	}

	private void ExchangeEvent()
	{
		string tempKey = CodeText.text;
		tempKey = tempKey.Replace("-", "");
		tempKey = tempKey.Replace(" ", "");
		tempKey = tempKey.ToLower();

		for(int i = 0; i < tempKey.Length; i++)
		{
			if(!CheckInputWordIsOK(tempKey[i]))
			{
				ShowDisableKeyHint();
				return;
			}
		}

		bool isActiveKey = false, isAlreadyHasSameTypeCode = true;
		string rewardContent = null;
		
		if(tempKey.Length == 14)
		{
			isActiveKey = ExchangeCodeTool.CheckExchangeCode(tempKey, out rewardContent);
			isAlreadyHasSameTypeCode = ExchangeCodeTool.CheckPlayerHasGetOneTypeCode(tempKey);
		}

		if(!isActiveKey || isAlreadyHasSameTypeCode || string.IsNullOrEmpty(rewardContent))
		{
			ShowDisableKeyHint();
		}else
		{
			HideDisableKeyHint();

			ExchangeCodeRewardControllor.Instance.InitData (rewardContent);
			UIManager.Instance.ShowModule(UISceneModuleType.ExchangeCodeReward);
			
			PlayerData.Instance.AddExchangeCode(tempKey);
			PlayerData.Instance.SaveData();
			
			CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.ExchangeCode, "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel ());
		}
	}

	void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	void ExchangeOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		ExchangeEvent ();
	}
}
