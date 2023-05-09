using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
// using Facebook.Unity;
using Facebook.MiniJSON;

public partial class AndroidPackage : MonoBehaviour
{
    public List<TopUserInMap> lstTopUserInMap = new List<TopUserInMap>();

    #region defined
    public class KeyDefine
    {
        public const string requestKeyFunction = "f";
        public const string requestKeyNick = "n";
        public const string requestKeyEmail = "ema";
        public const string requestKeyGameId = "gid";
        public const string requestKeyToken = "t";
        public const string requestKeyScore = "s";
        public const string requestKeyData = "dt";
        public const string requestTotalData = "rtd";
        public const string requestKeyCPID = "cpid";

        /// <summary>
        /// t3
        /// </summary>
        public const string fGetTop3 = "t3";

        /// <summary>
        /// twz
        /// </summary>
        public const string fGetTopWarZone = "twz";

        /// <summary>
        /// ruig
        /// </summary>
        public const string fGetRandomUserInGame = "ruig";

        /// <summary>
        /// r
        /// </summary>
        public const string fRegister = "r";

        /// <summary>
        /// as
        /// </summary>
        public const string fAddScore = "as";

        /// <summary>
        /// fUpdateInfo
        /// </summary>
        public const string fUpdateInfo = "ud";

        public const string fStartPlayGame = "spg";

        public const string retResult = "rr";
        public const string retCode = "rc";
        public const string retData = "data";

        public const string jKeyMessage = "msg";

        public const string jKeyTop3 = "t3";
        public const string jKeyTop = "top";
        public const string jKeyNick = "n";
        public const string jKeyScore = "s";
        public const string jKeyToken = "t";
        public const string jKeyData = "d";
        public const string jKeyNode = "node";

        public const string codeInvalidToken = "iv";

        public const string fLogLevel = "fll";
        public const string jKeyLogType = "lt";
        public const string jKeyLogLevel = "ll";
    }
    #endregion

    #region services
    public void CallWebService(string url, Action<Dictionary<string, object>> onSuccess, Action<string, string> onFailed)
    {
        StartCoroutine(StartCallWebService(url, onSuccess, onFailed));
    }

    IEnumerator StartCallWebService(string url, Action<Dictionary<string, object>> onSuccess, Action<string, string> onFailed)
    {
        if (onSuccess == null)
        {
            onSuccess = delegate (Dictionary<string, object> data)
            {

            };
        }
        if (onFailed == null)
        {
            onFailed = delegate (string data, string data2)
            {

            };
        }

        
        url += "&" + KeyDefine.jKeyToken + "=" + lastLoginToken;
        
        if(Application.platform == RuntimePlatform.Android)
        {
            url += "&os=android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url += "&os=ios";
        }
        else
        {
            url += "&os=unknown";
        }

        Debug.Log("===> call url:" + url);
        WWW w = new WWW(url);

        yield return w;

        if (!string.IsNullOrEmpty(w.error))
        {
            onFailed(w.error, "");
            yield break;
        }

        InnerErrorMessage(w.text);

        Dictionary<string, object> result = (Dictionary<string, object>)Json.Deserialize(w.text);

        bool isSuccess = result.Get<string>(KeyDefine.retResult).ToBool();
        if (isSuccess == false)
        {
            string code = result.Get<string>(KeyDefine.retCode);
            if (code == KeyDefine.codeInvalidToken) // wrong token
            {
                lastLoginToken = ""; // reset wrong token, so that we can re-register on next call
            }
            Debug.Log("Call service error [code:" + code + "][mess:" + result.Get<string>(KeyDefine.retData) + "]");
            onFailed(code, result.Get<string>(KeyDefine.retData));
            yield break;
        }

        onSuccess(result.Get<Dictionary<string, object>>(KeyDefine.retData));
    }
    #endregion

    public void LBRegister(Action onSuccess, Action<string, string> onFailed)
    {
        try
        {


            if (onSuccess == null)
            {
                onSuccess = delegate { };
            }
            if (onFailed == null)
            {
                onFailed = delegate (string code, string message) { };
            }

            if (lastLoginToken != "") // already registered
            {
                onSuccess();
                return;
            }

            string url = webServiceUrl_Register;
            Action<Dictionary<string, object>> onRegisterSuccess = delegate (Dictionary<string, object> data)
            {
                string token = data.Get<string>(KeyDefine.jKeyToken);
                lastLoginToken = token;
                onSuccess();
            };
            CallWebService(url, onRegisterSuccess, onFailed);
        }
        catch (Exception exp)
        {
            LogException("LBRegister error", exp);
            if (onFailed != null)
            {
                onFailed("", "");
            }
        }
    }

