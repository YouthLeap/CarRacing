using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConvertItem : MonoBehaviour {

	public ParticleSystem particle;
	public GameObject goConvertButton;
	public tk2dSprite IconImage;
	public EasyFontTextMesh TipText;
	public EasyFontTextMesh CountText;
	public tk2dSprite FruitImage;
	public EasyFontTextMesh TitleText;
	public EasyFontTextMesh FruitCountText;
	public ProgressBarNoMask progress;
	public ParticleSystem OnClickParticle;

	private int index;
	private int fruitCount;
	private int totalCount;

	private PlayerData.ItemType itemType;
	private PlayerData.ItemType fruitItemType;

	[HideInInspector]
	public bool convertFlag;
	
	public void Init(int index)
	{
		this.index = index;
		itemType = ConvertCenterData.Instance.GetItemType(index);
		fruitItemType = ConvertCenterData.Instance.GetFruitItemType(index);
		SetIconImage(ConvertCenterData.Instance.GetTargetIcon(index));
		SetFruitImage(ConvertCenterData.Instance.GetMaterialIcon(index));
		SetTitleText(ConvertCenterData.Instance.GetTargetName(index));
		InitData ();
	}
	
	public void InitData()
	{
		SetCountText(PlayerData.Instance.GetItemNum(itemType));
		fruitCount = PlayerData.Instance.GetItemNum(this.fruitItemType);
		totalCount = ConvertCenterData.Instance.GetMaterialCount(this.index);
		SetFruitCountText(fruitCount, totalCount);
		SetProgress((float)fruitCount / totalCount);
	}

	public void SetIconImage(string iconName)
	{
		IconImage.SetSprite(iconName);
	}

	public void SetFruitImage(string fruitName)
	{
		FruitImage.SetSprite(fruitName);
	}

	public void SetTitleText(string title)
	{
		TitleText.text = title;
	}

	public void SetCountText(int count)
	{
		CountText.text = "x" + count;
	}

	public void SetFruitCountText(int fruitCount, int totalCount)
	{
		this.fruitCount = fruitCount;
		this.totalCount = totalCount;
		FruitCountText.text = fruitCount + "/" + totalCount;
	}

	public void SetProgress(float percent)
	{
		if (percent >= 1) {
			percent = 1;
			convertFlag = true;
			goConvertButton.SetActive (true);
			TipText.gameObject.SetActive (false);
		} else {
			convertFlag = false;
			goConvertButton.SetActive (false);
			TipText.gameObject.SetActive (true);
		}
		progress.SetProgress (percent);
	}

	void ConvertOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		if(this.fruitCount >= this.totalCount)
		{
			OnClickParticle.Play();
			AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
			PlayerData.Instance.ReduceItemNum(fruitItemType, this.totalCount);
			PlayerData.Instance.AddItemNum(itemType, 1);
			ConvertCenterControllor.Instance.RefreshData ();
			particle.Play();
			//自定义事件.
			CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.ConvertCenter, "兑换类型", itemType.ToString ());

		}
	}
}
