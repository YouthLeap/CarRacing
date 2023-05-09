using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class AndroidPackageTest
{

    public Text txtError;
	// Use this for initialization
	void Start () {
       
	}

   
	// Update is called once per frame
	void Update () {

        txtError.text = AndroidPackage.instance.errorString;
	}

    public void OnShowAdsClick()
    {
        // xQueen.AndroidPackage.instance.errorString = "OnShowAdsClick";
        AndroidPackage.instance.ShowAdmob();
        // xQueen.AndroidPackage.instance.errorString = "OnShowAdsClick done";
    }

    public void OnGATestClick()
    {
        AndroidPackage.instance.LogEvent("Test", "OnGATestClick");
    }

    
}
