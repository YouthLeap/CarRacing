using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 礼包弹出 类型
/// </summary>
public enum GiftEventType
{
    Activate,
    Auto,
    Show,
    Click,
    Pay
}

/// <summary>
/// Custom event send ( maybe for anlytics )
/// </summary>
public class CollectInfoEvent
{

    //自定义事件的发送配置
    static bool Is_Umeng_Event = true;       //友盟统计

    public enum EventType
    {
        SignIn,
        Turnplate,
        ExchangeCode,
        ConvertCenter,

        Shop,
        DailyMission,
        Achievement,
        Complain,
        Activity,

        Pay,
        Pay_Success,
        Pay_Fail,
        Level_,
        Level_Quit,
        Player_WakeUp,
        Player_Upgrade,
        Strength_Buy,
        Dead,

        Gift_5Start,//五星豪礼
        Gift_AoteBrother,//角色礼包
        Gift_BuyJewel,//少量钻石
        Gift_Coin,//金币礼包
        Gift_DoubleCoin,//双倍金币
        Gift_LimitTime,//限时优惠
        Gift_Jewel,//钻石礼包
        Gift_3Start,//三星豪礼
        Gift_Light,//五灵之力
        Gift_NewPlayerGet,//新手礼包
        Gift_OneKeyFullLevel,//一键改满
        Gift_Reborn,//复活
        Gift_Shield,//五灵庇护
        Gift_Strengh,//体力回馈
        Gift_4Star,//四星豪礼

        Prop_SpeedUp,
        Prop_Shield,
        Prop_FlyBomb,

        Guide,//新手引导
        Loading,//加载界面统计
        Gift_MonthCardGift,//购买月卡礼包
        Reward_MonthCard,//月卡礼包奖励

        CommonGift,  //全民抢礼包
    };


#if UNITY_ANDROID

    static AndroidJavaClass ajc;
    static AndroidJavaObject ajo = null;

#endif

    /// <summary>
    /// 此方法用于后面跟着计数值，比如Level_1这种事件就用此方法。.
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="event_params">Event_params.</param>
    public static void SendEventWithCount(EventType eventType, string count, params string[] event_params)
    {
        SendEvent(eventType.ToString() + count, event_params);
    }

    /// <summary>
    /// <para>此方法用于正常事件发送，比如SignIn.</para>
    /// <para>如果事件类型后缀为下划线，请用<see cref="SendEventWithCount"/></para>
    /// </summary>
    /// <param name="eventType">Event type.</param>
    /// <param name="event_params">Event_params.</param>
    public static void SendEvent(EventType eventType, params string[] event_params)
    {
        //Debug.Log("Disable SendEvent");
        return;

        SendEvent(eventType.ToString(), event_params);
    }


    public static void SendGiftEvent(EventType eventType, GiftEventType type, params string[] event_params)
    {
        return;

        List<string> eventList = new List<string>();

        eventList.Add("State");
        switch (type)
        {
            case GiftEventType.Activate:
                eventList.Add("手动弹出");
                break;
            case GiftEventType.Auto:
                eventList.Add("自动弹出");
                break;
            case GiftEventType.Show:
                eventList.Add("弹出次数");
                break;
            case GiftEventType.Click:
                eventList.Add("确认次数(购买)");
                break;
            case GiftEventType.Pay:
                eventList.Add("付费次数(计费成功)");
                break;
        }

        eventList.Add("Level");
        eventList.Add(PlayerData.Instance.GetSelectedLevel().ToString());

        eventList.AddRange(event_params);


        SendEvent(eventType.ToString(), eventList.ToArray());
    }

