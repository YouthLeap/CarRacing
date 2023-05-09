using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public enum GetState
{
	NowGet = 1,
	BeforeGet,
	AfterGet
};
public class AchievementItem : MonoBehaviour {

   
	public GameObject goGetButton;
	public tk2dSprite IconImage;
	public ProgressBarNoMask progress;
	public EasyFontTextMesh DescText;
	public EasyFontTextMesh ProgressText;
	public EasyFontTextMesh CoinCountText, JewelCountText;
	public EasyFontTextMesh TitleText;

	public ParticleSystem Particle1,Particle2;
	public ParticleSystem OnClickParticle;

	public GameObject BeforeGet, NowGet, AfterGet, ContentGO;

//	[HideInInspector]
	public int achievementId;
    public int achievementIndex;
	[HideInInspector]
	public int achievementLevel;
	private int[] rewardIdArr;
	private int[] rewardCountArr;
	private int curNum;
	private int targetNum;
	[HideInInspector]
	public GetState getState;
	private bool isCumulative;

	IEnumerator PlayParticle()
	{
		Particle1.gameObject.SetActive (true);
		Particle1.Play ();
		yield return new WaitForSeconds (0.05f);
		Particle2.gameObject.SetActive (true);
		Particle2.Play ();
	}

 

    void OnDisable()
	{
		Particle1.gameObject.SetActive (false);
		Particle2.gameObject.SetActive (false);
	}

	public void Init(int achievementId, int achievementIndex)
	{
		this.achievementId = achievementId;
		this.achievementIndex = achievementIndex;

		InitData ();
	}


	public void InitData()
	{
		this.achievementLevel = AchievementData.Instance.GetLevel (achievementId);
		
		SetIconImage (AchievementData.Instance.GetIconName (achievementId));
		SetTitleText (AchievementData.Instance.GetTitleName (achievementId));
		SetDescText (AchievementData.Instance.GetDesc (achievementId));
		
		isCumulative = AchievementData.Instance.GetIsCumulative (achievementId);

		curNum = (int)PlayerData.Instance.GetAchievementCurrentNum (achievementIndex);
		targetNum = AchievementData.Instance.GetTargetNum (achievementId);
		
		SetProgressText (curNum, targetNum);
		SetProgressSlider ((float)curNum / targetNum);
		
		if (PlayerData.Instance.GetAchievementAreadyIsGet (achievementIndex) == 0) {
			if (curNum < targetNum)
				getState = GetState.BeforeGet;
			else
				getState = GetState.NowGet;
		} else {
			getState = GetState.AfterGet;
		}

		SetGetState (getState);

		rewardIdArr = AchievementData.Instance.GetRewardIdArr (achievementId);
		rewardCountArr = AchievementData.Instance.GetRewardCountArr (achievementId);
		SetCoinCountText (rewardCountArr [0]);
		SetJewelCountText (rewardCountArr [1]);
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

	public void SetProgressSlider(float percent)
	{
		progress.SetProgress (percent);
	}

	public void SetProgressText(int curNum, int targetNum)
	{
		ProgressText.text = curNum + "/" + targetNum;
	}

	public void SetCoinCountText(int coinCount)
	{
		CoinCountText.text = "x" + coinCount;
	}

	public void SetJewelCountText(int jewelCount)
	{
		JewelCountText.text = "x" + jewelCount;
	}

	public void SetGetState(GetState getState)
	{
		this.getState = getState;
		switch (getState) {
		case GetState.BeforeGet:
			BeforeGet.SetActive(true);
			NowGet.SetActive(false);
			ContentGO.SetActive(true);
			AfterGet.SetActive(false);
			break;
		case GetState.NowGet:
			BeforeGet.SetActive(false);
			NowGet.SetActive(true);
			ContentGO.SetActive(true);
			AfterGet.SetActive(false);
			break;
		case GetState.AfterGet:
			BeforeGet.SetActive(false);
			NowGet.SetActive(false);
			ContentGO.SetActive(false);
			AfterGet.SetActive(true);
			break;
		}
	}

	void GetOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
		OnClickParticle.Play ();
		if(getState == GetState.NowGet)
		{
			if(achievementLevel < 3)
			{
				if(isCumulative)
				{
					PlayerData.Instance.SetAchievementCurrentNum (achievementIndex, curNum);
				}
				else
				{
					PlayerData.Instance.SetAchievementCurrentNum (achievementIndex, 0);
				}
				++ achievementId;
				PlayerData.Instance.SetAchievementIds(achievementIndex, achievementId);

				AchievementCheckManager.Instance.CheckAgainAfterGetReward (achievementId);
			}
			else
			{
				SetGetState(GetState.AfterGet);
				PlayerData.Instance.SetAchievementAreadyIsGet(achievementIndex, achievementId);
			}

			AchievementControllor.Instance.RefreshData ();
            AchievementControllor.Instance.MoveItemPosAfterGet(transform);

			for(int i=0; i<rewardIdArr.Length; ++i)
			{
				PlayerData.Instance.AddItemNum(RewardData.Instance.GetItemType(rewardIdArr[i]), rewardCountArr[i]);
			}
			StartCoroutine(PlayParticle());

		}
	}
}
