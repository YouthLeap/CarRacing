using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
// using Facebook.Unity;


public partial class AndroidPackage : MonoBehaviour
{
    /// <summary>
    /// Admob unit id
    /// </summary>
#if UNITY_ANDROID
    string adUnitId = "ca-app-pub-11111111/2222222"; // android
    string adUnitIdBanner = "no banner right now";
#else
    string adUnitId = "ca-app-pub-8013646213565823/7016523393"; // ios
    string adUnitIdBanner = "no banner right now"; 
#endif

    /// <summary>
    /// Onesignal app id
    /// </summary>
    string oneSignalAppId = "1111-222-3333-4444-55555555";

    /// <summary>
    /// Google project number
    /// </summary>
    string googleProjectNumber = "01234567890123";

    /// <summary>
    /// Maximum lenght is 60 characters
    /// </summary>
    string fbAppRequestMessage = "This game is great";

#if UNITY_EDITOR
    const string webServiceUrl = "http://192.168.1.105:9200/Services/YouGame.aspx?gid=10000"; //"http://192.168.1.105:9200/Services/YouGame.aspx?gid=8000";
    const string gameConfigUrl = "http://192.168.1.105:9200/Data/SHTCR/100.txt";
#else
    const string webServiceUrl = "http://192.168.1.105:9200/Services/YouGame.aspx?gid=10000";
    const string gameConfigUrl = "http://192.168.1.105:9200/Data/SHTCR/100.txt";
#endif


}



