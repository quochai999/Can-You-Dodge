//using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class AdsWatcher : MonoBehaviour
{
    private string gameId = "3373937";
    private bool testMode = true;
    private string placementId = "rewardedVideo";
    private ShowAdPlacementContent ad = null;
    public Action actionWatchFailed;
    public Action actionWatchSuccess;
    [SerializeField]
    private Button btGetMore;
    [SerializeField]
    private GameObject imgMask, centerOb;
    //private RewardedAd rewardedAd;
    //private string unitTestId = "ca-app-pub-3940256099942544/5224354917";
    //private string unitRealId = "ca-app-pub-9286235148771355/8172660376";

    private void Start()
    {
        CreateAds();
    }

    private void CreateAds()
    {
        if (Monetization.isSupported)
            Monetization.Initialize(gameId, false);
        //MobileAds.Initialize("ca-app-pub-9286235148771355~1073254772");
        //CreateAndLoadRewardedAd();
    }

    //private void CreateAndLoadRewardedAd()
    //{
    //    this.rewardedAd = new RewardedAd(unitTestId);
    //    // Called when an ad request has successfully loaded.
    //    this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
    //    // Called when an ad request failed to load.
    //    this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
    //    // Called when an ad is shown.
    //    this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
    //    // Called when an ad request failed to show.
    //    this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
    //    // Called when the user should be rewarded for interacting with the ad.
    //    this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    //    // Called when the ad is closed.
    //    this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

    //    AdRequest request = new AdRequest.Builder().Build();
    //    this.rewardedAd.LoadAd(request);
    //}

    public void Run()
    {
        if (centerOb != null)
            centerOb.SetActive(true);
        //if (!this.rewardedAd.IsLoaded())
        //    this.CreateAndLoadRewardedAd();
        WaitForAds();
    }

    private async void WaitForAds()
    {
        SetBtGetMoreInteractable(false);
        while (true)
        {
            //if (this.rewardedAd.IsLoaded() || Monetization.IsReady(placementId)) break;
            if ( Monetization.IsReady(placementId)) break;
            await Task.Delay(100);
        }
        //if (this.rewardedAd.IsLoaded())
        //{
        //    SetBtGetMoreInteractable(true);
        //}
        if (Monetization.IsReady(placementId))
        {
            ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;
            if (ad != null)
                SetBtGetMoreInteractable(true);
        }
    }

    public void ClickGetMore()
    {
//        if (this.rewardedAd.IsLoaded())
//        {
//#if UNITY_EDITOR
//            WatchSuccess();
//            this.CreateAndLoadRewardedAd();
//#else
//            this.rewardedAd.Show();
//#endif
//            return;
//        }

        if (!Monetization.IsReady(placementId)) return;
        if (ad != null)
            ad.Show(AdFinished);
    }

    private void AdFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            WatchSuccess();
        }
        else if (result == ShowResult.Failed)
        {
            Debug.Log("Watch Ads failed.");
            ClickBack();
        }
        else if (result == ShowResult.Skipped)
        {
            Debug.Log("Ads skipped.");
            ClickBack();
        }
    }

    //public void HandleRewardedAdLoaded(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardedAdLoaded event received");
    //}

    //public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    //{
    //    MonoBehaviour.print(
    //        "HandleRewardedAdFailedToLoad event received with message: "
    //                         + args.Message);
    //}

    //public void HandleRewardedAdOpening(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardedAdOpening event received");
    //}

    //public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    //{
    //    MonoBehaviour.print(
    //        "HandleRewardedAdFailedToShow event received with message: "
    //                         + args.Message);
    //    ClickBack();
    //}

    //public void HandleRewardedAdClosed(object sender, EventArgs args)
    //{
    //    MonoBehaviour.print("HandleRewardedAdClosed event received");
    //    this.CreateAndLoadRewardedAd();
    //}

    //public void HandleUserEarnedReward(object sender, Reward args)
    //{
    //    WatchSuccess();
    //}

    private void WatchSuccess()
    {
        PlayerPrefs.SetInt("game_played", 0);
        PlayerPrefs.Save();
        actionWatchSuccess?.Invoke();
        SetBtGetMoreInteractable(false);
        if (centerOb != null)
            centerOb.SetActive(false);
    }

    public void ClickBack()
    {
        int gamePlayed = PlayerPrefs.GetInt("game_played", 0);
        gamePlayed++;
        PlayerPrefs.SetInt("game_played", gamePlayed);
        PlayerPrefs.Save();
        actionWatchFailed?.Invoke();
        if (centerOb != null)
            centerOb.SetActive(false);
    }

    private void SetBtGetMoreInteractable(bool interact)
    {
        if (btGetMore != null) btGetMore.interactable = interact;
        if (imgMask != null) imgMask.SetActive(!interact);
    }
}
