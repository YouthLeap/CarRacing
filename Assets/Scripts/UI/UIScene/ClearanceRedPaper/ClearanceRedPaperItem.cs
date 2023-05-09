using UnityEngine;
using System.Collections;

public class ClearanceRedPaperItem : MonoBehaviour {
	public ParticleSystem uiFangkuangPs;
	public ParticleSystem uiHongbaoPs;
	public ParticleSystem uiJinagliPs;
	public ParticleSystem uiDianjiPs;
	public Transform getBtnTran;
	public Transform beforeGetTran;
	public tk2dSprite beforeGetSpite;
	public EasyFontTextMesh targetLevelText;
	public tk2dSprite[] awardTypeSprit;
	public EasyFontTextMesh[] awardCountText;
	private int[] awardTypeIds;
	private int[] awardCount;
	private int targetLevelId;
	private int index;
	public void Init(int id)
	{
		targetLevelId = int.Parse (ClearanceRedPaperData.Instance.GetTargetLevel (id));
		targetLevelText.text = "Complete " + ClearanceRedPaperData.Instance.GetTargetLevel (id) + "";
		awardTypeIds = ClearanceRedPaperData.Instance.GetAwardType (id);
		awardCount = ClearanceRedPaperData.Instance.GetAwardCount (id);
		for(int i = 0; i < awardTypeIds.Length; i++)
		{
			awardTypeSprit[i].SetSprite(RewardData.Instance.GetIconName(awardTypeIds[i]));
			awardCountText[i].text = "X" + awardCount[i];
		}
		SetBtnState ();
	}
	void SetBtnState()
	{
		int[] ids = PlayerData.Instance.GetClearanceRedIds ();
		for(int i = 0; i < ids.Length; i++)
		{
			if(ids[i] == targetLevelId)
			{
				index = i;
				break;
			}
		}
		bool isCanGet = PlayerData.Instance.GetClearanceRedState (index) == 0 ? false : true;
		bool isAlreadyGet = PlayerData.Instance.GetClearanceRedAlreadyIsGet (index) == 0 ? false : true;

		if(isAlreadyGet)
		{
			beforeGetTran.gameObject.SetActive(true);
			getBtnTran.gameObject.SetActive(false);
			//beforeGetText.text = "已领取";
			beforeGetSpite.SetSprite ("after_get");
		}else if(!isAlreadyGet && isCanGet)
		{
			beforeGetTran.gameObject.SetActive(false);
			getBtnTran.gameObject.SetActive(true);
			ClearanceRedPaper.Instance.TipCount++;
			ActivityControllor.Instance.TipCount++;
		}
		else if(!isAlreadyGet && !isCanGet)
		{
			if(targetLevelId < PlayerData.Instance.GetCurrentChallengeLevel())
			{
				PlayerData.Instance.SetClearanceRedState(index,1);
				beforeGetTran.gameObject.SetActive(false);
				getBtnTran.gameObject.SetActive(true);
				ClearanceRedPaper.Instance.TipCount++;
				ActivityControllor.Instance.TipCount++;
			}
			else
			{
				beforeGetTran.gameObject.SetActive(true);
				getBtnTran.gameObject.SetActive(false);
				//beforeGetText.text = "未完成";
				beforeGetSpite.SetSprite ("un_finish_gray");
			}
		}
	}
	public void OnClick()
	{
		for(int i = 0; i < awardTypeIds.Length; i++)
		{
			PlayerData.Instance.AddItemNum((PlayerData.ItemType)awardTypeIds[i],awardCount[i]);
		}
		beforeGetTran.gameObject.SetActive(true);
		getBtnTran.gameObject.SetActive(false);
		//beforeGetText.text = "已领取";
		beforeGetSpite.SetSprite ("after_get");
		PlayerData.Instance.SetClearanceRedState(index,0);
		PlayerData.Instance.SetClearanceRedAlreadyIsGet(index,1);
		ClearanceRedPaper.Instance.TipCount--;
		ActivityControllor.Instance.TipCount--;
		StartCoroutine ("IEPlayPs");
	}
	IEnumerator IEPlayPs()
	{
		uiDianjiPs.Play ();
		yield return new WaitForSeconds (0.1f);
		uiHongbaoPs.Play ();
		yield return new WaitForSeconds (0.2f);
		uiJinagliPs.Play ();
		yield return new WaitForSeconds (1.2f);
		uiFangkuangPs.Play();
	}
}
