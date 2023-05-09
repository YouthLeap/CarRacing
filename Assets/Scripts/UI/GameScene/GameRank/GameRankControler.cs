using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
///游戏排名界面
/// </summary>
public class GameRankControler : UIBoxBase {

	public static GameRankControler Instance;

	public List<GameRankItem> itemList = new List<GameRankItem>();

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public override void Show ()
	{
		GamePlayerUIControllor.Instance.gameObject.SetActive(false);

		base.Show();
		transform.localPosition = ShowPosV2;
		InitRankData();
		Invoke("Hide",5f);
	}
	
	public override void Hide ()
	{
		gameObject.SetActive (false);
		transform.localPosition = GlobalConst.LeftHidePos;
		GameUIManager.Instance.HideModule(UISceneModuleType.GameRank);

		GameController.Instance.ShowEndGame();
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide ();
	}
	#endregion



	public void InitRankData()
	{
		List<CarMove> carMoveList = CarManager.Instance.GetCarMoveSortList();
		carMoveList.Reverse();

		CarMove playerCarMove = PlayerCarControl.Instance.carMove;

		if(CarManager.Instance.gameLevelModel== GameLevelModel.Weedout)
		{
			for(int k=0;k<carMoveList.Count;++k)
			{
				if(carMoveList[k].moveLen>=playerCarMove.moveLen)
				{
					carMoveList[k].isFinish=true;
					carMoveList[k].runningTime= CarManager.Instance.playerUseTime;
				}else
				{
					carMoveList[k].isFinish=false;
				}
			}
		}else
		{
			carMoveList.Sort(CompareList);
		}

		for(int i=0;i<carMoveList.Count;++i)
		{
			int index=i+1;
			int carId = carMoveList[i].carId;
			float time= carMoveList[i].runningTime;
			if(carMoveList[i].isFinish==false)
			{
				time =-1f;
			}
			itemList[i].SetData(index,carId,time,carMoveList[i].isPlayer);

			Vector3 pos = itemList[i].transform.localPosition;
			pos.x=600f;
			itemList[i].transform.localPosition=pos;
			itemList[i].transform.DOLocalMoveX(0,0.3f).SetEase(Ease.OutBack).SetDelay(i*0.15f);

		}

		for(int j=carMoveList.Count;j<itemList.Count;++j)
		{
			itemList[j].gameObject.SetActive(false);
		}
	}

	private int CompareList(CarMove x, CarMove y){
		return x.runningTime.CompareTo (y.runningTime);
	}
}
