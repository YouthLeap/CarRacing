using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using MiniJSON;

public class PhoneNumberInputControllor : UIBoxBase {

	public delegate void CommitPhoneNumHander();
	public static event CommitPhoneNumHander CommitPhoneNumCallBack;

	public tk2dUITextInput PhoneNumInput;
	public GameObject goNumDisableText;

	#region 重写父类方法
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
		UIManager.Instance.HideModule(UISceneModuleType.PhoneNumberInput);
		CommitPhoneNumCallBack = null;
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	#endregion

	#region  手机号码输入处理
	void ConfirmPhoneNumber()
	{
		string phoneNum = PhoneNumInput.Text;
		
		if(!CheckPhoneNumberEnabled(phoneNum))
			return;
		
		PlayerData.Instance.SetPhoneNumber(phoneNum);
		//发送玩家信息

		if(CommitPhoneNumCallBack != null)
		{
			CommitPhoneNumCallBack();
			CommitPhoneNumCallBack = null;
		}
		Hide();
	}
	
	bool CheckPhoneNumberEnabled(string num)
	{
		if(num.Length != 11)
		{
			ShowNumDisableText();
			return false;
		}
		
		for(int i = 0; i < num.Length; i ++)
		{
			if(num[i] < '0' || num[i] > '9')
			{
				ShowNumDisableText();
				return false;
			}
		}
		
		return true;
	}
	
	void ShowNumDisableText()
	{
		Debug.Log("ShowNumDisableText");
		goNumDisableText.SetActive(true);
		StartCoroutine("ShowNumDisableTextIE");
		
	}
	
	IEnumerator ShowNumDisableTextIE()
	{
		yield return new WaitForSeconds(0.8f);
		goNumDisableText.SetActive(false);
	}
	#endregion

	#region  按钮控制
	void CloseBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
		return;
	}

	void CommitPhoneNumBtOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		ConfirmPhoneNumber();
		return;
		
	}
	#endregion
}
