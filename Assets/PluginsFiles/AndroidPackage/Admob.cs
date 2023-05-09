using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;


public partial class AndroidPackage : MonoBehaviour
{

    [HideInInspector]
    public InterstitialAd interstitialAds;
#if UNITY_ANDROID
    public bool isUseAds = false;
#elif UNITY_IOS
    public bool isUseAds = false;
#else
    public bool isUseAds = false;
#endif

    DateTime lastTimeShowAd = DateTime.Now;

    private void RequestInterstitial()
    {
        //// temporary disable ads
        //return;
        //// end
        try
        {
            if (isUseAds == false) { return; }

            if (interstitialAds != null)
            {
                try
                {
                    interstitialAds.Destroy();
                }
                catch { }

                interstitialAds = null;
            }

            LogEvent("Admob", "RequestInterstitial");
            InnerErrorMessage("adUnitId:" + adUnitId);

            // Initialize an InterstitialAd.
            interstitialAds = new InterstitialAd(adUnitId);


            // Called when an ad request has successfully loaded.
            interstitialAds.OnAdLoaded += HandleAdLoaded;

            // Called when an ad request failed to load.
            interstitialAds.OnAdFailedToLoad += HandleAdFailedToLoad;

            // Called when an ad is clicked.
            interstitialAds.OnAdOpening += HandleAdOpening; ;

            // Called when the user is about to return to the app after an ad click.
            // interstitialAds.OnAdClosed += HandleAdClosing;

            // Called when the user returned from the app after an ad click.
            interstitialAds.OnAdClosed += HandleAdClosed;

            // Called when the ad click caused the user to leave the application.
            interstitialAds.OnAdLeavingApplication += HandleAdLeftApplication;



            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the interstitial with the request.
            interstitialAds.LoadAd(request);

            InnerErrorMessage("LoadAd");
        }
        catch (Exception exp)
        {
            LogException("RequestInterstitial error", exp);
        }
    }

    private void HandleAdOpening(object sender, EventArgs e)
    {
        LogEvent("Admob", "HandleAdOpening");
    }

    public void ShowInterstitialAds()
    {
        ShowAdmob();
    }   

    public void ShowAdmob()
    {
        //// disable admob
        //Debug.LogError("ShowAdmob disabled");
        //return;
        //// end
        try
        {
            if (isUseAds == false) { return; }            

            if (lastTimeShowAd.AddSeconds(30).CompareTo(DateTime.Now) > 0) { return; }

            lastTimeShowAd = DateTime.Now;

            InnerErrorMessage("ShowAdmob ");

            if (interstitialAds == null)
            {
                InnerErrorMessage("ShowAdmob  interstitialAds == null");
                RequestInterstitial();
                return;
            }

            if (interstitialAds.IsLoaded())
            {
                InnerErrorMessage("interstitialAds  IsLoaded");                                
                interstitialAds.Show();
                //try
                //{
                //    Firebase.Analytics.FirebaseAnalytics.LogEvent("ShowAdmob", "Interstitial", 1);
                //}
                //catch { }
                InnerErrorMessage("interstitialAds  show");

                
            }
            else
            {
                RequestInterstitial();
                InnerErrorMessage("ShowAdmob  interstitialAds.IsLoaded() = false");
            }
        }
        catch (Exception exp)
        {

            LogException("ShowAdmob error", exp);
        }
    }

    private void HandleAdLeftApplication(object sender, System.EventArgs e)
    {

        //try
        //{
        //    Firebase.Analytics.FirebaseAnalytics.LogEvent("Admob_LeftApp", "Admob_LeftApp", 1);
        //}
        //catch
        //{

        //}
        LogEvent("Admob", "HandleAdLeftApplication");
    }

    private void HandleAdClosed(object sender, System.EventArgs e)
    {
        LogEvent("Admob", "HandleAdClosed");
        // re request new ads
        RequestInterstitial();
    }

    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {

        LogEvent("Admob", "HandleAdFailedToLoad:" + e.Message);
    }

    private void HandleAdLoaded(object sender, System.EventArgs e)
    {

        LogEvent("Admob", "HandleAdLoaded");
    }

    #region baner ads
    BannerView bannerView = null;

    public void RequestBanner()
    {
        return; // disable banner ads

        Debug.Log("RequestBanner");
        try
        {
            if (bannerView != null)
            {
                bannerView.Hide();
                bannerView.Destroy();
                bannerView = null;
            }

            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitIdBanner, AdSize.Banner, AdPosition.Top);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            bannerView.LoadAd(request);
        }
        catch (Exception exp)
        {
            // xQueenStudio.MessageBoxPopupScr.ShowMessageBox("err 1:" + exp.ToString());
            InnerErrorMessage(exp.ToString());
        }

    }

    public void ShowBanner()
    {
        return; // disable banner ads

        try
        {
            if (bannerView != null)
            {
               
                //bannerView.Hide();
                bannerView.Show();
                return;
            }

           
            RequestBanner();
        }
        catch (Exception exp)
        {
            // xQueenStudio.MessageBoxPopupScr.ShowMessageBox("err 2:" + exp.ToString());
            InnerErrorMessage(exp.ToString());
        }
    }

    public void HideBanner()
    {
        return; // disable banner ads

        try
        {
            if (bannerView != null)
            {
                bannerView.Hide();
            }
        }
        catch (Exception exp)
        {
            // xQueenStudio.MessageBoxPopupScr.ShowMessageBox("err 3:" + exp.ToString());
            InnerErrorMessage(exp.ToString());
        }

    }
    #endregion

}

