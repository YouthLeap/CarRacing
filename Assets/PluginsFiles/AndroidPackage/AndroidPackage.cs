using UnityEngine;
//using System.Collections;
//using GoogleMobileAds.Api;
//using System;
//using System.Collections.Generic;
//using Facebook.Unity;


public partial class AndroidPackage : MonoBehaviour
{
    public enum ReleaseCIPI : int
    {
        CHPlay = 1,
        Amazon = 2,
        WindowStore = 3,
        MoboMarket = 4,
        Slideme = 5,
        Appslib = 6,
        Getjar = 7,
    }

    public ReleaseCIPI releaseCIPI;

#if UNITY_EDITOR
    [HideInInspector]
    public bool isDebug = true;
#else
    [HideInInspector]
    public bool isDebug = false;
#endif
    public static AndroidPackage instance;

    [HideInInspector]
    public string errorString = "";

    #region leaderboard

    public string lastLoginToken
    {
        get
        {
            return PlayerPrefs.GetString("___lastLoginToken_", "");
        }
        set
        {
            PlayerPrefs.SetString("___lastLoginToken_", value);
        }
    }


    public int nextSumitScore
    {
        get
        {
            return PlayerPrefs.GetInt("___nextSumitScore_", 0);
        }
        set
        {
            PlayerPrefs.SetInt("___nextSumitScore_", value);
        }
    }


    string currentCIPIParam
    {
        get
        {
            string cpid = "";
            switch (releaseCIPI)
            {
                case ReleaseCIPI.CHPlay: cpid = "CHPlay"; break;
                case ReleaseCIPI.Amazon: cpid = "Amazon"; break;
                case ReleaseCIPI.WindowStore: cpid = "WindowStore"; break;
                case ReleaseCIPI.MoboMarket: cpid = "MoboMarket"; break;
                case ReleaseCIPI.Slideme: cpid = "Slideme"; break;
                case ReleaseCIPI.Appslib: cpid = "Appslib"; break;
                case ReleaseCIPI.Getjar: cpid = "Getjar"; break;
                default: cpid = "x"; break;

            }
            return "&" + KeyDefine.requestKeyCPID + "=" + cpid;
        }
    }

    string GetParamFunction(string func)
    {
        return "&" + KeyDefine.requestKeyFunction + "=" + func;
    }

    string GetServiceUrl(string func)
    {
        return webServiceUrl + GetParamFunction(func) + currentCIPIParam;
    }

    /// <summary>
    /// AMAZON , CHPlay
    /// </summary>
    string webServiceUrl_Register { get { return GetServiceUrl(KeyDefine.fRegister); } }
    string webServiceUrl_AddScore { get { return GetServiceUrl(KeyDefine.fAddScore); } }
    string webServiceUrl_UpdateInfo { get { return GetServiceUrl(KeyDefine.fUpdateInfo); } }
    string webServiceUrl_GetTopRanking { get { return GetServiceUrl(KeyDefine.fGetTopWarZone); } }
    string webServiceUrl_StartPlayGame { get { return GetServiceUrl(KeyDefine.fStartPlayGame); } }

    string webServiceUrl_GetRandomUserInGame { get { return GetServiceUrl(KeyDefine.fGetRandomUserInGame); } }

    string webServiceUrl_LogLevel { get { return GetServiceUrl(KeyDefine.fLogLevel); } }

    #endregion
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        InitMe();
    }

    void InitMe()
    {
        StartSessionAnalytic();
        RequestInterstitial();
        //RequestBanner();
        InitOneSignal();
    }

    public void InnerErrorMessage(string message)
    {
        if (isDebug == false) { return; }

        errorString += "\n" + message;
        Debug.Log(message);
    }
}


