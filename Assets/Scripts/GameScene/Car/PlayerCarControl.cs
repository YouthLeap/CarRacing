using UnityEngine;
using System.Collections;

/// <summary>
/// 主角赛车
/// </summary>
[RequireComponent(typeof(CarMove))]
[RequireComponent(typeof(PropControl))]
[RequireComponent(typeof(CarParticleControl))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerCarControl : MonoBehaviour {

	public static PlayerCarControl Instance=null;
	public CarMove carMove;
	public PropControl propCon;
	public CarParticleControl carParticleCon;
	public PropType testProp;

	
	private Vector3 prePos=Vector3.zero;
	private Vector3 curPos=Vector3.zero;

	private int preCircleCount=1;

	void Awake()
	{
		Instance = this;
	}

	public void Init () {
		carMove = GetComponent<CarMove> ();
		propCon = GetComponent<PropControl> ();
		carParticleCon = GetComponent<CarParticleControl> ();

		propCon.Init ();
		carParticleCon.Init ();

		carMove.StartRun();

		preCircleCount=1;
	}

	void Update () {

#if UNITY_EDITOR

		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			GamePlayerUIControllor.Instance.RightDown();
		}
		else if(Input.GetKeyUp(KeyCode.RightArrow))
		{
			GamePlayerUIControllor.Instance.RightUp();
		}else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			GamePlayerUIControllor.Instance.LeftDown();
		}else if(Input.GetKeyUp(KeyCode.LeftArrow))
		{
			GamePlayerUIControllor.Instance.LeftUp();
		}

		if(Input.GetKey(KeyCode.UpArrow))
		{
			carMove.speed+=2f;
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			carMove.speed-=2f;
		}

		if(Input.GetKeyDown(KeyCode.A))
		{
			propCon.UseCurProp();
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			propCon.StartShapeShift (GameData.Instance.ShapeTime);
		}else if(Input.GetKeyDown(KeyCode.N))
		{
			propCon.ChangeToCar();
		}

		if(Input.GetKeyDown(KeyCode.T))
		{
			propCon.curHavePropId = (int)testProp;
			propCon.UseCurProp();
		}

		if(Input.GetKeyDown(KeyCode.S))
		{
			propCon.SlipHit();
		}

		if(Input.GetKeyDown(KeyCode.L))
		{
			propCon.LightingHit();
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			propCon.FireBallHit();
		}

		if(Input.GetKeyDown(KeyCode.W))
		{
			GameData.Instance.completeMissionIds.Add(1);
			GameData.Instance.RewardDic[PlayerData.ItemType.Apple]=3;
			GameData.Instance.RewardDic[PlayerData.ItemType.Banana]=3;
			GameData.Instance.RewardDic[PlayerData.ItemType.Ganoderma]=3;
			GameController.Instance.FinishGame();

			for(int i=0;i<CarManager.Instance.carMoveList.Count;++i)
			{
				CarManager.Instance.carMoveList[i].isFinish=true;
			}

		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			propCon.UsePropByType (PropType.Missile);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			propCon.UsePropByType (PropType.FlyBmob);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			propCon.UsePropByType (PropType.RedTurtleShell);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			propCon.UsePropByType (PropType.GreenTurtleShell);
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			propCon.UsePropByType (PropType.CuttleFish);
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			propCon.UsePropByType (PropType.Mine);
		}
		if (Input.GetKeyDown (KeyCode.Alpha7)) {
			propCon.UsePropByType (PropType.BananaPeel);
		}
		if (Input.GetKeyDown (KeyCode.Alpha8)) {
			propCon.UsePropByType (PropType.FireBall);
		}
		if (Input.GetKeyDown (KeyCode.Alpha9)) {
			propCon.UsePropByType (PropType.Mushroom);
		}
		if (Input.GetKeyDown (KeyCode.Alpha0)) {
			propCon.UsePropByType (PropType.Gloves);
		}
		if (Input.GetKeyDown (KeyCode.F1)) {
			propCon.UsePropByType (PropType.Lighting);
		}

		if (Input.GetKeyDown (KeyCode.Z)) {
			propCon.MissileHit ();
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			propCon.TurtleShellHit ();
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			propCon.CuttleFishHit ();
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			propCon.MineHit ();
		}
        if (Input.GetKeyDown (KeyCode.Q)) {
            PlayerCarControl.Instance.propCon.UsePropByType(PropType.SpeedUp);
        }

#endif

	}


    void FixedUpdate()
	{
		if(carMove.circleCount>preCircleCount)
		{
			preCircleCount=carMove.circleCount;

			if(PlayerData.Instance.IsWuJinGameMode())
			{
				//DropItemManage.Instance.ShowCircleCount(preCircleCount);
			}
			else if(GameLevelModel.Weedout == CarManager.Instance.gameLevelModel)
			{
				DropItemManage.Instance.ShowCircleCount(preCircleCount);
			}else
			{
				if(preCircleCount<=CarManager.Instance.circleCount)
				{
					DropItemManage.Instance.ShowCircleCount(preCircleCount);
				}

				if(preCircleCount == CarManager.Instance.circleCount -1)
				{
					AudioManger.Instance.PlaySound(AudioManger.SoundName.LastLap);
				}
			}
		}
	}

	/// <summary>
	/// Gets the path percent.
	/// </summary>
	/// <returns>The path percent.</returns>
	public float  GetPathPercent()
	{
		if(CarManager.Instance.gameLevelModel == GameLevelModel.Weedout)
		{
			return -1f;
		}
		float moveLen = carMove.moveLen;
		float totalLen = CarManager.Instance.totalPathLen;
		float pathLen = RoadPathManager.Instance.pathLen;

		if(CarManager.Instance.gameLevelModel == GameLevelModel.WuJing)
		{
			float passLen = pathLen*(carMove.circleCount-1);
			return (moveLen-passLen) / (totalLen-passLen);
		}

		return moveLen / totalLen;
	}
}
