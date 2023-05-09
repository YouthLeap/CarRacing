using UnityEngine;
using System.Collections;

public class LevelSelectItem : MonoBehaviour {

	public GameObject goLevelButton;
	public tk2dSprite IconHeadImage;
	public tk2dSprite IconHeadGrayImage;
	public tk2dSprite IconImage;
	public ParticleSystem LevelPS;
	public Transform StarContainer;
	public Transform YellowStarTran;
	public EasyFontTextMesh LevelText;

	public int levelId;
	private bool lockFlag;

	public void Init()
	{
	}

	public void SetLevelText(int levelId)
	{
		this.levelId = levelId;
		if (this.levelId < 10)
			LevelText.text = "0" + this.levelId;
		else
			LevelText.text = this.levelId.ToString ();
	}

	public void SetLevelTextColor(Color textColor)
	{
		//LevelText.GetComponent<Outline> ().effectColor = textColor;
	}

	public void SetButtonEnable(bool flag)
	{
		goLevelButton.GetComponent<BoxCollider> ().enabled = flag;
	}

	public void SetLockFlag(bool flag)
	{
		lockFlag = flag;
	}

	public void SetIconHeadGrayImage(string iconName)
	{
		IconHeadGrayImage.SetSprite (iconName);
	}

	public void SetIconHeadImage(string iconName)
	{
		IconHeadImage.SetSprite (iconName);
	}
	public void SetIconImage(string iconName)
	{
		IconImage.SetSprite (iconName);
	}
	

	public void ShowLevelPS()
	{
		LevelPS.gameObject.SetActive (true);
		LevelPS.Play ();
	}

	public void ShowStar(bool flag)
	{
		StarContainer.gameObject.SetActive (flag);
	}

	public void SetStarCount(int count)
	{
		for (int i=0; i<count; ++i) {
			YellowStarTran.Find("Star"+i).gameObject.SetActive(true);
		}
		for (int i=count; i<3; ++i) {
			YellowStarTran.Find("Star"+i).gameObject.SetActive(false);
		}
	}

	public void LevelOnClick()
	{
		if (lockFlag)
			return;
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		LevelInfoControllor.Instance.InitData (levelId);
		UIManager.Instance.ShowModule (UISceneModuleType.LevelInfo);
		LevelSelectControllor.Instance.PlayDianJiPS (this.transform.position);
	}
}
