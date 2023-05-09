using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManger : MonoBehaviour
{
	 
    public static GameObject AudioPool;

    // 要播放的音阶编号
    private int coinSoundNum = 0;
    private float currentTime = 0f;
    private int powerSoundNum = 0;
    private float currentPowerTime = 0f;
    // 过了这个时间没有吃到金币就重新播放音阶
    private float repeatSoundTimeGap = 0.5f;
    // 音阶的总数量,每次更换音阶的时候总数量要改！
    private int coinSoundQuantity = 14;
    private int powerSoundQuantity = 6;
    //同一时间只播放一次
    static List<string> audioOnceSameTime = new List<string>();
    //奥特曼声音,用于小动作的优先级
    static List<string> aotemanSound = new List<string>();

    private static AudioManger instance = null;
    public static AudioManger Instance
    {
        get
        {
            if (instance == null)
            {
                AudioPool = (GameObject)Instantiate(Resources.Load("Audio/AudioContainer"));
                AudioPool.name = "AudioContainer";
				AudioPool.SetActive(true);
                DontDestroyOnLoad(AudioPool);
                instance = AudioPool.AddComponent<AudioManger>();

//                audioOnceSameTime.Add("Yell");
//                audioOnceSameTime.Add("KongLong");
//                audioOnceSameTime.Add("CarHorn");
//                audioOnceSameTime.Add("LittleAction");
//                audioOnceSameTime.Add("UpdateUI");
//                audioOnceSameTime.Add("StartGame");
//                audioOnceSameTime.Add("AotemanSayOne");
//                audioOnceSameTime.Add("AotemanSayTwo");
//                audioOnceSameTime.Add("AotemanSayThree");
//                audioOnceSameTime.Add("AotemanSayFour");
//                audioOnceSameTime.Add("AotemanSayFive");
//                audioOnceSameTime.Add("AotemanSaySeven");

                //奥特曼声音,用于小动作的优先级
                for (int i = 1; i <= 5; i++)
                {
//                    aotemanSound.Add("Big" + GetAoteManName(1));
//                    aotemanSound.Add("Show" + GetAoteManName(1));
//                    aotemanSound.Add("Skill" + GetAoteManName(1));
                }
//                aotemanSound.Add("Fly");
//                aotemanSound.Add("JumpBoy");
//                aotemanSound.Add("JumpGirl");
            }
            return instance;
        }
    }

	#region  音量
	public float MusicVolume
	{
		get
		{
			return AudioController.GetCategory("Music").Volume;
		}

		set
		{
			AudioController.GetCategory("Music").Volume = value;

			PlayerData.Instance.SetMusicVolume(value);
		}
	}

	public float SoundVolume
	{
		get
		{
			return AudioController.GetCategory("Sound").Volume;
		}
		
		set
		{
			AudioController.GetCategory("Sound").Volume = value;

			PlayerData.Instance.SetSoundVolume(value);
		}
	}
	#endregion

    #region 音效控制（内部方法）

    private int MaxSoundNum = 3;//同一种音效，同时播放的最大次数

    /// <summary>
    /// 根据audioId，获取此音效正在被播放的次数
    /// </summary>
    /// <returns>The playing audio number.</returns>
    /// <param name="audioId">Audio identifier.</param>
    private int GetPlayingAudioNumber(string audioId)
    {
        return AudioController.GetPlayingAudioObjectsCount(audioId, false);
    }

     


    /// <summary>
    /// 根据audioId播放音效
    /// </summary>
    /// <param name="audioId">Audio identifier.</param>
    private void PlaySound(string audioId,float volumn=1,float delay=0)
    {
        
        if (!PlayerData.Instance.GetSoundState())
            return;
        //同一种音效，同时只能播放 MaxSoundNum 次
        if (GetPlayingAudioNumber(audioId) >= MaxSoundNum && audioId.CompareTo("Turnplate") != 0 && audioId.CompareTo("Get") != 0)
        {
            return;
        }
        if (audioOnceSameTime.Contains(audioId) && GetPlayingAudioNumber(audioId) >= 1)
        {
            return;
        }

        //小动作声音放到最后面
//        if (AudioController.IsPlaying(SoundName.LittleAction + GetAoteManName()))
//        {
//            Debug.LogWarning("playing");
//            Debug.LogWarning(audioId);
//            if (aotemanSound.Contains(audioId))
//                StopSound(SoundName.LittleAction + GetAoteManName());
//        }

        AudioObject ao = AudioController.Play(audioId, AudioPool.transform, volumn);

//        if (audioId.StartsWith("Skill"))
//            ao.transform.position = new Vector3(0,0,-7);
    }

    public static string GetAoteManName()
    {
        string s = null;
        switch (PlayerData.Instance.GetSelectedModel() / 100)
        {
            case 1:
                s = "WuKong";
                break;
            case 2:
                s = "BaJie";
                break;
            case 3:
                s = "BaiGu";
                break;
        }
        return s;
    }

    public static string GetAoteManName(int index)
    {
        string s = null;
        switch (index)
        {
		case 1:
			s = "WuKong";
			break;
		case 2:
			s = "BaJie";
			break;
		case 3:
			s = "BaiGu";
			break;
        }
        return s;
    }



    /// <summary>
    /// 根据audioId停止音效
    /// </summary>
    /// <param name="audioId">Audio identifier.</param>
    private void StopSound(string audioId)
    {
        if (!PlayerData.Instance.GetSoundState())
            return;
        

        AudioController.Stop(audioId);
    }
    #endregion

    #region  音乐控制（内部方法）
    /// <summary>
    /// 根据audioId播放音乐
    /// </summary>
    /// <param name="audioId">Audio identifier.</param>
    private void PlayMusic(string audioId)
    {
        if (!PlayerData.Instance.GetMusicState())
            return;

        AudioController.PlayMusic(audioId, AudioPool.transform);
    }
    #endregion

    #region  音乐、音效的暂停、停止控制（外部方法）
    /// <summary>
    /// 暂停所有音效
    /// </summary>
    public void PauseSound()
    {
//        if (!PlayerData.Instance.GetSoundState())
//            return;
        AudioController.PauseCategory("Sound");
    }

    /// <summary>
    /// 恢复所有音效
    /// </summary>
    public void UnPauseSound()
    {
//        if (!PlayerData.Instance.GetSoundState())
//            return;
        AudioController.UnpauseCategory("Sound");
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    /// <param name="audioId">Audio identifier.</param>
    public void StopMusic()
    {
//        if (!PlayerData.Instance.GetMusicState())
//            return;
        AudioController.StopMusic();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseMusic()
    {
//        if (!PlayerData.Instance.GetMusicState())
//            return;
        AudioController.PauseMusic();
    }

    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void UnPauseMusic()
    {
//        if (!PlayerData.Instance.GetMusicState())
//            return;
        AudioController.UnpauseMusic();
    }

    /// <summary>
    /// 停止所有音效和音乐
    /// </summary>
    public void StopAll()
    {
        AudioController.StopAll();
    }

    /// <summary>
    /// 暂停所有音效和音乐
    /// </summary>
    public void PauseAll()
    {
        AudioController.PauseAll();
    }

    /// <summary>
    /// 恢复所有音效和音乐
    /// </summary>
    public void UnPauseAll()
    {
        AudioController.UnpauseAll();
    }
    #endregion

    #region  外部引用播放方法
    /// <summary>
    /// 根据音乐名字播放音乐
    /// </summary>
    /// <param name="musicName">Music name.</param>
    public void PlayMusic(MusicName musicName)
    {
        switch (musicName)
        {

            default:
                PlayMusic(musicName.ToString());
                break;
        }
    }

    


    /// <summary>
    /// 根据音效名字播放音效.
    /// </summary>
    /// <param name="soundName">Sound name.</param>
    public void PlaySoundByName(string soundName)
    {                
        PlaySound((SoundName)System.Enum.Parse(typeof(SoundName), soundName));
    }

    /// <summary>
    /// 根据音效名字停止播放音效
    /// </summary>
    /// <param name="soundName">Sound name.</param>
    public void StopSoundByName(string soundName)
    {        
        StopSound(soundName);
    }

    /// <summary>
    /// 根据音效名字播放音效
    /// </summary>
    /// <param name="soundName">Sound name.</param>
    public void PlaySound(SoundName soundName)
    {
        // if (ValidSound(soundName) == false) { return; }

        switch (soundName)
        {
		case SoundName.Get:
			if (currentTime == 0f)
			{
				PlaySound("Get" + coinSoundNum);
				currentTime = Time.time;
				coinSoundNum++;
			}
			else
			{
				if (Time.time - currentTime < repeatSoundTimeGap)
				{
					
					if ((Time.time - currentTime) == 0f)
					{
						//Debug.Log("return");
						return;
					}
					else
					{
						PlaySound("Get" + coinSoundNum);
						// 当音阶编号小于音阶总数的时候，音阶＋1，每次更换音阶的时候总数量要改！
						if (coinSoundNum < coinSoundQuantity)
						{
							coinSoundNum++;
						}
						else
						{
							//Debug.Log(coinSoundNum);
						}
						currentTime = Time.time;
					}
				}
				else
				{
					currentTime = 0f;
					coinSoundNum = 0;
					PlaySound("Get" + coinSoundNum);
					coinSoundNum++;
				}
			}
			break;
		case SoundName.GetPower:
			if (currentPowerTime == 0f)
			{
				PlaySound("GetPower" + powerSoundNum);
				currentPowerTime = Time.time;
				powerSoundNum++;
			}
			else
			{
				if (Time.time - currentPowerTime < repeatSoundTimeGap)
				{
					
					if ((Time.time - currentPowerTime) == 0f)
					{
						//Debug.Log("return");
						return;
					}
					else
					{
						PlaySound("GetPower" + powerSoundNum);
						// 当音阶编号小于音阶总数的时候，音阶＋1，每次更换音阶的时候总数量要改！
						if (powerSoundNum < powerSoundQuantity - 1)
						{
							powerSoundNum++;
						}
						else
						{
							//Debug.Log(coinSoundNum);
						}
						currentPowerTime = Time.time;
					}
				}
				else
				{
					currentPowerTime = 0f;
					powerSoundNum = 0;
					PlaySound("GetPower" + powerSoundNum);
					powerSoundNum++;
				}
			}
			break;
//		case SoundName.Win:
//			PlaySound("WinMusic");
//			PlaySound("WinCheer");
//			break;
			////            case SoundName.Die:
			////                if (PlayerData.Instance.GetSelectedModel() / 100 == 3)
			////                {
			////                    PlaySound("KamilaDie");
			////                    break;
			////                }
			////                PlaySound("Die");
			////                break;
		case SoundName.Fly:
			if (PlayerData.Instance.GetSelectedModel() / 100 == 5)
			{
				PlaySound("FlyFemale");
				break;
			}
			PlaySound("FlyMale");
			break;
		case SoundName.Exert:
			if (PlayerData.Instance.GetSelectedModel() / 100 == 5)
			{
				PlaySound("ExertFemale");
				break;
			}
			PlaySound("ExertMale");
			break;
		case SoundName.SecondJump:
			if (PlayerData.Instance.GetSelectedModel() / 100 == 5)
			{
				PlaySound("SecondJumpFemale");
				break;
			}
			PlaySound("SecondJumpMale");
			break;
			break;
			//            case SoundName.AotemanToBig:
			//                PlaySound("Big" + GetAoteManName());
			//                break;
			//            case SoundName.AotemanShow:
			//                PlaySound("Show" + GetAoteManName());
			//                break;
			//            case SoundName.MustKill:
			//                PlaySound("Skill" + GetAoteManName());
			//                break;
			//            case SoundName.LittleAction:
			//                PlaySound("LittleAction" + GetAoteManName());
			//                break;
			////            case SoundName.Jump:
			////                PlaySound("Jump");
			////                if (PlayerData.Instance.GetSelectedModel() / 100 == 3)
			////                {
			////                    break;
			////                }
			////                PlaySound("JumpCry");
			////                break;
			//            case SoundName.StartGame:
			//                //			int temp = Random.Range(0, 100) % 2;
			//                //		
			//                //			if(temp == 0)
			//                //			{
			//                PlaySound("StartGame");
			//                //			}else
			//                //			{
			//                //				PlaySound("StartGame2");
			//                //			}
			//                break;
			//            case SoundName.BattleHeigh:
			//                PlaySound(soundName.ToString());
			//                StartCoroutine(UnPauseMusicWhenBattleHeighEnd());
			//                break;
		default:
			PlaySound(soundName.ToString());
			break;
		}
	}
	IEnumerator UnPauseMusicWhenBattleHeighEnd()
	{
		//        while (AudioController.IsPlaying(SoundName.BattleHeigh.ToString()))
		yield return 0;
		UnPauseMusic();
	}
	
	
	/// <summary>
	/// 根据音效名字停止播放音效
	/// </summary>
	/// <param name="soundName">Sound name.</param>
	public void StopSound(SoundName soundName)
	{
		//        if (soundName == SoundName.Win)
		//        {
		//            StopSound("WinMusic");
		//            StopSound("WinCheer");
		//            return;
		//        }
		//        else if (soundName == SoundName.MustKill)
		//        {
		//            StopSound("Skill" + GetAoteManName());
		//            return;
		//        }
		//        else if (soundName == SoundName.AotemanModelBox)
		//        {
		//            StopSound("WuKong");
		//            StopSound("BaJie");
		//            StopSound("BaiGu");
		//            //StopSound("NaiKeSaiShi");
		//            //StopSound("SaiJia");
		//            return;
		//        }
		
		
		StopSound(soundName.ToString());
	}


	private bool isPlayDriftSound = false;
	/// <summary>
	/// 角色车的 引擎声音 
	/// 同一时间只会有一种声音
	/// </summary>
	/// <param name="soundName">Sound name.</param>
	public void PlayDriftSound(SoundName soundName)
	{
		if (isPlayDriftSound)
			return;
		isPlayDriftSound=true;
		PlaySound (soundName);
	}
	public void StopDriftSound(SoundName soundName)
	{
		isPlayDriftSound=false;
		StopSound(soundName);
	}

	#endregion
	
	#region  音效名字 SoundName
	
	public enum SoundName
	{
		ButtonClick,
		CloseBtClick,
		/// <summary>
		/// 初代展示
		/// </summary>
		AotemanChudai,
		/// <summary>
		/// 艾斯展示
		/// </summary>
		AotemanAisi,
		/// <summary>
		/// 赛文展示
		/// </summary>
		AotemanSaiwen,
		/// <summary>
		/// 泰罗展示
		/// </summary>
		AotemanTailuo,
		/// <summary>
		/// 奥特之母展示
		/// </summary>
		AotemanMother,
		/// <summary>
		/// 初代进入游戏
		/// </summary>
		AotemanChudaiShow,
		/// <summary>
		/// 艾斯进入游戏
		/// </summary>
		AotemanAisiShow,
		/// <summary>
		/// 赛文进入游戏
		/// </summary>
		AotemanSaiwenShow,
		/// <summary>
		/// 泰罗进入游戏
		/// </summary>
		AotemanTailuoShow,
		/// <summary>
		/// 奥特之母进入游戏
		/// </summary>
		AotemanMotherShow,
		/// <summary>
		/// 航母助战
		/// </summary>
		AotemanKuizeng,
		/// <summary>
		/// 奥特兄弟
		/// </summary>
		AotemanXiongdi,
		/// <summary>
		/// 使用血瓶
		/// </summary>
		AotemanXueping,
		/// <summary>
		/// 死亡
		/// </summary>
		AotemanDeath,
		/// <summary>
		/// 进入角色升级界面
		/// </summary>
		UpgredeUi,
		/// <summary>
		/// 进入机甲强化界面
		/// </summary>
		UpgradeJijia,
		/// <summary>
		/// 角色升至10级
		/// </summary>
		UpgredeMidLevel,
		/// <summary>
		/// 角色升至30级
		/// </summary>
		UpgredeMaxLevel,
		/// <summary>
		/// 兑换成功，签到领取，转盘获得
		/// </summary>
		CashMachine,
		/// <summary>
		/// 领取星级奖励，秘藏礼包
		/// </summary>
		CoinCollision,
		Lose,
		Star,
		WinCheer,
		Win,
		/// <summary>
		/// 银币
		/// </summary>
		Get,
		/// <summary>
		/// 小积分(小月亮)
		/// </summary>
		GetPower,
		/// <summary>
		/// 金币道具拾取
		/// </summary>
		GetCoin,
		/// <summary>
		/// 积分道具拾取
		/// </summary>
		GetScore,
		/// <summary>
		/// 碰撞音效
		/// </summary>
		Bang,
		Fly,
		/// <summary>
		/// 磁铁/飞鹰号全屏
		/// </summary>
		Magnet,
		/// <summary>
		/// 护盾/血瓶/爱心变幻
		/// </summary>
		Shield,
		/// <summary>
		/// 破盾
		/// </summary>
		ShieldBroken,
		/// <summary>
		/// 跳
		/// </summary>
		Jump,
		SecondJump,
		/// <summary>
		/// 普攻第三击、二段跳
		/// </summary>
		Exert,
		/// <summary>
		/// 进出虐怪空间的传送声
		/// </summary>
		Transmit,
		/// <summary>
		/// 完成关卡
		/// </summary>
		CompleteLevel,
		CountDown,

		//开始游戏
		StartGo,
		//npc 车爆炸
		Bomb,
		//变身机甲
		ChangeRobot,

		//最大速度
		Engine_High,
		//无敌冲刺
		Engine_Limiter,
		//加速
		Engine_Middle,
		//Null no sound
		Null,
		//
		Explosion,

		//时间不足10秒时
		Fuelout,

		//角色技能声音
		SkillKaixin,
		SkillTianxin,
		SkillCuxin,
		SkillHuaxin,
		SkillXiaoxin,

		//角色变身
		ChangeKaixin,
		ChangeTianxin,
		ChangeCuxin,
		ChangeHuaxin,
		ChangeXiaoxin,

		//宅博士
		IndoorDoctor,
		//砸地冲击波
		HitBomb,
		//BOSS警告
		Warming,
		//飞弹
		Shooting,
		//机甲跑步脚落地
		LeftFoot,
		RightFoot,
		FlyBoost,

		//拾取经验道具时
		GetExp,

		MissileExplode,//导弹爆炸

		SpeedUp,
		CuttleFish,
		FireBall,
		Mushroom,
		BananaSlip,
		//发射龟壳
		ShellShoot,
		//龟壳击中
		ShellHit,
		ShapeShift,

		//车辆碰撞
		CarCrash,
		//车辆撞飞
		CarCrashFly,
		//闪电
		Lighting,
		//漂移音效
		Drifting,
		//眩晕
		Stun,
		//最后一圈
		LastLap,

		CommonGift
	}
	#endregion
	
	#region 背景音乐名字 MusicName
	public enum MusicName
	{
		/// <summary>
		/// UI场景背景音乐
		/// </summary>
		UIBackground,
		/// <summary>
		/// 游戏场景背景音乐
		/// </summary>
		GameBackground,
	}
	#endregion
}
