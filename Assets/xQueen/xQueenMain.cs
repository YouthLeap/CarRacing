using PayBuild;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xQueenMain : MonoBehaviour {

    private void Awake()
    {
        if(
            Application.platform == RuntimePlatform.WindowsEditor 
            || Application.platform == RuntimePlatform.OSXEditor
            || Application.platform == RuntimePlatform.LinuxEditor
        )
        {
            Debug.logger.logEnabled = true;
        }
        else
        {
            Debug.logger.logEnabled = false;
        }        

        PayBuildPayManage pbpm = PayBuildPayManage.Instance;
        AndroidPackage.instance.ProcessFirstTimeEnterGame();
    }

}
