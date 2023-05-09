using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


/// <summary>
/// 游戏场景中的数值
/// </summary>
public class GameData :MonoBehaviour {


	public static GameData Instance=null;

	void Awake()
	{
		Instance = this;
	}

	#region 基本数值
	public delegate void DataChangeHandle<T>(T curNum);
	public event DataChangeHandle<int> CoinChangeEvnet;
	public event DataChangeHandle<bool> IsPauseChangeEvent;

	//mission
	public event DataChangeHandle<int> CrashChangeEvent;
	public event DataChangeHandle<int> ItemUseChangeEvent;
	public event DataChangeHandle<int> BeHitChangeEvent;
	public event DataChangeHandle<int> SpeedUpChangeEvent;
	public event DataChangeHandle<int> CrashCarChangeEvent;
	public event DataChangeHandle<int> RankChangeEvent;

   //mission
	int _crashCount=0;
	int _itemUseCount=0;
	int _beHitCount=0;
	int _speedUpCount=0;
	int _crashCarCount=0;
	int _rank=1;
    int _curCoin = 0;
	public int StarCount=0;
	public int curMulti = 1;

	public int curCoin {
		get{return _curCoin;}
		set{
			_curCoin = value;
			if(CoinChangeEvnet != null)
			{
				CoinChangeEvnet(_curCoin);
			}
		}
	}

	public int crashCount
	{
		get{return _crashCount;}
		set{
			_crashCount = value;
			if(CrashChangeEvent!=null)
			{
				CrashChangeEvent(_crashCount);
			}
		}
	}

	public int itemUseCount{
		get{return _itemUseCount;}
		set{
			_itemUseCount= value;
			if(ItemUseChangeEvent!=null)
			{
				ItemUseChangeEvent(_itemUseCount);
			}
		}
	}

	public int beHitCount{
		get{return _beHitCount;}
		set{
			_beHitCount = value;
			if(BeHitChangeEvent!=null)
			{
				BeHitChangeEvent(_beHitCount);
			}
		}
	}

	public int speeedUpCount
	{
		get{return _speedUpCount;}
		set{
			_speedUpCount = value;
			if(SpeedUpChangeEvent!=null)
			{
				SpeedUpChangeEvent(_speedUpCount);
			}
		}
	}

	public int crashCarCount{
		get{return _crashCarCount;}
		set{
			_crashCarCount= value;
			if(CrashCarChangeEvent!=null)
			{
				CrashCarChangeEvent(_crashCarCount);
			}
		}
	}

	public int rank{
		get{return _rank;}
		set{

			if(IsWin)
				return;

			_rank = value;
			if(RankChangeEvent!=null)
			{
				RankChangeEvent(_rank);
			}
		}
	}
	

	/**选择的超人信息*/
	public int selectedModelId = 0, selectedModelLevel = 0, selectedModelType = 0;


	//记录收集到的材料
	public Dictionary<PlayerData.ItemType,int> RewardDic = new Dictionary<PlayerData.ItemType, int>();
	
	/*成就数量*/
	public event DataChangeHandle<int> KillCountChangeEvent;

	//完成的任务id
	public List<int> completeMissionIds = new List<int>();
	
	#endregion

	public bool IsWin=false;



	/// <summary>
	/// 控制计算时间 是否暂停
	/// </summary>

	private bool _isPause = true;
	public bool IsPause
	{
		get{
			return _isPause;
		}
		
		set{
			_isPause = value;
			
			if(IsPauseChangeEvent != null)
			{
				IsPauseChangeEvent(_isPause);
			}
		}
	}

	/// <summary>
	/// 把所有委托事件都置空
	/// </summary>
	void OnDisable()
	{
		CoinChangeEvnet = null;

		KillCountChangeEvent = null;
		IsPauseChangeEvent = null;
	}
	
	public  void Init ()
	{
		selectedModelId = PlayerData.Instance.GetSelectedModel ();
		selectedModelLevel = IDTool.GetModelLevel (selectedModelId);
		selectedModelType = IDTool.GetModelType (selectedModelId);

		curMulti = 1;
		curMulti += PlayerData.Instance.GetForeverDoubleCoin ();

		InitDropProps ();
		InitPlayerSelfSkill ();
	}

