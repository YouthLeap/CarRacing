using UnityEngine;
//using System.Collections;
//using GoogleMobileAds.Api;
using System;
//using System.Collections.Generic;
// using Facebook.Unity;


public partial class AndroidPackage : MonoBehaviour
{
#if UNITY_ANDROID
    bool isUseAnalytic = false;
#elif UNITY_IOS
    bool isUseAnalytic = false;
#else
    bool isUseAnalytic = false;
#endif

    public void StartSessionAnalytic()
    {        
        try
        {
            if (isUseAnalytic == false) { return; }

            Debug.Log("Enabling data collection.");            
            //Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        }
        catch (Exception exp)
        {
            InnerErrorMessage("StartSessionAnalytic error: " + exp.ToString());
        }
    }

    public void LogScreen(string screenName)
    {
        //// temporary no use
        //return;

        try
        {
            if (isUseAnalytic == false) { return; }

            InnerErrorMessage("logScreen: " + screenName);
            //Firebase.Analytics.FirebaseAnalytics.LogEvent("log_screen", screenName, "");                        
        }
        catch (System.Exception exp)
        {
            InnerErrorMessage("GA LogScreen error => " + exp.ToString());

        }
    }
    public void LogEvent(string eventCategory, string eventAction)
    {        
        try
        {
            if (isUseAnalytic == false) { return; }

            InnerErrorMessage("LogEvent: " + eventCategory + " - " + eventAction);
            //Firebase.Analytics.FirebaseAnalytics.LogEvent("LogEvent", eventCategory, eventAction);
            //googleAnalytics.LogEvent
            //(
            //    new EventHitBuilder()
            //        .SetEventCategory(eventCategory)
            //        .SetEventAction(eventAction)
            //);
        }
        catch (System.Exception exp)
        {
            InnerErrorMessage("GA LogEvent error => " + exp.ToString());
        }
    }

    public void LogException(string title, System.Exception anException)
    {
        
        try
        {
            if (isUseAnalytic == false) { return; }

            InnerErrorMessage("LogException anException: [" + title + "]" + anException.ToString());
            //Firebase.Analytics.FirebaseAnalytics.LogEvent("exception", title, anException.ToString());
            //googleAnalytics.LogException
            //(
            //    new ExceptionHitBuilder()
            //        .SetExceptionDescription(anException.ToString())
            //        .SetFatal(true)
            //);

        }
        catch (System.Exception exp)
        {
            InnerErrorMessage("LogException with exception => " + exp.ToString());
        }
    }


}

