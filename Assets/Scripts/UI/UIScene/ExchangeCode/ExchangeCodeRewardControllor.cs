using UnityEngine;
using System.Collections;

public class ExchangeCodeRewardControllor : UIBoxBase {
	
	public static ExchangeCodeRewardControllor Instance;
	
	public Transform ItemContainer;
	public EasyFontTextMesh textRewardHint;
	
	private int[] rewardIdArr;
	private int[] rewardCountArr;
	
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public void InitData(string rewardContent)
	{
		string[] rewardInfos = ConvertTool.StringToAnyTypeArray<string> (rewardContent, '|');
		int rewardCount = rewardInfos.Length;

		rewardIdArr = new int[rewardCount];
		rewardCountArr = new int[rewardCount];
		Transform itemTran;
		string rewardHintStr = "恭喜获得：";
		for (int i=1; i<=rewardCount; ++i) {
			rewardIdArr[i-1] = int.Parse(ConvertTool.StringToAnyTypeArray<string>(rewardInfos[i-1], '*')[0]);
			rewardCountArr[i-1] = int.Parse(ConvertTool.StringToAnyTypeArray<string>(rewardInfos[i-1], '*')[1]);

			itemTran = ItemContainer.Find("Content" + i);
			itemTran.gameObject.SetActive(true);
			itemTran.localPosition = new Vector3(- 150 * (rewardCount - 1) / 2 + 150 * (i - 1),0,0);
			tk2dSprite iconImage = itemTran.Find("IconImage").GetComponent<tk2dSprite>();
			EasyFontTextMesh countText = itemTran.Find("CountText").GetComponent<EasyFontTextMesh>();
			PlayerData.ItemType itemType = RewardData.Instance.GetItemType(rewardIdArr[i-1]);
			if(itemType == PlayerData.ItemType.Coin)
				iconImage.SetSprite ("coin_2");
			else
				iconImage.SetSprite (RewardData.Instance.GetIconName(rewardIdArr[i-1]));
			countText.text = "x" + rewardCountArr[i-1];
			
			if(i < rewardCount)
				rewardHintStr += RewardData.Instance.GetName(rewardIdArr[i-1]) + "x" + rewardCountArr[i-1] + "、";
			else
				rewardHintStr += RewardData.Instance.GetName(rewardIdArr[i-1]) + "x" + rewardCountArr[i-1];
		}
		textRewardHint.text = rewardHintStr;
		for (int i=rewardCount+1; i<=3; ++i) {
			itemTran = ItemContainer.Find("Content" + i);
			itemTran.gameObject.SetActive(false);
		}
	}
	
	public override void Show ()
	{
		base.Show();
	}
	
	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule (UISceneModuleType.ExchangeCodeReward);
	}
	
	public override void Back ()
	{
	}
	
	public void GetOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
		Hide();
		for(int i = 0; i < rewardIdArr.Length; i++)
		{
			PlayerData.Instance.AddItemNum(RewardData.Instance.GetItemType(rewardIdArr[i]), rewardCountArr[i]);
		}
	}
}