    public void LBAddScore(int score, string additionalData, Action onSuccess, Action<string, string> onFailed)
    {
        try // incase error, the game should continue 
        {


            if (onSuccess == null)
            {
                onSuccess = delegate { };
            }
            if (onFailed == null)
            {
                onFailed = delegate (string code, string message) { };
            }

            nextSumitScore += score;

            LBRegister(() =>
            {
                string url = webServiceUrl_AddScore + "&" + KeyDefine.requestKeyScore + "=" + nextSumitScore + "&" + KeyDefine.requestKeyData + "=" + additionalData;
                Action<Dictionary<string, object>> onAddScoreSuccess = delegate (Dictionary<string, object> data)
                {
                    nextSumitScore = 0;
                    onSuccess();
                };

                CallWebService(url, onAddScoreSuccess, onFailed);
            }, onFailed);
        }
        catch (Exception exp)
        {
            instance.LogException("LBAddScore error", exp);
            if (onFailed != null)
            {
                onFailed("", "");
            }
        }
    }

    public void LBGetTop(Action<List<object>> onSuccess, Action<string, string> onFailed, int totalData)
    {
        try
        {


            if (onSuccess == null)
            {
                onSuccess = delegate (List<object> result) { };
            }
            if (onFailed == null)
            {
                onFailed = delegate (string code, string message) { };
            }

            string url = webServiceUrl_GetTopRanking;
            url += "&" + KeyDefine.requestTotalData + "=" + totalData;

            Action<Dictionary<string, object>> onGetSuccess = delegate (Dictionary<string, object> result)
            {
                List<object> lst = result.Get<List<object>>(KeyDefine.jKeyTop);
                onSuccess(lst);
            };


            CallWebService(url, onGetSuccess, onFailed);
        }
        catch (Exception exp)
        {
            LogException("LBGetTop error", exp);
            if (onFailed != null)
            {
                onFailed("", "");
            }
        }
    }

    public void LBUpdateInfo(string nick, string email, Action<string> onSuccess, Action<string, string> onFailed)
    {
        try
        {
            if (onSuccess == null)
            {
                onSuccess = delegate (string strNick) { };
            }
            if (onFailed == null)
            {
                onFailed = delegate (string code, string message) { };
            }

            LBRegister(() =>
            {
                string url = webServiceUrl_UpdateInfo
                                + "&" + KeyDefine.requestKeyNick + "=" + WWW.EscapeURL(nick)
                                + "&" + KeyDefine.requestKeyEmail + "=" + WWW.EscapeURL(email);
                Action<Dictionary<string, object>> onSendSuccess = delegate (Dictionary<string, object> data)
                {
                    onSuccess(nick);
                };

                CallWebService(url, onSendSuccess, onFailed);
            }, onFailed);
        }
        catch (Exception exp)
        {
            LogException("LBUpdateInfo error", exp);
            if (onFailed != null)
            {
                onFailed("", "");
            }
        }
    }

    public void LBGetRandomUserInGame(Action<List<TopUserInMap>> onSuccess, Action<string, string> onFailed, int totalData = 20)
    {
        try
        {
            if (onSuccess == null)
            {
                onSuccess = delegate (List<TopUserInMap> result) { };
            }
            if (onFailed == null)
            {
                onFailed = delegate (string code, string message) { };
            }

            string url = webServiceUrl_GetRandomUserInGame;
            url += "&" + KeyDefine.requestTotalData + "=" + totalData;

            Action<Dictionary<string, object>> onGetSuccess = delegate (Dictionary<string, object> result)
            {
                List<object> lst = result.Get<List<object>>(KeyDefine.jKeyTop);
                if (lst == null) { lst = new List<object>(); }

                lstTopUserInMap = new List<TopUserInMap>();

                for (int i = 0; i < lst.Count; i++)
                {
                    Dictionary<string, object> data = (Dictionary<string, object>)lst[i];

                    TopUserInMap item = new TopUserInMap();
                    item.nick = data.Get<string>(AndroidPackage.KeyDefine.jKeyNick);
                    item.node = int.Parse(data.Get<string>(AndroidPackage.KeyDefine.jKeyNode));
                    lstTopUserInMap.Add(item);
                }

                onSuccess(lstTopUserInMap);
            };

            CallWebService(url, onGetSuccess, onFailed);
        }
        catch (Exception exp)
        {
            LogException("LBGetRandomUserInGame error", exp);
            if (onFailed != null)
            {
                onFailed("", "");
            }
        }
    }

