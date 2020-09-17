﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickerManager : MonoBehaviour
{
    [System.Serializable]
    public class ClickerData
    {
        public string title;
        public float requiredCoinz;
        public float coinzPerSecond;
    }

    [SerializeField]
    GameObject coinzText = null;
    [SerializeField]
    GameObject coinzPerSecondText = null;
    [SerializeField]
    GameObject stardomCounter = null;
    [SerializeField]
    GameObject buttonScrollRect = null;
    [SerializeField]
    GameObject videoButtons = null;
    [SerializeField]
    GameObject buttonPrefab = null;
    [SerializeField]
    ParticleSystem particles = null;
    [SerializeField]
    TextAsset clickerData = null;

    private ClickerData[] videos = null;
    private string[] soundNames = new string[] {"woof", "woof", "arf", "bark", "bork" };
    private int lastClickedVideo = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioManager>().PlaySound("music");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float CoinzPerSecond(bool useStardom)
    {
        float coinzPerSecond = 0f;
        for (int i = 0; i < videos.Length; i++)
            coinzPerSecond += videos[i].coinzPerSecond * 
            PersistentGameManager.instance.playerData.playerData.clickerVideos[i] * 
            PersistentGameManager.instance.playerData.playerData.clickerVideosComments[i];
        if (useStardom)
            coinzPerSecond *= (1f + (PersistentGameManager.instance.playerData.playerData.stardomBonus / 100f));
        return coinzPerSecond;
    }

    public void IncrementCoinz(float dCoinz)
    {
        PersistentGameManager.instance.playerData.playerData.coinz += dCoinz;
        coinzText.GetComponent<TextMeshProUGUI>().text = ConvertToShortNumber(PersistentGameManager.instance.playerData.playerData.coinz) + " coinz";
    }

    public void Woof()
    {
        IncrementCoinz(CoinzPerSecond(true) / GlobalConfig.incrementsPerSecond);
        UpdateTextColors();
    }

    public void PressDoug()
    {
        IncrementCoinz(1f);
        particles.Emit(1);
        GetComponent<AudioManager>().PlayRandomSound(soundNames);

        Random.InitState(System.DateTime.Now.Millisecond);
        if(Random.Range(0, (int) (1 / GlobalConfig.stardomChance)) == 0)
        {
            PersistentGameManager.instance.playerData.playerData.stardomBonus += 1;
            UpdateText();
        }
    }

    public long requiredCoinz(int videoNumber)
    {
        return (long) Mathf.Ceil(videos[videoNumber].requiredCoinz * Mathf.Pow(1.15f, PersistentGameManager.instance.playerData.playerData.clickerVideos[videoNumber]));
    }

    public void AddVideo(int videoNumber)
    {
        long tempRequiredCoinz = requiredCoinz(videoNumber);
        
        if (PersistentGameManager.instance.playerData.playerData.coinz >= tempRequiredCoinz && PersistentGameManager.instance.playerData.playerData.clickerVideos[videoNumber] <= 9000)
        {
            IncrementCoinz(-tempRequiredCoinz);
            PersistentGameManager.instance.playerData.playerData.clickerVideos[videoNumber]++;
            UpdateText();
        }
    }

    public long CommentCost(int videoNumber, int commentNumber)
    {
        return (long) videos[videoNumber].requiredCoinz * 500 * (commentNumber + 1);
    }

    public void BuyComment(int videoNumber, int commentNumber) //commentNumber is the comment that is being bought
    {
        if (PersistentGameManager.instance.playerData.playerData.coinz >= CommentCost(videoNumber, commentNumber))
        {
            if ((commentNumber == 0 && PersistentGameManager.instance.playerData.playerData.clickerVideosComments[videoNumber] == 1) ||
                (commentNumber == 1 && PersistentGameManager.instance.playerData.playerData.clickerVideosComments[videoNumber] == 2) ||
                (commentNumber == 2 && PersistentGameManager.instance.playerData.playerData.clickerVideosComments[videoNumber] == 4))
            {
                IncrementCoinz(-CommentCost(videoNumber, commentNumber));
                PersistentGameManager.instance.playerData.playerData.clickerVideosComments[videoNumber] *= 2;
                AssignVideoButtons(videoNumber);
                UpdateText();
            }
        }
    }

    public void AssignVideoButtons(int videoNumber)
    {
        lastClickedVideo = videoNumber;
        UpdateTextColors();
        videoButtons.transform.Find("Add Video Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            "uplaod \"" + videos[videoNumber].title + "\":\n" + ConvertToShortNumber(requiredCoinz(videoNumber)) + " coinz";
        
        videoButtons.transform.Find("Add Video Button").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();

        videoButtons.transform.Find("Add Video Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {AddVideo(videoNumber);});
        videoButtons.transform.Find("Add Video Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {gameObject.GetComponent<AudioManager>().PlaySound("click");});

        videoButtons.transform.Find("Buy Comment 1 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            (PersistentGameManager.instance.playerData.playerData.clickerVideosComments[videoNumber] >= 2) ? "(x2 cps) wow funny dog" : "buy comment:\n" + ConvertToShortNumber(CommentCost(videoNumber, 0)) + " coinz";
        videoButtons.transform.Find("Buy Comment 2 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            (PersistentGameManager.instance.playerData.playerData.clickerVideosComments[videoNumber] >= 4) ? "(x2 cps) wow funny dog" : "buy comment:\n" + ConvertToShortNumber(CommentCost(videoNumber, 1)) + " coinz";
        videoButtons.transform.Find("Buy Comment 3 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            (PersistentGameManager.instance.playerData.playerData.clickerVideosComments[videoNumber] >= 8) ? "(x2 cps) wow funny dog" : "buy comment:\n" + ConvertToShortNumber(CommentCost(videoNumber, 2)) + " coinz";

        for (int i = 0; i < 3; i++)
        {
            int temp = i;
            videoButtons.transform.Find("Buy Comment " + (temp + 1).ToString() + " Button").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            videoButtons.transform.Find("Buy Comment " + (temp + 1).ToString() + " Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {BuyComment(videoNumber, temp);});
            videoButtons.transform.Find("Buy Comment " + (temp + 1).ToString() + " Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {gameObject.GetComponent<AudioManager>().PlaySound("click");});
        }
    }

    public void UpdateText()
    {
        coinzPerSecondText.GetComponent<TextMeshProUGUI>().text =
            ((1 + (PersistentGameManager.instance.playerData.playerData.stardomBonus / 100f)) * CoinzPerSecond(true)).ToString("F1") + " subscribers";
            
        for (int i = 0; i < videos.Length; i++) 
        {
            ModifyButton(buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject, "Count Text", PersistentGameManager.instance.playerData.playerData.clickerVideos[i].ToString());
            ModifyButton(buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject, "Cost Text", "+" + ConvertToShortNumber(videos[i].coinzPerSecond).ToString() + " subscribers");
        }

        if (PersistentGameManager.instance.playerData.playerData.stardomBonus > 0)
        {
            stardomCounter.SetActive(true);
            stardomCounter.GetComponent<TextMeshProUGUI>().text = PersistentGameManager.instance.playerData.playerData.stardomBonus + "% stardum power";
        }
        else
        {
            stardomCounter.SetActive(false);
        }

        AssignVideoButtons(lastClickedVideo);
    }

    public void UpdateTextColors()
    {
        for (int i = 0; i < videos.Length; i++)
            buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject.transform.Find("Name Text").gameObject.GetComponent<TextMeshProUGUI>().color = 
                (PersistentGameManager.instance.playerData.playerData.coinz > requiredCoinz(i)) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;

        videoButtons.transform.Find("Add Video Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color =
            (PersistentGameManager.instance.playerData.playerData.coinz > requiredCoinz(lastClickedVideo)) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;

        videoButtons.transform.Find("Buy Comment 1 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = 
            (PersistentGameManager.instance.playerData.playerData.clickerVideosComments[lastClickedVideo] >= 2 || PersistentGameManager.instance.playerData.playerData.coinz > CommentCost(lastClickedVideo, 0))
                ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
        videoButtons.transform.Find("Buy Comment 2 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = 
            (PersistentGameManager.instance.playerData.playerData.clickerVideosComments[lastClickedVideo] >= 4 || PersistentGameManager.instance.playerData.playerData.coinz > CommentCost(lastClickedVideo, 1))
                ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
        videoButtons.transform.Find("Buy Comment 3 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = 
            PersistentGameManager.instance.playerData.playerData.clickerVideosComments[lastClickedVideo] >= 8 || (PersistentGameManager.instance.playerData.playerData.coinz > CommentCost(lastClickedVideo, 2))
                ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
    }

    public void GoToHomeScreen()
    {
        PersistentGameManager.instance.SwitchScene((int) SceneIndexes.HOME_SCREEN);
    }

    void OnEnable()
    {
        LoadClickerData();
        InvokeRepeating("Woof", 0.0f, 1.0f / GlobalConfig.incrementsPerSecond);
        InvokeRepeating("DepleteStardom", 15f, 15f * 1f);
        UpdateText();
    }

    void OnDisable()
    {
        PersistentGameManager.instance.playerData.playerData.coinzPerSecond = CoinzPerSecond(false);
        PersistentGameManager.instance.SaveGame();
    }

    public void LoadClickerData()
    {
        videos = JsonHelper.FromJson<ClickerData>(clickerData.text);

        for (int i = 0; i < videos.Length; i++)
        {
            int value = i;
            GameObject popupInstance = Instantiate(buttonPrefab);
            popupInstance.name = "Button " + i.ToString();
            popupInstance.transform.SetParent(buttonScrollRect.transform);
            popupInstance.transform.localScale = buttonPrefab.transform.localScale;
            LoadButton(popupInstance, videos[i]);
            popupInstance.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {AssignVideoButtons(value);});
            popupInstance.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {GetComponent<AudioManager>().PlaySound("click");});
        }
    }

    public void DepleteStardom()
    {
        PersistentGameManager.instance.ModifyStat(Stat.Stardom, -1);
        UpdateText();
    }

    void ModifyButton(GameObject button, string component, string text)
    {
        button.transform.Find(component).gameObject.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    void LoadButton(GameObject button, ClickerData video)
    {
        ModifyButton(button, "Name Text", video.title);
        ModifyButton(button, "Cost Text", video.coinzPerSecond.ToString() + " coinz/sec");
    }

    string ConvertToShortNumber(long number)
    {
        if (number < 1000 * 1000)
            return number.ToString("N0");
        else if (number < 1000 * 1000 * 1000)
            return (number / (1000 * 1000)).ToString("0.##") + " million";
        else if (number < (long) 1000 * 1000 * 1000 * 1000)
            return (number / (1000 * 1000 * 1000)).ToString("0.##") + " billion";
        else if (number < (long) 1000 * 1000 * 1000 * 1000 * 1000)
            return (number / ((long) 1000 * 1000 * 1000 * 1000)).ToString("0.##") + " trillion";
        else
            return number.ToString("E2");
    }

    string ConvertToShortNumber(double number)
    {
        if (number < 1)
            return number.ToString("N1");
        if (number < 1000 * 1000)
            return number.ToString("N0");
        else if (number < 1000 * 1000 * 1000)
            return (number / (1000 * 1000)).ToString("0.##") + " million";
        else if (number < (double) 1000 * 1000 * 1000 * 1000)
            return (number / (1000 * 1000 * 1000)).ToString("0.##") + " billion";
        else if (number < (double) 1000 * 1000 * 1000 * 1000 * 1000)
            return (number / ((double) 1000 * 1000 * 1000 * 1000)).ToString("0.##") + " trillion";
        else
            return number.ToString("E2");
    }
}

public static class JsonHelper
{
    //thanks to: https://stackoverflow.com/a/36244111/4200551
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items = null;
    }

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
}