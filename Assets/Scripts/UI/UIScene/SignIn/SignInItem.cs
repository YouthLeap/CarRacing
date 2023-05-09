using UnityEngine;
using System.Collections;

public class SignInItem : MonoBehaviour {

	public EasyFontTextMesh TitleText;
	public tk2dSprite[] IconImages;
	public EasyFontTextMesh[] CountTexts;
	public GameObject GetedImage;
	public ParticleSystem ParticleEffect0, ParticleEffect1, ParticleEffect2;

	public void Init(int index)
	{
		//Debug.Log ("signIn index " + index);
		//TitleText.text = SignInData.Instance.GetTitle (index);
		string[] rewardIdArr = SignInData.Instance.GetRewardIdArr (index);
		int[] rewardCountArr = SignInData.Instance.GetRewardCountArr (index);
		int[] modelState = PlayerData.Instance.GetModelState();

		for(int i = 0; i < rewardIdArr.Length; i++)
		{
			//检测是否奖励其他的超人
			int[] rewardMoreIds = ConvertTool.StringToAnyTypeArray<int> (rewardIdArr[i], '^');

			if (rewardMoreIds.Length == 1) { //没奖励角色
				IconImages[i].SetSprite(RewardData.Instance.GetIconName (rewardMoreIds [0]));
				CountTexts[i].text = "+" + rewardCountArr [i];
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
					IconImages[i].SetSprite (ModelData.Instance.GetPlayerIcon(rewardMoreIds [0]));
					CountTexts[i].text = "+1";
				}
				else
				{
					IconImages[i].SetSprite (RewardData.Instance.GetIconName (rewardMoreIds [1]));
					CountTexts[i].text = "+" + rewardCountArr [i];
				}

			}
		}

	}

	public void GetedReward(bool flag)
	{
		GetedImage.SetActive (flag);
	}

	public void PlayParticleEffect(int index)
	{
		ParticleEffect0.gameObject.SetActive (true);
		ParticleEffect0.Play ();
		if (index == 7) {
			ParticleEffect1.gameObject.SetActive (true);
			ParticleEffect1.Play ();
			ParticleEffect2.gameObject.SetActive (true);
			ParticleEffect2.Play ();
		}
	}
}
