using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActivityItem : MonoBehaviour {
	
	public GameObject goGetButton;
	public GameObject grayGetButton;
	public tk2dSprite IconImage;
	public EasyFontTextMesh DescText;
	public EasyFontTextMesh TitleText;
	public GameObject TipsGameobj;
	
	private int index;
	private UISceneModuleType goSceneModuleType;
	
	public void Init(int index)
	{
		this.index = index;
		
		SetIconImage (ActivityData.Instance.GetIconName (index));
		SetTitleText (ActivityData.Instance.GetTitle (index));
		SetDescText (ActivityData.Instance.GetDesc (index));
		goSceneModuleType = ActivityData.Instance.GetUISceneModuleType (index);
		SetBtnState ();
	}
	
	void SetBtnState()
	{
		bool isOpenActvity = false;
		switch(goSceneModuleType)
		{
		case UISceneModuleType.LuckyNumbers:
			isOpenActvity = PlatformSetting.Instance.isOpenLuckyNumbersActivity;
			break;
		case UISceneModuleType.ExchangeActivity:
			isOpenActvity = PlatformSetting.Instance.isOpenExchangeActivity;
			break;
		case UISceneModuleType.GamePlayingActivity:
			isOpenActvity = PlatformSetting.Instance.isOpenGamePlayingActivity;
			break;
		case UISceneModuleType.ClearanceRedPaper:
			isOpenActvity = PlatformSetting.Instance.isOpenClearanceRedPaper;
			break;
		}
		goGetButton.SetActive(isOpenActvity);
		grayGetButton.SetActive(!isOpenActvity);
	}
	
	public void SetIconImage(string iconName)
	{
		IconImage.SetSprite(iconName);
	}
	
	public void SetDescText(string desc)
	{
		DescText.text = desc;
	}
	
	public void SetTitleText(string title)
	{
		TitleText.text = title;
	}
	
	void GoOnClick()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(goSceneModuleType);
	}
}