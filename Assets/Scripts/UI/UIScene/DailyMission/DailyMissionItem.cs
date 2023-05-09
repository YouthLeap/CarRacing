using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DailyMissionItem : MonoBehaviour {


	public tk2dSprite rewardTypeImage, countTypeImage;
	public EasyFontTextMesh descText, rewardCount, countText;
	public ProgressBarNoMask progressBar;
    public ParticleSystem RewardCoinParticle, RewardJewelParticle;
    public ParticleSystem OnClickParticle;

    public int missionId;

	public GameObject getBtnGo, beforeGetBtnGo, afterGetBtnGo;


	public enum State{
		Lock,
		Unfinished,
		CanGet,
		HasGot
	}

	public State getState=State.Unfinished;
	DailyMissionDataStruct itemData;


	public void SetData(int missionId)
	{
		this.missionId = missionId;
		int index = missionId - 1;
		itemData = DailyMissionCheckManager.Instance.missionList [index];

		descText.text = itemData.missionDes;
		if(itemData.missionRewardCount > 999)
			rewardCount.text = "+" + itemData.missionRewardCount.ToString();
		else
			rewardCount.text = "+" + itemData.missionRewardCount.ToString();


		//missionIconImage.SetSprite (itemData.icon);
		if (itemData.missionRewardType == 1){
			rewardTypeImage.SetSprite ("coin_1");
			countTypeImage.SetSprite ("coin_icon");
		} else {
			rewardTypeImage.SetSprite ("jewel_1");
			countTypeImage.SetSprite ("jewel_icon");
		}

		float percent = (float)itemData.missionCurCount / (float)itemData.missionTarget;
		progressBar.SetProgress (percent);

		if (itemData.missionCurCount < itemData.missionTarget) {

			getBtnGo.SetActive(false);
			beforeGetBtnGo.SetActive(true);
			afterGetBtnGo.SetActive(false);
			getState = State.Unfinished;
            countText.text = itemData.missionCurCount + "/" + itemData.missionTarget;
		} else if (itemData.isGetReward == false) {
			getBtnGo.SetActive(true);
			beforeGetBtnGo.SetActive(false);
			afterGetBtnGo.SetActive(false);
			getState = State.CanGet;
            countText.text = itemData.missionTarget + "/" + itemData.missionTarget;
		} else if (itemData.isGetReward == true) {
			getBtnGo.SetActive(false);
			beforeGetBtnGo.SetActive(false);
			afterGetBtnGo.SetActive(true);
			getState = State.HasGot;
            countText.text = itemData.missionTarget + "/" + itemData.missionTarget;
		}

	}

	public void GetOnClick()
	{
        OnClickParticle.Play();
        AudioManger.Instance.PlaySound (AudioManger.SoundName.ButtonClick);
		if (getState != State.CanGet)
			return;
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
		itemData.isGetReward = true;
		DailyMissionCheckManager.Instance.SaveState ();

		if (itemData.missionRewardType == 1)
			PlayerData.Instance.AddItemNum (PlayerData.ItemType.Coin, this.itemData.missionRewardCount);
		else
			PlayerData.Instance.AddItemNum (PlayerData.ItemType.Jewel, this.itemData.missionRewardCount);
		//UIParticleController.Instance.PlayEffect (UIParticleType.ButtonClick, lightBtnGo.transform.position);

		 bool isChange= DailyMissionCheckManager.Instance.ChangeToNextLevelMission (missionId);
		if(isChange)
		{
			ChangeAnimate();
		}

		DailyMissionControllor.Instance.SetData ();
        StartCoroutine(PlayParticle());
    }
	public void GreyGetOnClick()
	{
		Debug.Log ("GreyGetOnClick");
	}

    IEnumerator PlayParticle()
    {
        yield return new WaitForEndOfFrame();
        if (itemData.missionRewardType == 1)
        {
            RewardCoinParticle.gameObject.SetActive(true);
            RewardCoinParticle.Play();
        }
        else
        {
            RewardJewelParticle.gameObject.SetActive(true);
            RewardJewelParticle.Play();
        }
    }

	public void ChangeAnimate()
	{
		StartCoroutine("IEChangeAnimate");
	}
	IEnumerator IEChangeAnimate()
	{
		//yield return new WaitForSeconds(0.5f);
		transform.DOLocalMoveX(500f,0.25f);
		yield return new WaitForSeconds(0.26f);
		Vector3 pos = transform.localPosition;
		pos.x=-500f;
		transform.localPosition = pos;
		transform.DOLocalMoveX(0,0.2f);
	}
}
