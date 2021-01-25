using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

[RequireComponent (typeof (Button))]
public class RewardedAdsButton : MonoBehaviour, IUnityAdsListener {

    #if UNITY_IOS
    private string gameId = "1486551";
    #elif UNITY_ANDROID
    private string gameId = "1486550";
    #endif

    UnityEngine.UI.Button myButton;
    public string myPlacementId = "rewardedVideo";
    bool rewardReady = false;

    void Start () {   
        myButton = GetComponent <UnityEngine.UI.Button> ();

        // Set interactivity to be dependent on the Placement’s status:
        myButton.interactable = Advertisement.IsReady (myPlacementId); 

        // Map the ShowRewardedVideo function to the button’s click listener:
        if (myButton) myButton.onClick.AddListener (ShowRewardedVideo);

        // Initialize the Ads listener and service:
        Advertisement.AddListener (this);
        Advertisement.Initialize (gameId, true);
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo () {
        Advertisement.Show (myPlacementId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady (string placementId) {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId) {        
            myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        if (rewardReady)
        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished || showResult == ShowResult.Skipped) {
                PersistentGameManager.instance.ModifyStat(Stat.Boredom, -40);
                PersistentGameManager.instance.ModifyStat(Stat.Love, 40);
                PersistentGameManager.instance.MakePopup("-40 bord\n+40 luv");
                PersistentGameManager.instance.audioManager.PlaySound("wee");
            } else if (showResult == ShowResult.Failed) {
                PersistentGameManager.instance.ModifyStat(Stat.Boredom, -15);
                PersistentGameManager.instance.ModifyStat(Stat.Love, 15);
                PersistentGameManager.instance.MakePopup("-15 bord\n+15 luv");
                PersistentGameManager.instance.audioManager.PlaySound("wee");
                Debug.LogWarning("The ad did not finish due to an error.");
            }
            rewardReady = false;
        }
    }

    public void OnUnityAdsDidError (string message) {
        // Log the error.
    }

    public void OnUnityAdsDidStart (string placementId) {
        rewardReady = true;
        // Optional actions to take when the end-users triggers an ad.
    } 
}