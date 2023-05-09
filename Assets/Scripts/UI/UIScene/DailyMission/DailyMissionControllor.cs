using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DailyMissionControllor : UIBoxBase {
	
	public static DailyMissionControllor Instance;
	
	public DailyMissionItem item1,item2,item3;
	
	public int[] curMissionIds = new int[3];
	
	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		DailyMissionCheckManager.Instance.Init ();
		InitData ();
		SetData ();

		base.Init();
	}
	
	private void InitData()
	{
		
		bool isRresh = true;
		string lastTime = PlayerData.Instance.GetDailyMissionDateTime ();
		if (!string.IsNullOrEmpty (lastTime)) {
			System.DateTime dateTime = System.DateTime.Parse (lastTime);
			System.DateTime nowTime = System.DateTime.Now;
			if (dateTime.Day != nowTime.Day || dateTime.Month != nowTime.Month || dateTime.Year != nowTime.Year) {
				isRresh = true;
			} else {
				isRresh = false;
			}
		} else {
			isRresh=true;
		}
		
		if (isRresh) {
			
			int[] ids =DailyMissionCheckManager.Instance.GetRandomId();
			curMissionIds= ids;
			DailyMissionCheckManager.Instance.CurMissionId=ids;
			PlayerData.Instance.SetDailyMissionCurMissionId (ids);
			PlayerData.Instance.SetDailyMissionDateTime (System.DateTime.Now.ToString ());
			
			DailyMissionCheckManager.Instance.RreshNum ();
		} else {
			DailyMissionCheckManager.Instance.LoadNum();
			curMissionIds= DailyMissionCheckManager.Instance.CurMissionId;
		}
		
	}
	
	
	public void SetData()
	{
		item1.SetData (curMissionIds [0]);
		item2.SetData (curMissionIds [1]);
		item3.SetData (curMissionIds [2]);
		
		SetTipsCount ();
	}
	
	void SetTipsCount()
	{
		int count = 0;
		if (item1.getState == DailyMissionItem.State.CanGet)
			++count;
		if (item2.getState == DailyMissionItem.State.CanGet)
			++count;
		if (item3.getState == DailyMissionItem.State.CanGet)
			++count;
		
		MainInterfaceControllor.Instance.SetTaskCount (count);
	}
	
	public override void Show ()
	{
		base.Show();
		SetData ();

		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.DailyMission, "Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
	}
	
	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule(UISceneModuleType.DailyMission);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}
	#endregion
	
	#region 按钮控制
	void CloseBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	#endregion
}