    private static void SendEvent(string eventName, params string[] event_params)
    {
        return;

#if UNITY_EDITOR || UNITY_EDITOR_OSX
        //事件属性处理
        System.Text.StringBuilder key_value_str = new System.Text.StringBuilder();
        if (event_params != null && event_params.Length != 0)
        {
            for (int i = 0; i < event_params.Length; ++i)
            {
                if (i % 2 == 0)
                    key_value_str.Append(event_params[i]).Append("-").Append(event_params[i + 1]);
                else if (i != (event_params.Length - 1))
                    key_value_str.Append("|");
            }
        }
        else
        {
            key_value_str.Append("");
        }
        Debug.Log(eventName + " : " + key_value_str.ToString());
        return;
#endif

#if UNITY_IOS
		return;
#endif

#if UNITY_ANDROID
        try
        {
            if (ajc == null)
                ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (ajo == null)
            {
                ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
            }

            //Event attribute processing
            System.Text.StringBuilder key_value = new System.Text.StringBuilder();
            if (event_params != null && event_params.Length != 0)
            {
                for (int i = 0; i < event_params.Length; ++i)
                {
                    if (i % 2 == 0)
                        key_value.Append(event_params[i]).Append("-").Append(event_params[i + 1]);
                    else if (i != (event_params.Length - 1))
                        key_value.Append("|");
                }
            }
            else
            {
                key_value.Append("");
            }

            //Friends of the Union statistics
            if (Is_Umeng_Event)
            {
                ajo.Call("SendEvent", eventName, key_value.ToString());
            }
        }
        catch
        {
        }
#endif
    }

    public static void StartLevel(int level)
    {
        Debug.Log("Start the level：" + level);
#if UNITY_EDITOR || UNITY_EDITOR_OSX

        return;
#endif

#if UNITY_IOS
		return;
#endif

#if UNITY_ANDROID
        try
        {
            if (ajc == null)
                ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (ajo == null)
            {
                ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            ajo.Call("StartLevel", level.ToString());
        }
        catch
        {
            //			Debug.Log("Send Start Level Info Fail");
        }
#endif
    }

    public static void FailLevel(int level)
    {
        Debug.Log("Failure level：" + level);
#if UNITY_EDITOR || UNITY_EDITOR_OSX

        return;
#endif

#if UNITY_IOS
		return;
#endif

#if UNITY_ANDROID
        try
        {
            if (ajc == null)
                ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (ajo == null)
            {
                ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            ajo.Call("FailLevel", level.ToString());
        }
        catch
        {
            //			Debug.Log("Send Fail Level Info Fail");
        }
#endif
    }

    public static void FinishLevel(int level)
    {
        Debug.Log("Complete the level：" + level);
#if UNITY_EDITOR || UNITY_EDITOR_OSX

        return;
#endif

#if UNITY_IOS
		return;
#endif

#if UNITY_ANDROID
        try
        {
            if (ajc == null)
                ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (ajo == null)
            {
                ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            ajo.Call("FinishLevel", level.ToString());
        }
        catch
        {
            //			Debug.Log("Send Finish Level Info Fail");
        }
#endif
    }

    public static void PayEvent(float price, float coin, int source)
    {
        Debug.Log("PayEvent：" + price);
#if UNITY_EDITOR || UNITY_EDITOR_OSX

        return;
#endif

#if UNITY_IOS
		return;
#endif

#if UNITY_ANDROID
        try
        {
            if (ajc == null)
                ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (ajo == null)
            {
                ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            ajo.Call("PayEvent", price, coin, source);
        }
        catch
        {
            //			Debug.Log("Send Finish Level Info Fail");
        }
#endif
    }

    public static void PayEvent(float money, string item, int number, float price, int source)
    {
        Debug.Log("PayEvent：" + money + " " + item + " " + number + " " + price + " " + source);
#if UNITY_EDITOR || UNITY_EDITOR_OSX

        return;
#endif

#if UNITY_IOS
        return;
#endif

#if UNITY_ANDROID
        try
        {
            if (ajc == null)
                ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (ajo == null)
            {
                ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            ajo.Call("PayEvent", money, item, number, price, source);
        }
        catch
        {
            //          Debug.Log("Send Finish Level Info Fail");
        }
#endif
    }

}