	void InitDropProps()
	{
		RewardDic [PlayerData.ItemType.Apple] = 0;
		RewardDic [PlayerData.ItemType.Banana] = 0;
		RewardDic [PlayerData.ItemType.Ganoderma] = 0;
		RewardDic [PlayerData.ItemType.Ginseng] = 0;
		RewardDic [PlayerData.ItemType.Pear] = 0;
		RewardDic [PlayerData.ItemType.Grape] = 0;
		int curLevel = PlayerData.Instance.GetSelectedLevel ();
		int[] dropIdArr = LevelData.Instance.GetDropItemRewardIdList (curLevel);
		int[] dropArrCount = LevelData.Instance.GetDropItemRewardIdListCount (curLevel);
		for(int i = 0 ; i < dropIdArr.Length; i++)
		{
			RewardDic [RewardData.Instance.GetItemType(dropIdArr[i])] = Random.Range(1,dropArrCount[i]) ;
		}
	}
	
	
	/// <summary>
	/// 角色本身的被动技能
	/// </summary>
	[HideInInspector]public float SpeedUpTime;
	[HideInInspector]public float SpeedAddTime;
	[HideInInspector]public float AddCoinPecent;
	[HideInInspector]public float ShieldTime;
	[HideInInspector]public float ShapeTime;
	[HideInInspector]public float MissileVertigoTime;

	[HideInInspector]public float MushroomVertigoTime;
	[HideInInspector]public float GlovesVertigoTime;
	[HideInInspector]public float LightingVertigoTime;
	[HideInInspector]public float SpeedUpLightingVertigoTime;
	[HideInInspector]public float TurtleShellVertigoTime;
	[HideInInspector]public float MineVertigoTime;

	[HideInInspector]public float MushroomTime;
	[HideInInspector]public float FireBallTime;
	[HideInInspector]public float GloveTime;
	[HideInInspector]public float CuttleFishTime;
	void InitPlayerSelfSkill()
	{
		SpeedUpTime = PropConfigData.Instance.GetSkillTime ((int)PropType.SpeedUp);
		SpeedAddTime = PropConfigData.Instance.GetSkillTime ((int)PropType.SpeedAdd);
		AddCoinPecent = 1.0f;
		ShieldTime = PropConfigData.Instance.GetSkillTime ((int)PropType.Shield);
		ShapeTime = ModelData.Instance.GetShapeTime (selectedModelId);
		MissileVertigoTime = PropConfigData.Instance.GetVertigoTime ((int)PropType.Missile);

		MushroomVertigoTime = PropConfigData.Instance.GetVertigoTime ((int)PropType.Mushroom);
		GlovesVertigoTime = PropConfigData.Instance.GetVertigoTime ((int)PropType.Gloves);
		LightingVertigoTime = PropConfigData.Instance.GetVertigoTime ((int)PropType.Lighting);
		SpeedUpLightingVertigoTime = PropConfigData.Instance.GetVertigoTime ((int)PropType.SpeedUp);
		TurtleShellVertigoTime = PropConfigData.Instance.GetVertigoTime ((int)PropType.GreenTurtleShell);
		MineVertigoTime = PropConfigData.Instance.GetVertigoTime ((int)PropType.Mine);

		MushroomTime = PropConfigData.Instance.GetSkillTime ((int)PropType.Mushroom);
		FireBallTime = PropConfigData.Instance.GetSkillTime ((int)PropType.FireBall);
		GloveTime = PropConfigData.Instance.GetSkillTime ((int)PropType.Gloves);
		CuttleFishTime = PropConfigData.Instance.GetSkillTime ((int)PropType.CuttleFish);

		if (selectedModelLevel < ModelData.Instance.GetSkillLevel (selectedModelId))
			return;
		
		switch(selectedModelType)
		{
		case 1:
			SpeedUpTime = SpeedUpTime * (1 + 0.1f);
			break;
		case 2:
			AddCoinPecent = AddCoinPecent * (1 + 0.1f);
			break;
		case 3:
			ShieldTime += 1;
			break;
		case 4:
			ShapeTime = ShapeTime * (1 + 0.2f);
			break;
		case 5:
			MissileVertigoTime += 1;
			break;
		}
	}
	
	void InitAchiveData()
	{
		/*成就数量*/
	
		
//		costShield = 0;
//		costMagnet = 0;
//
//		_crashFlyCarContinue = 0;
//		_curExperience = 0;
//		_curCoin = 0;
//		_StarCount = 0;
//		_oilTime=0;
//		_power = 0;
//
//		_curPlayerSkillCoolTime = 0;
//
//
//		//关卡任务
//		_overtakeCount = 0;
//		_crashCarCount = 0;
//		_crashFlyCarCount = 0;
//		_crashFlyCarContinue = 0;
//
//		_jumpMission = 0;
//		_overtakeContinueMission = 0;
//		_destroyPoliceMission = 0;
//		_beHitByBossMission = 0;
//		_killBossTime=0;
//		_powerStationMission = 0;
//		
//
//		skillUseShield = 0;
//		skillMultiCoin = 0;0
		
	}

	#region  活动彩蛋
	public int curEggCount=0;

	#endregion

}
