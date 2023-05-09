using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
// using Facebook.Unity;


public partial class AndroidPackage : MonoBehaviour
{
#if UNITY_ANDROID
    bool isUseOneSignal = true;
#elif UNITY_IOS
    bool isUseOneSignal = true;
#else
    bool isUseOneSignal = false;
#endif


    void InitOneSignal()
    {
        if(isUseOneSignal == false) { return; }

        try
        {
            OneSignal.StartInit(oneSignalAppId)
            .Settings(new Dictionary<string, bool>() {
                { OneSignal.kOSSettingsAutoPrompt, true }})
            .HandleNotificationOpened(HandleNotificationOpened)
            .EndInit();

            OneSignal.EnableVibrate(true);
            OneSignal.EnableSound(true);
        }
        catch(Exception exp)
        {
            Debug.LogError("OneSignal error:" + exp.ToString());
        }
        
        //// temporary no use
        //return;

        //try
        //{
        //    if (isUseOneSignal == false) { return; }

        //    // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
        //    //OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);

        //    //OneSignal.Init(oneSignalAppId, googleProjectNumber, HandleNotification);
        //    OneSignal.Init(oneSignalAppId);

        //    // Shows a Native iOS/Android alert dialog when the user is in your app when a notification comes in.
        //    // true: show
        //    // false: not show
        //    OneSignal.EnableInAppAlertNotification(false);

        //    SendMyID();
        //}
        //catch (Exception exp)
        //{
        //    LogException("InitOneSignal error", exp);
        //}
    }
    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {

    }

    void SendMyID()
    {
        //OneSignal.GetIdsAvailable((playerId, pushToken) =>
        //{
        //    WebService.Api.RegisterPushNotification(playerId, pushToken, OnRegisterPushNotificationComplete);
        //});           
    }
    void OnRegisterPushNotificationComplete(Dictionary<string, object> val)
    {
        // do nothing
    }

    private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive)
    {
        //print("GameControllerExample:HandleNotification:message" + message);

        //extraMessage += "Notification opened with text: " + message;

        // When isActive is true this means the user is currently in your game.
        // Use isActive and your own game logic so you don't interrupt the user with a popup or menu when they are in the middle of playing your game.
        /*if (additionalData != null) {
           if (additionalData.ContainsKey("discount")) {
              extraMessage = (string)additionalData["discount"];
              // Take user to your store.
           }
           else if (additionalData.ContainsKey("actionSelected")) {
              // actionSelected equals the id on the button the user pressed.
              // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
              extraMessage = "Pressed ButtonId: " + additionalData["actionSelected"];
           }
        }*/
    }
   

}

