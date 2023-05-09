using UnityEngine;
using System.Collections;

public class UIItemData : MonoBehaviour {

	public tk2dClippedSprite ClippedSprite;
	public tk2dTextMesh UINumberText;
	public tk2dTextMesh UIJewelCostText;
	public tk2dTextMesh UICoinCostText;
	public GameObject UIJewelCostObj;
	public GameObject UICoinCostObj;
	public ParticleSystem ProtectShieldParticle;
	
	private PlayerData.ItemType SkillType;
	private float skillTime, coolTime;
	[HideInInspector]
	public bool coolFlag;

	public void Init (PlayerData.ItemType type)
	{
		SkillType = type;
		SetNumberText (PlayerData.Instance.GetItemNum (SkillType));

		switch (SkillType) {
		case PlayerData.ItemType.ProtectShield:
			skillTime = PropConfigData.Instance.GetSkillTime ((int)PropType.Shield);
			break;
		case PlayerData.ItemType.SpeedUp:
			skillTime = PropConfigData.Instance.GetSkillTime ((int)PropType.SpeedUp);
			break;
		case PlayerData.ItemType.FlyBomb:
			skillTime = PropConfigData.Instance.GetSkillTime ((int)PropType.FlyBmob);
			break;
		}
		coolFlag = false;
	}

	public void SetNumberText(int number)
	{
		if (Application.isPlaying == false)
			return;
		if (number > 0)
		{
			UINumberText.text = "X" + number;
			UINumberText.gameObject.SetActive (true);

			UIJewelCostObj.SetActive (false);
			UICoinCostObj.SetActive(false);
			if (SkillType == PlayerData.ItemType.ProtectShield) {
				ProtectShieldParticle.Stop ();
				ProtectShieldParticle.gameObject.SetActive (false);
			}
		} 
		else
		{
			if(SkillType == PlayerData.ItemType.ProtectShield && PlatformSetting.Instance.PayVersionType != PayVersionItemType.GuangDian)
			{
				UINumberText.text = "X" + number;
				UINumberText.gameObject.SetActive (true);

				UIJewelCostObj.SetActive (false);
				UICoinCostObj.SetActive(false);
				if (!ProtectShieldParticle.gameObject.activeSelf)
					ProtectShieldParticle.gameObject.SetActive (true);
				ProtectShieldParticle.Play ();
			}
			else
			{
				if(BuySkillData.Instance.GetBuyType((int)SkillType).CompareTo("Jewel") == 0)
				{
					UIJewelCostObj.SetActive (true);
					UICoinCostObj.SetActive(false);
					UIJewelCostText.text = BuySkillData.Instance.GetCost((int)SkillType);
				}
				else
				{
					UIJewelCostObj.SetActive (false);
					UICoinCostObj.SetActive(true);
					UICoinCostText.text = BuySkillData.Instance.GetCost((int)SkillType);
				}
				UINumberText.gameObject.SetActive (false);
			}


		}
	}

	public void HideClippedEffect()
	{
		StopCoroutine ("HideClippedSprite");
		ClippedSprite.ClipRect = new Rect (0, -1, 1, 1);
		gameObject.GetComponent<tk2dUITweenItem> ().enabled = true;
		coolFlag = false;
		ClippedSprite.gameObject.SetActive (false);
	}
	
	public void ShowClippedEffect()
	{
        if (!this.gameObject.activeInHierarchy)
            return;
		coolTime = skillTime;
		gameObject.GetComponent<tk2dUITweenItem> ().enabled = false;
		coolFlag = true;
		ClippedSprite.gameObject.SetActive (true);
		ClippedSprite.ClipRect = new Rect (0, 0, 1, 1);
        StartCoroutine ("HideClippedSprite");
	}

	IEnumerator HideClippedSprite()
	{
		while(coolTime > 0)
		{
			while(GameData.Instance.IsPause)
				yield return 0;
			
			coolTime -= Time.deltaTime;
			ClippedSprite.ClipRect = new Rect(0, coolTime / skillTime - 1, 1, 1);
			yield return 0;
		}
		gameObject.GetComponent<tk2dUITweenItem> ().enabled = true;
		coolFlag = false;
		ClippedSprite.gameObject.SetActive (false);
	}
}
