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

    [SerializeField]
    GameObject coinzText = null;
    [SerializeField]
    GameObject coinzPerSecondText = null;
    [SerializeField]
    GameObject buttonScrollRect = null;
    [SerializeField]
    GameObject buttonPrefab = null;
    [SerializeField]
    ParticleSystem particles = null;
    [SerializeField]
    TextAsset clickerData = null;

    private SaveData playerData = null;
    private ClickerData[] videos = null;
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
        coinzText.GetComponent<TextMeshProUGUI>().text = ConvertToShortNumber(playerData.playerData.coinz) + " coinz";
    }

    public void Woof()
    {
        IncrementCoinz(playerData.playerData.coinzPerSecond / GlobalConfig.incrementsPerSecond);
        for (int i = 0; i < videos.Length; i++)
            buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject.transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().color = 
                (playerData.playerData.coinz > videos[i].requiredCoinz) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
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
            ModifyButton(buttonScrollRect.transform.Find("Button " + videoNumber.ToString()).gameObject, "Count Text", playerData.playerData.clickerVideos[videoNumber].ToString());
            UpdateText();
        }
    }

    public void UpdateText()
    {
        coinzPerSecondText.GetComponent<TextMeshProUGUI>().text = playerData.playerData.coinzPerSecond.ToString("F1") + " subscribers";
        //coinzText.GetComponent<TextMeshProUGUI>().text = ConvertToShortNumber(playerData.playerData.coinz) + " coinz"; //playerData.playerData.coinz.ToString("F1") + " views";
        for (int i = 0; i < videos.Length; i++)
            ModifyButton(buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject, "Count Text", playerData.playerData.clickerVideos[i].ToString());
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
        videos = JsonHelper.FromJson<ClickerData>(clickerData.text);

        for (int i = 0; i < videos.Length; i++)
        {
            int value = i;
            GameObject popupInstance = Instantiate(buttonPrefab);
            popupInstance.name = "Button " + i.ToString();
            popupInstance.transform.SetParent(buttonScrollRect.transform);
            popupInstance.transform.localScale = buttonPrefab.transform.localScale;
            LoadButton(popupInstance, videos[i]);
            popupInstance.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {AddVideo(value);});
        }
    }

    public void UpdateStats()
    {
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Boredom, -2, playerData.playerData);
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Weight, -1, playerData.playerData);
        SerializationManager.Save("save", playerData);
    }

    void ModifyButton(GameObject button, string component, string text)
    {
        if (component == "Cost Text")
            text = text + " coinz needed";
        button.transform.Find(component).gameObject.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    void LoadButton(GameObject button, ClickerData video)
    {
        ModifyButton(button, "Name Text", video.title);
        ModifyButton(button, "Cost Text", ConvertToShortNumber(video.requiredCoinz));
    }

    string ConvertToShortNumber(long number)
    {
        if (number < 1000 * 1000)
            return number.ToString("N0");
        else if (number < 1000 * 1000 * 1000)
            return (number / (1000 * 1000)).ToString("0.##") + " million";
        else if (number < (long) 1000 * 1000 * 1000 * 1000)
            return (number / (1000 * 1000 * 1000)).ToString("0.##") + " billion";
        else
            return number.ToString("E2");
    }

    string ConvertToShortNumber(double number)
    {
        if (number < 1000 * 1000)
            return number.ToString("N0");
        else if (number < 1000 * 1000 * 1000)
            return (number / (1000 * 1000)).ToString("0.##") + " million";
        else if (number < (double) 1000 * 1000 * 1000 * 1000)
            return (number / (1000 * 1000 * 1000)).ToString("0.##") + " billion";
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