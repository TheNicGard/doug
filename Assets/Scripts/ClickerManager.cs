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

    private SaveData playerData = null;
    private ClickerData[] videos = null;
    private string[] soundNames = new string[] {"woof", "woof", "arf", "bark", "bork" };
    private int lastClickedVideo = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementCoinz(float dCoinz, bool useStardom)
    {
        if (useStardom)
            playerData.playerData.coinz += (1f + (playerData.playerData.stardomBonus / 100f)) * dCoinz;
        else
            playerData.playerData.coinz += dCoinz;
        coinzText.GetComponent<TextMeshProUGUI>().text = ConvertToShortNumber(playerData.playerData.coinz) + " coinz";
    }

    public void Woof()
    {
        IncrementCoinz(playerData.playerData.coinzPerSecond / GlobalConfig.incrementsPerSecond, true);
        UpdateTextColors();
    }

    public void PressDoug()
    {
        IncrementCoinz(1f, true);
        particles.Emit(1);
        GetComponent<AudioManager>().PlayRandomSound(soundNames);

        Random.InitState(System.DateTime.Now.Millisecond);
        if(Random.Range(0, 5) == 0)
        {
            playerData.playerData.stardomBonus += 1;
            UpdateText();
        }
    }

    public long requiredCoinz(int videoNumber)
    {
        return (long) Mathf.Ceil(videos[videoNumber].requiredCoinz * Mathf.Pow(1.15f, playerData.playerData.clickerVideos[videoNumber]));
    }

    public void AddVideo(int videoNumber)
    {
        long tempRequiredCoinz = requiredCoinz(videoNumber);
        
        if (playerData.playerData.coinz >= tempRequiredCoinz && playerData.playerData.clickerVideos[videoNumber] <= 9000)
        {
            IncrementCoinz(-tempRequiredCoinz, false);
            playerData.playerData.clickerVideos[videoNumber]++;
            playerData.playerData.coinzPerSecond += videos[videoNumber].coinzPerSecond;
            UpdateText();
            Debug.Log("bought " + videoNumber.ToString() + " for " + tempRequiredCoinz.ToString() + " coinz!");
        }
    }

    public void AssignVideoButtons(int videoNumber)
    {
        Debug.Log("clicked on " + videoNumber.ToString() + "!");
        lastClickedVideo = videoNumber;
        UpdateTextColors();
        videoButtons.transform.Find("Add Video Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            "upload \"" + videos[videoNumber].title + "\":\n" + ConvertToShortNumber(requiredCoinz(videoNumber)) + " coinz";
        
        videoButtons.transform.Find("Add Video Button").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();

        videoButtons.transform.Find("Add Video Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {AddVideo(videoNumber);});
        videoButtons.transform.Find("Add Video Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {gameObject.GetComponent<AudioManager>().PlaySound("click");});

        videoButtons.transform.Find("Buy Comment 1 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "buy comment:\n1 coinz";
        videoButtons.transform.Find("Buy Comment 2 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "buy comment:\n1 coinz";
        videoButtons.transform.Find("Buy Comment 3 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "buy comment:\n1 coinz";
    }

    public void UpdateText()
    {
        coinzPerSecondText.GetComponent<TextMeshProUGUI>().text =
            ((1 + (playerData.playerData.stardomBonus / 100f)) * playerData.playerData.coinzPerSecond).ToString("F1") + " subscribers";
            
        for (int i = 0; i < videos.Length; i++) 
        {
            ModifyButton(buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject, "Count Text", playerData.playerData.clickerVideos[i].ToString());
            ModifyButton(buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject, "Cost Text", videos[i].coinzPerSecond.ToString() + " coinz/sec");
        }

        if (playerData.playerData.stardomBonus > 0)
        {
            stardomCounter.SetActive(true);
            stardomCounter.GetComponent<TextMeshProUGUI>().text = playerData.playerData.stardomBonus + "% stardum power";
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
            buttonScrollRect.transform.Find("Button " + i.ToString()).gameObject.transform.Find("Cost Text").gameObject.GetComponent<TextMeshProUGUI>().color = 
                (playerData.playerData.coinz > requiredCoinz(i)) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;

        videoButtons.transform.Find("Add Video Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color =
            (playerData.playerData.coinz > requiredCoinz(lastClickedVideo)) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
        videoButtons.transform.Find("Buy Comment 1 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = 
            (playerData.playerData.coinz > requiredCoinz(lastClickedVideo)) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
        videoButtons.transform.Find("Buy Comment 2 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = 
            (playerData.playerData.coinz > requiredCoinz(lastClickedVideo)) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
        videoButtons.transform.Find("Buy Comment 3 Button").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = 
            (playerData.playerData.coinz > requiredCoinz(lastClickedVideo)) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
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
        InvokeRepeating("DepleteStardom", 15f, 15f * 1f);
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
            popupInstance.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {AssignVideoButtons(value);});
            popupInstance.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {GetComponent<AudioManager>().PlaySound("click");});
        }
    }

    public void UpdateStats()
    {
        if (playerData.playerData.hunger <= 0)
             HomeScreenManager.ModifyStat( HomeScreenManager.Stat.Weight, -1, playerData.playerData);
         HomeScreenManager.ModifyStat( HomeScreenManager.Stat.Hunger, 1, playerData.playerData);
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Boredom, -2, playerData.playerData);
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Weight, -1, playerData.playerData);
        SerializationManager.Save("save", playerData);
    }

    public void DepleteStardom()
    {
        HomeScreenManager.ModifyStat(HomeScreenManager.Stat.Stardom, -1, playerData.playerData);
        UpdateText();
    }

    void ModifyButton(GameObject button, string component, string text)
    {
        //if (component == "Cost Text")
        //    text = text + " coinz needed";
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