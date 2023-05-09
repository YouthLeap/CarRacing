using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobAdsSet : MonoBehaviour {

	public BannerView bannerView;
	private InterstitialAd interstitial;

	public static AdmobAdsSet Instance;

	void Awake(){
		Instance = this;
	}
	void Start () {

		RequestInterstitial ();
		bannerView = new BannerView("ca-app-pub-1349008581689523/4043442868", AdSize.Banner, AdPosition.Bottom);
		AdRequest request = new AdRequest.Builder().Build();
		
		// Load the banner with the request.
		bannerView.LoadAd(request);

		showInterstitial ();
	}
	
	public void RequestInterstitial()
	{
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd("ca-app-pub-1349008581689523/3353586848");
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);
	}
	public void showInterstitial()
	{
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
		} else {
			RequestInterstitial();

		}
	}

	public void showBanner()
	{
		bannerView.Show ();
	}
	public void HideBanner()
	{
		bannerView.Hide ();
	}
}
