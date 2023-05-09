
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FacebookTest : MonoBehaviour
{
    public Text txtError;
    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

        txtError.text = AndroidPackage.instance.errorString;
    }

    public void OnLoginClick()
    {
        //AndroidPackage.instance.InnerErrorMessage("OnLoginClick");

        //Action onSuccess = delegate { };
        //Action onFailed = delegate { };
        //Action onUserCancelled = delegate { };
        
        //AndroidPackage.instance.FBLogin(onSuccess, onFailed, onUserCancelled);
    }

    public void OnInviteClick()
    {
        //Action onSuccess = delegate { };
        //Action onFailed = delegate { };
        //Action onUserCancelled = delegate { };

        //AndroidPackage.instance.FBInviteFriend(onSuccess, onFailed, onUserCancelled);
    }

    public void OnSubmitScoreClick()
    {
        Debug.LogError("Not implement yet");

        Action onSuccess = delegate { };
        Action onFailed = delegate { };
        Action onUserCancelled = delegate { };

        // AndroidPackage.instance.FBSubmitScore(10, onSuccess, onFailed, onUserCancelled);
    }

   
}

