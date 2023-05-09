using UnityEngine;
using System.Collections;

public class GamePlayingActivity : UIBoxBase {

	private const int awardStageCount = 4;
	public ProgressBarNoMask progress;
	public Transform iconTran;
	public tk2dSprite[] awardTypeSprite;
	private int[] awardTypeId = new int[awardStageCount];
	public EasyFontTextMesh[] awardCountText;
	private int[] awardCount = new int[awardStageCount];
	public EasyFontTextMesh[] scoreText;
	private int[] score = new int[awardStageCount];

	public ParticleSystem[] uiJiangLiPs;
	public Transform lightBtn;
	public Transform grayBtn;
	private const float startY = -267;
	private bool isCanClickGet = false;
	private int hasScore;
	public override void Init ()
	{
		if (!PlatformSetting.Instance.isOpenGamePlayingActivity)
		{
			return;
		}
		int totalScore = GamePlayingActivityData.Instance.GetScore (awardStageCount);
		for(int i =  1; i <= 4; i++ )
		{
			score[i - 1] = GamePlayingActivityData.Instance.GetScore(i);
			scoreText[i - 1].text = score[i - 1].ToString();
			awardTypeId[i - 1] = GamePlayingActivityData.Instance.GetAwardType(i);
			awardTypeSprite[i - 1].SetSprite(RewardData.Instance.GetIconName(awardTypeId[i - 1]));
			awardCount[i - 1] = GamePlayingActivityData.Instance.GetAwardCount(i);
			awardCountText[i - 1].text = "X" + awardCount[i - 1];
			awardCountText[i - 1].transform.parent.localPosition = new Vector3(startY + score[i - 1] * 1.0f / totalScore * 2 * Mathf.Abs(startY) ,4,0);
		}
		hasScore = PlayerData.Instance.GetGamePlayingActivityScore();
		float ratio = Mathf.Clamp01 (hasScore * 1.0f / totalScore);
		progress.SetProgress (ratio);
		iconTran.localPosition = new Vector3 (startY + ratio * 2 * Mathf.Abs(startY),iconTran.localPosition.y,0);
		SetGetBtn ();

		base.Init();
	}
	void SetGetBtn()
	{
		int hasStage = PlayerData.Instance.GetGamePlayingActivityStage ();
		for(int i = 0; i < hasStage - 1; i++)
		{
			uiJiangLiPs[i].gameObject.SetActive(false);
		}
		if(PlayerData.Instance.GetGamePlayingActivityAllAchievie())
		{
			lightBtn.gameObject.SetActive (false);
			grayBtn.gameObject.SetActive(true);
			isCanClickGet = false;
			ActivityControllor.Instance.ActivityItemList[2].TipsGameobj.SetActive(false);
			uiJiangLiPs[hasStage - 1].gameObject.SetActive(false);
		}
		else if (PlayerData.Instance.GetGamePlayingActivityAchievie ()) {
			lightBtn.gameObject.SetActive (true);
			grayBtn.gameObject.SetActive(false);
			isCanClickGet = true;
			ActivityControllor.Instance.TipCount++;
			ActivityControllor.Instance.ActivityItemList[2].TipsGameobj.SetActive(true);
		} else {
			if(hasScore > score[hasStage - 1])
			{
				PlayerData.Instance.SetGamePlayingActivityAchievie(true);
				lightBtn.gameObject.SetActive (true);
				grayBtn.gameObject.SetActive(false);
				isCanClickGet = true;
				ActivityControllor.Instance.TipCount++;
				ActivityControllor.Instance.ActivityItemList[2].TipsGameobj.SetActive(true);
			}
			else
			{
				lightBtn.gameObject.SetActive (false);
				grayBtn.gameObject.SetActive(true);
				isCanClickGet = false;
				ActivityControllor.Instance.ActivityItemList[2].TipsGameobj.SetActive(false);
			}
		}
	}
	public override void Show ()
	{
		base.Show ();
	}
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.GamePlayingActivity);
	}
	public override void Back ()
	{
		Hide ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
	}
	public void Close()
	{
		Hide ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
	}
	public void GetBtn()
	{
		if (isCanClickGet) {
			ActivityControllor.Instance.TipCount--;
			int hasStage = PlayerData.Instance.GetGamePlayingActivityStage ();
			PlayerData.Instance.AddItemNum ((PlayerData.ItemType)awardTypeId[hasStage - 1], awardCount[hasStage - 1]);
			if(hasStage < 4)
			{
				PlayerData.Instance.SetGamePlayingActivityStage(hasStage + 1);
				PlayerData.Instance.SetGamePlayingActivityAchievie(false);
			}
			else
			{
				PlayerData.Instance.SetGamePlayingActivityAchievie(true);
				PlayerData.Instance.SetGamePlayingActivityAllAchievie(true);
			}
			SetGetBtn();
				
		}
	}
}
