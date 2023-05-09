
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardTest : MonoBehaviour
{
    public Text txtError;
    // Use this for initialization
    void Start()
    {
        // PlayerPrefs.DeleteAll();
    }


    // Update is called once per frame
    void Update()
    {

        txtError.text = AndroidPackage.instance.errorString;
    }

    public void OnRegisterClick()
    {
        AndroidPackage.instance.InnerErrorMessage("OnLoginClick");

        Action onSuccess = delegate 
        {
            Debug.LogError("Register success");
        };
        Action<string, string> onFailed = delegate (string code, string message) 
        {
            Debug.LogError("register failed");
            Debug.LogError("message:" + message);
        };

        AndroidPackage.instance.LBRegister(onSuccess, onFailed);
    }

    public void OnAddScore()
    {
        AndroidPackage.instance.InnerErrorMessage("OnAddScore");

        Action onSuccess = delegate
        {
            Debug.LogError("Add score success");
        };
        Action<string, string> onFailed = delegate (string code, string message)
        {
            Debug.LogError("add score failed");
            Debug.LogError("message:" + message);
        };

        AndroidPackage.instance.LBAddScore(7, "", onSuccess, onFailed);
    }
   
}

