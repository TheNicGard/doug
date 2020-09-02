using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private SaveData playerData;
    private ClickerData[] videos;

    public GameObject coinzText;
    public GameObject coinzPerSecondText;
    public GameObject buttonA;
    public GameObject buttonB;
    public GameObject buttonC;
    public GameObject buttonD;
    public ParticleSystem particles;
    public TextAsset clickerData;

    private string[] soundNames = new string[] {"woof", "woof", "arf", "bark", "bork" };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementCoinz(float dCoinz)
    {
        playerData.playerData.coinz += dCoinz;
        coinzText.GetComponent<TextMeshProUGUI>().text = playerData.playerData.coinz.ToString("F1") + " coinz";
    }

    public void Woof()
    {
        IncrementCoinz(playerData.playerData.coinzPerSecond / GlobalConfig.incrementsPerSecond);
    }

    public void PressDoug()
    {
        IncrementCoinz(1f);
        particles.Emit(1);
        GetComponent<AudioManager>().PlayRandomSound(soundNames);
        
    }

    public void AddVideo(int videoNumber)
    {
        if (playerData.playerData.coinz >= videos[videoNumber].requiredCoinz && playerData.playerData.clickerVideos[videoNumber] <= 9000)
        {
            IncrementCoinz(-(videos[videoNumber].requiredCoinz));
            playerData.playerData.clickerVideos[videoNumber]++;
            playerData.playerData.coinzPerSecond += videos[videoNumber].coinzPerSecond;
            UpdateText();
        }
    }

    public void UpdateText()
    {
        // TODO: change to button array
        ModifyButton(buttonA, "Count Text", playerData.playerData.clickerVideos[0].ToString());
        ModifyButton(buttonB, "Count Text", playerData.playerData.clickerVideos[1].ToString());
        ModifyButton(buttonC, "Count Text", playerData.playerData.clickerVideos[2].ToString());
        ModifyButton(buttonD, "Count Text", playerData.playerData.clickerVideos[3].ToString());
        coinzPerSecondText.GetComponent<TextMeshProUGUI>().text = playerData.playerData.coinzPerSecond.ToString("F1") + " subscribers";
        coinzText.GetComponent<TextMeshProUGUI>().text = playerData.playerData.coinz.ToString("F1") + " views";  
    }

    public void GoToHomeScreen()
    {
        SceneManager.LoadScene("HomeScreen");
    }

    void OnEnable()
    {
        LoadClickerData();
        playerData = SerializationManager.Load("save") as SaveData;
        InvokeRepeating("Woof", 0.0f, 1.0f / GlobalConfig.incrementsPerSecond);
        InvokeRepeating("UpdateStats", 60f, 60f * 1f);
        UpdateText();
    }

    void OnDisable()
    {
        playerData.playerData.lastDate = System.DateTime.Now;
        SerializationManager.Save("save", playerData);
    }

    public void LoadClickerData()
    {
        string json = clickerData.text;
        videos = JsonHelper.FromJson<ClickerData>(json);

        ModifyButton(buttonA, "Name Text", videos[0].title);
        ModifyButton(buttonB, "Name Text", videos[1].title);
        ModifyButton(buttonC, "Name Text", videos[2].title);
        ModifyButton(buttonD, "Name Text", videos[3].title);
    }

    public void UpdateStats()
    {
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Boredom, -2, playerData.playerData);
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Weight, -1, playerData.playerData);
        SerializationManager.Save("save", playerData);
    }

    void ModifyButton(GameObject button, string component, string text)
    {
        button.transform.Find(component).gameObject.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    void LoadButton(GameObject button, ClickerData video)
    {
        ModifyButton(button, "Name Text", video.title);
        ModifyButton(button, "Cost Text", video.requiredCoinz.ToString());
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