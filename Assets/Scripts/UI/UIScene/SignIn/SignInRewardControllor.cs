using UnityEngine;
using System.Collections;

public class SignInRewardControllor : UIBoxBase {

	public static SignInRewardControllor Instance;

	public GameObject[] Contents;
	private tk2dSprite[] iconImages;
	private EasyFontTextMesh[] countTexts;
	public EasyFontTextMesh textRewardHint;

	private string[] rewardIdArr;
	private int[] rewardCountArr;
	private int[] modelState;

	private int index;
	private bool hasModel;

	public override void Init ()
	{
		Instance = this;

		iconImages = new tk2dSprite[Contents.Length];
		countTexts = new EasyFontTextMesh[Contents.Length];
		for(int i = 0; i < Contents.Length; i ++)
		{
			iconImages[i] = Contents[i].transform.Find("IconImage").GetComponent<tk2dSprite>();
			countTexts[i] = Contents[i].transform.Find("CountText").GetComponent<EasyFontTextMesh>();

			Contents [i].SetActive (false);
		}

		base.Init();
	}

	public void InitData(int index)
	{
		this.index = index;

		rewardIdArr = SignInData.Instance.GetRewardIdArr(index);
		rewardCountArr = SignInData.Instance.GetRewardCountArr(index);
		modelState = PlayerData.Instance.GetModelState();

		string showText = "Congratulations："; // 恭喜获得

        for (int i = 0; i < rewardIdArr.Length; i++)
		{
			Contents [i].SetActive (true);

			//检测是否奖励其他的超人
			int[] rewardMoreIds = ConvertTool.StringToAnyTypeArray<int> (rewardIdArr[i], '^');

			if (rewardMoreIds.Length == 1) { //没奖励角色
				iconImages[i].SetSprite(RewardData.Instance.GetIconName (rewardMoreIds [0]));
				countTexts[i].text = "+" + rewardCountArr [i];
				//文字描述
				showText += RewardData.Instance.GetName (rewardMoreIds [0]) + "x" + rewardCountArr [i];
				//道具下发
				PlayerData.Instance.AddItemNum(RewardData.Instance.GetItemType(rewardMoreIds [0]), rewardCountArr [i]);
					
			} else { //奖励角色
				//判断角色是否拥有
				bool hasModel = false;
				for(int j = 0; j < modelState.Length; ++j)
				{
					if(IDTool.GetModelType(modelState[j]) == IDTool.GetModelType(rewardMoreIds [0]))
					{
						hasModel = true;
						break;
					}
				}

				if(!hasModel)
				{
					iconImages[i].SetSprite (ModelData.Instance.GetPlayerIcon(rewardMoreIds [0]));
					countTexts[i].text = "+1";
					//文字描述
					showText += ModelData.Instance.GetName(rewardMoreIds [0]) + "x1";
					//道具下发
					PlayerData.Instance.UpdateModelState(rewardMoreIds [0]);
				}
				else
				{
					iconImages[i].SetSprite (RewardData.Instance.GetIconName (rewardMoreIds [1]));
					countTexts[i].text = "+" + rewardCountArr [i];
					//文字描述
					showText += RewardData.Instance.GetName (rewardMoreIds [1]) + "x" + rewardCountArr [i];
					//道具下发
					PlayerData.Instance.AddItemNum(RewardData.Instance.GetItemType(rewardMoreIds [1]), rewardCountArr [i]);
				}

			}

			if (i != rewardIdArr.Length - 1) {
				showText += "、";
			}
		}

		textRewardHint.text = showText;
	}

	public override void Show ()
	{
		base.Show();
	}

	public override void Hide ()
	{
		gameObject.SetActive (false);
		UIManager.Instance.HideModule (UISceneModuleType.SignInReward);
	}

	public override void Back ()
	{
	}

	public void GetOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
		SignInControllor.Instance.Hide();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
		SignInControllor.Instance.SetSignIn();

//		//set and show
//		SignInNextDayRewardControllor.Instance.InitData ( (this.index+1)%7);
//		UIManager.Instance.ShowModule (UISceneModuleType.SignInNextDayReward);
	}
}