    public enum LogLevelType : int
    {
        start_level = 1,
        win_level = 2,
        lose_level = 3,
    }

    public void LogLevel(LogLevelType logLevelType, int level)
    {
        try // incase error, the game should continue 
        {            
            Action<string, string> onFailed = delegate (string code, string message) { };

            string url = webServiceUrl_LogLevel + "&" + KeyDefine.jKeyLogType + "=" + ((int)logLevelType) + "&" + KeyDefine.jKeyLogLevel + "=" + level;
            Action<Dictionary<string, object>> onAddScoreSuccess = delegate (Dictionary<string, object> data) { };                                       

            CallWebService(url, onAddScoreSuccess, onFailed);
        }
        catch (Exception exp)
        {
            instance.LogException("LogLevel error", exp);            
        }
    }

    void StartPlayGame()
    {
        string url = webServiceUrl_StartPlayGame;//  + "&" + KeyDefine.requestKeyToken + "=" + AndroidPackage.instance.lastLoginToken;

        //Action<Dictionary<string, object>> onRegisterSuccess = delegate (Dictionary<string, object> data)
        //{
        //    string token = data.Get<string>(KeyDefine.jKeyToken);
        //    lastLoginToken = token;

        //};
        CallWebService(url, null, null);
    }

    public void ProcessFirstTimeEnterGame()
    {
        //// temporary no use
        //return;

        //AndroidPackage.instance.lastLoginToken = "";
        //return; 


        try
        {
            if (AndroidPackage.instance.lastLoginToken == "")
            {
                AndroidPackage.instance.LBRegister(null, null);
                //try
                //{
                //    Debug.Log("Firebase EventSignUp");
                //    Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventSignUp);
                //}
                //catch (Exception exp)
                //{
                //    Debug.LogError("Firebase error:" + exp.ToString());
                //    //MessageBoxPopupScr.ShowMessageBox("fb:" + exp.ToString());
                //}
            }
            else
            {
                StartPlayGame();
                //try
                //{
                //    Debug.Log("Firebase EventLogin");
                //    Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
                //}
                //catch (Exception exp)
                //{
                //    Debug.LogError("Firebase error:" + exp.ToString());
                //    //MessageBoxPopupScr.ShowMessageBox("fb:" + exp.ToString());
                //}
            }
        }
        catch (Exception exp)
        {
            AndroidPackage.instance.LogException("ProcessFirstTimeEnterGame", exp);
        }
    }

    public class TopUserInMap
    {
        public string nick;
        public int node;
    }

    #region customize    

    public void DownloadFile(string url, Action<Dictionary<string, object>> onSuccess, Action<string, string> onFailed)
    {
        StartCoroutine(StartDownloadFile(url, onSuccess, onFailed));
    }

    IEnumerator StartDownloadFile(string url, Action<Dictionary<string, object>> onSuccess, Action<string, string> onFailed)
    {
        if (onSuccess == null)
        {
            onSuccess = delegate (Dictionary<string, object> data)
            {

            };
        }
        if (onFailed == null)
        {
            onFailed = delegate (string data, string data2)
            {

            };
        }

        Debug.Log("===> download file url:" + url);
        WWW w = new WWW(url);

        yield return w;

        if (!string.IsNullOrEmpty(w.error))
        {
            onFailed(w.error, "");
            yield break;
        }

        InnerErrorMessage(w.text);

        Dictionary<string, object> result = (Dictionary<string, object>)Json.Deserialize(w.text);

        onSuccess(result);
    }

    //public void LoadGameConfig(Action onSuccess, Action<string, string> onFailed)
    //{
    //    if (onSuccess == null)
    //    {
    //        onSuccess = delegate  { };
    //    }
    //    if (onFailed == null)
    //    {
    //        onFailed = delegate (string code, string message) { };
    //    }

    //    try // in case error, the game should continue 
    //    {
    //        DownloadFile(gameConfigUrl + "?v=" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"), (Dictionary<string, object> res) =>
    //        {
    //            GameConfig.instance.ParseData(res);

    //            onSuccess();

    //        }, onFailed);

    //    }
    //    catch (Exception exp)
    //    {
    //        instance.LogException("LoadGameConfig error", exp);
    //        onFailed("", "");
    //    }
    //}

    #endregion
}

