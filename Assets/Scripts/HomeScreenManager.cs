using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreenManager : MonoBehaviour
{
    public enum Food
    {
        FoodA, FoodB, FoodC
    }

    public enum Stat
    {
        Hunger, Boredom, Weight, Love, Stardom
    }    

    [SerializeField]
    GameObject coinzText = null;
    [SerializeField]
    GameObject ageText = null;
    [SerializeField]
    List<GameObject> panels = new List<GameObject>();

    [SerializeField]
    GameObject toggleSoundButtonText = null;
    [SerializeField]
    GameObject toggleMusicButtonText = null;
    [SerializeField]
    GameObject toggleAdsButtonText = null;

    [SerializeField]
    GameObject hungerStat = null;
    [SerializeField]
    GameObject boredomStat = null;
    [SerializeField]
    GameObject weightStat = null;
    [SerializeField]
    GameObject loveStat = null;

    [SerializeField]
    GameObject guessingGameButton = null;
    [SerializeField]
    GameObject chachaGameButton = null;

    [SerializeField]
    Canvas canvas = null;
    [SerializeField]
    GameObject doug = null;
    [SerializeField]
    GameObject textPopup = null;
    [SerializeField]
    GameObject imagePopup = null;

    [SerializeField]
    GameObject deactivationPanel = null;
    [SerializeField]
    GameObject disableInteractionPanel = null;


    private Vector3 dougSpriteDefaultScale = new Vector3(3f, 3f, 1f);
    private Vector3 dougSpriteDefaultPosition;
    private SaveData playerData;
    private bool deactivated = false;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Woof", 0.0f, 1.0f / GlobalConfig.incrementsPerSecond);
        InvokeRepeating("UpdateStats", GlobalConfig.depletionTickTime * 0.8f, GlobalConfig.depletionTickTime);
        InvokeRepeating("SaveGame", 60f, 60f * 5f);
        InvokeRepeating("CheckDeactivateDoug", 0f, 60f * 1f);
        doug.transform.localScale = new Vector3(dougSpriteDefaultScale.x * GetDougWeightScale(), dougSpriteDefaultScale.y, dougSpriteDefaultScale.z);
        dougSpriteDefaultPosition = doug.transform.position;
        GetComponent<AudioManager>().PlaySound("music");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStats()
    {
        if (!CheckDeactivateDoug())
        {
            if (playerData.playerData.hunger >= GlobalConfig.maxHunger)
                ModifyStat(Stat.Weight, -1, playerData.playerData);
            ModifyStat(Stat.Hunger, 1, playerData.playerData);
            ModifyStat(Stat.Boredom, 1, playerData.playerData);
            ModifyStat(Stat.Love, -1, playerData.playerData);
            ModifyStat(Stat.Stardom, -4, playerData.playerData);
        }
    }

    public void SaveGame()
    {
        SerializationManager.Save("save", playerData);
    }

    public void GoToClicker()
    {
        SceneManager.LoadScene("Clicker");
    }

    public void GoToGuessing()
    {
        Debug.Log("unlocked guessing: " + playerData.playerData.unlockedGuessing.ToString());
        if (playerData.playerData.unlockedGuessing)
            SceneManager.LoadScene("Clicker");
    }

    public void GoToChachaTrail()
    {
        Debug.Log("unlocked chacha: " + playerData.playerData.unlockedChachaTrail.ToString());
        if (playerData.playerData.unlockedChachaTrail)
            SceneManager.LoadScene("Clicker");
    }

    void OnEnable()
    {
        playerData = SerializationManager.Load("save") as SaveData;
        int minutes = (int) System.DateTime.Now.Subtract(playerData.playerData.lastDate).TotalMinutes;
        int updatesToDo = (int)(minutes / ((float)GlobalConfig.totalDepletionTimeInMinutes / GlobalConfig.maxHunger));
        for (int i = 0; i < updatesToDo; i++)
            UpdateStats();
        playerData.playerData.coinz += minutes * 60f * playerData.playerData.coinzPerSecond;
        coinzText.GetComponent<TextMeshProUGUI>().text = playerData.playerData.coinz.ToString("F1") + " coinz";
        UpdateText();
        UnlockMinigame(-1);
        UpdateBars();
        SaveGame();
        CheckDeactivateDoug();
    }

    void OnDisable()
    {
        playerData.playerData.lastDate = System.DateTime.Now;
        SerializationManager.Save("save", playerData);
    }

    public void EnablePanel(string panelName)
    {
        foreach(GameObject go in panels)
        {
            if (go.activeSelf)
            {
                go.GetComponent<PanelBounce>().Close();
                go.SetActive(false);
                if (go.transform.Find("Info Button/Info Panel") != null && go.transform.Find("Info Button/Info Panel").gameObject.activeSelf)
                    go.transform.Find("Info Button/Info Panel").gameObject.SetActive(false);
            }
        }

        if (panelName == "Stats Panel")
        {
            UpdateBars();
            ageText.GetComponent<TextMeshProUGUI>().text = GetAgeOfDoug();
        }

        foreach(GameObject go in panels)
        {
            if (go.name == panelName)
                go.SetActive(true);
        }
    }

    public void DisablePanel(string panelName)
    {
        foreach(GameObject go in panels)
        {
            if (go.name == panelName)
            {
                if (go.transform.Find("Info Button/Info Panel") != null && go.transform.Find("Info Button/Info Panel").gameObject.activeSelf)
                    go.transform.Find("Info Button/Info Panel").gameObject.SetActive(false);
                go.GetComponent<PanelBounce>().Close();
                go.SetActive(false);
                
            }
        }
    }

    public void ClearSave()
    {
        playerData.ResetPlayerData(false);
        PlayerPrefs.SetInt("soundEnabled", 1);
        PlayerPrefs.SetInt("musicEnabled", 1);
        PlayerPrefs.SetInt("adsEnabled", 1);
        SceneManager.LoadScene("MainMenu");
    }

    public void ToggleSound()
    {
        if (PlayerPrefs.GetInt("soundEnabled") == 1)
        {
            PlayerPrefs.SetInt("soundEnabled", 0);
            toggleSoundButtonText.GetComponent<TextMeshProUGUI>().text = "sound: off";
        }
        else
        {
            PlayerPrefs.SetInt("soundEnabled", 1);
            toggleSoundButtonText.GetComponent<TextMeshProUGUI>().text = "sound: on";
        }
    }

    public void ToggleMusic()
    {
        if (PlayerPrefs.GetInt("musicEnabled") == 1)
        {
            PlayerPrefs.SetInt("musicEnabled", 0);
            toggleMusicButtonText.GetComponent<TextMeshProUGUI>().text = "music: off";
            GetComponent<AudioManager>().StopMusic();
        }
        else
        {
            PlayerPrefs.SetInt("musicEnabled", 1);
            toggleMusicButtonText.GetComponent<TextMeshProUGUI>().text = "music: on";
            GetComponent<AudioManager>().PlaySound("music");
        }
    }

    public void ToggleAds()
    {
        MakePopup("this does nothing!");
        if (PlayerPrefs.GetInt("adsEnabled") == 1)
        {
            PlayerPrefs.SetInt("adsEnabled", 0);
            toggleAdsButtonText.GetComponent<TextMeshProUGUI>().text = "ads: off";
        }
        else
        {
            PlayerPrefs.SetInt("adsEnabled", 1);
            toggleAdsButtonText.GetComponent<TextMeshProUGUI>().text = "ads: on";
        }
    }

    public void UpdateText()
    {
        coinzText.GetComponent<TextMeshProUGUI>().text = playerData.playerData.coinz.ToString("F1") + " coinz";
        ageText.GetComponent<TextMeshProUGUI>().text = GetAgeOfDoug();
        if (PlayerPrefs.GetInt("soundEnabled") == 1)
            toggleSoundButtonText.GetComponent<TextMeshProUGUI>().text = "sound: on";
        else
            toggleSoundButtonText.GetComponent<TextMeshProUGUI>().text = "sound: off";
        if (PlayerPrefs.GetInt("musicEnabled") == 1)
            toggleMusicButtonText.GetComponent<TextMeshProUGUI>().text = "music: on";
        else
            toggleMusicButtonText.GetComponent<TextMeshProUGUI>().text = "music: off";
        if (PlayerPrefs.GetInt("adsEnabled") == 1)
            toggleAdsButtonText.GetComponent<TextMeshProUGUI>().text = "ads: on";
        else
            toggleAdsButtonText.GetComponent<TextMeshProUGUI>().text = "ads: off";
    }

    public void modifyBar(float amount, string text, GameObject stat)
    {
        stat.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        stat.transform.Find("Bar").Find("Filled Bar").GetComponent<Image>().fillAmount = amount;
    }

    public void MakePopup(string text)
    {
        GameObject popupInstance = Instantiate(textPopup);

        popupInstance.transform.SetParent(canvas.transform);
        popupInstance.GetComponent<TextMeshPro>().text = text;
        popupInstance.transform.localScale = textPopup.transform.localScale;
        popupInstance.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-5f, 5f), Random.Range(5f, 10f), 0f);

        Destroy(popupInstance, 2f);
    }

    public void MakePopupHeart()
    {
        GameObject popupInstance = Instantiate(imagePopup);

        popupInstance.transform.SetParent(canvas.transform);
        popupInstance.transform.localScale = imagePopup.transform.localScale;
        popupInstance.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-5f, 5f), Random.Range(5f, 10f), 0f);

        Destroy(popupInstance, 2f);
    }

    public void PetDoug()
    {
        GetComponent<AudioManager>().PlaySound("woof");
        if (Random.Range(0, 4) == 3)
        {
            MakePopupHeart();
            ModifyStat(Stat.Love, 1, playerData.playerData);
        }

        //Vector3 a = ;
        //LeanTween.scale(gameObject, originalScale * (1 - animationGrowth), animationSpeed * 2).setFrom(originalScale).setLoopPingPong().setEaseInOutSine();
        Random.InitState(System.DateTime.Now.Millisecond);
        LeanTween.move(doug, Random.insideUnitCircle * 0.5f, 0.75f).setFrom(dougSpriteDefaultPosition).setEasePunch();
    }

    public void UseFood(int food)
    {
        switch ((Food) food)
        {
            case Food.FoodA:
                ModifyStat(Stat.Hunger, -1, playerData.playerData);
                ModifyStat(Stat.Weight, 1, playerData.playerData);
                MakePopup("-1 hungy\n+1 weiht");
                break;
            case Food.FoodB:
                ModifyStat(Stat.Hunger, -2, playerData.playerData);
                ModifyStat(Stat.Weight, 2, playerData.playerData);
                ModifyStat(Stat.Love, 1, playerData.playerData);
                MakePopup("-2 hungy\n+2 weiht\n+1 luv");
                break;
            case Food.FoodC:
                ModifyStat(Stat.Hunger, -3, playerData.playerData);
                ModifyStat(Stat.Love, -1, playerData.playerData);
                MakePopup("-3 hungy\n-1 luv");
                break;
        }
    }

    public void UpdateBars()
    {
        modifyBar((float)playerData.playerData.hunger / GlobalConfig.maxHunger, "hungy: " + playerData.playerData.hunger.ToString() + "/" + GlobalConfig.maxHunger.ToString(), hungerStat);
        modifyBar((float)playerData.playerData.boredom / GlobalConfig.maxBoredom, "bord: " + playerData.playerData.boredom.ToString() + "/" + GlobalConfig.maxBoredom.ToString(), boredomStat);
        modifyBar((float)playerData.playerData.weight / (float)GlobalConfig.maxWeight, "weiht: " + playerData.playerData.weight.ToString() + "/" + GlobalConfig.maxWeight.ToString(), weightStat);
        modifyBar((float)playerData.playerData.love / GlobalConfig.maxLove, "luv: " + playerData.playerData.love.ToString() + "/" + GlobalConfig.maxLove.ToString(), loveStat);
        doug.transform.localScale = new Vector3(dougSpriteDefaultScale.x * GetDougWeightScale(), dougSpriteDefaultScale.y, dougSpriteDefaultScale.z);
    }

    public static void ModifyStat(Stat stat, int amount, PlayerData saveData)
    {
        switch (stat)
        {
            case Stat.Hunger:
                if (saveData.hunger + amount > GlobalConfig.maxHunger)
                    saveData.hunger = GlobalConfig.maxHunger;
                else if (saveData.hunger + amount < 0)
                    saveData.hunger = 0;
                else saveData.hunger += amount;
                break;
            case Stat.Boredom:
                if (saveData.boredom + amount > GlobalConfig.maxBoredom)
                    saveData.boredom = GlobalConfig.maxBoredom;
                else if (saveData.boredom + amount < 0)
                    saveData.boredom = 0;
                else saveData.boredom += amount;
                break;
            case Stat.Weight:
                if (saveData.weight + amount > GlobalConfig.maxWeight)
                    saveData.weight = GlobalConfig.maxWeight;
                else if (saveData.weight + amount < 0)
                    saveData.weight = 0;
                else saveData.weight += amount;
                break;
            case Stat.Love:
                if (saveData.love + amount > GlobalConfig.maxLove)
                    saveData.love = GlobalConfig.maxLove;
                else if (saveData.love + amount < 0)
                    saveData.love = 0;
                else saveData.love += amount;
                break;
            case Stat.Stardom:
                if (saveData.stardomBonus + amount < 0)
                    saveData.stardomBonus = 0;
                else saveData.stardomBonus += amount;
                break;
        }
    }

    public float GetDougWeightScale()
    {
        
        float weight = (float)playerData.playerData.weight / (float)GlobalConfig.maxWeight;
        if (weight < 0.25)
        {
            return (weight * 2f) + .5f;
        }
        else if (weight > 0.75)
        {
            return (weight * 8f) - 5f;
        }
        else
        {
            return 1f;
        }
    }

    public string GetAgeOfDoug()
    {
        System.TimeSpan age = System.DateTime.Now - playerData.playerData.acquisitionDate;
        if (age.Hours < 1)
            return age.Minutes.ToString() + (age.Minutes == 1 ? " minute old" : " minutes old");
        else if (age.Days < 1)
            return age.Hours.ToString() + (age.Hours == 1 ? " hour, " : " hours, ") + age.Minutes.ToString() + (age.Minutes == 1 ? " minute old" : " minutes old");
        else
            return age.Hours.ToString() + (age.Days == 1 ? " day, " : " days, ") + age.Hours.ToString() + (age.Hours == 1 ? " hour old" : " hours old");
    }

    public void UnlockMinigame(int i)
    {
        if (i == 2)
        {
            playerData.playerData.unlockedGuessing = true;
            UpdateText();
            SaveGame();
            Debug.Log("unlocked guessing: " + playerData.playerData.unlockedGuessing.ToString());
        }
        else if (i == 3 && playerData.playerData.unlockedGuessing)
        {
            playerData.playerData.unlockedChachaTrail = true;
            UpdateText();
            SaveGame();
            Debug.Log("unlocked chacha: " + playerData.playerData.unlockedChachaTrail.ToString());
        }

        if (!playerData.playerData.unlockedGuessing)
        {
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
            guessingGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
        }
        else
        {
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.textColor;
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "find the bisco";
            guessingGameButton.transform.Find("Unlock Text").gameObject.SetActive(false);
        }
        if (!playerData.playerData.unlockedChachaTrail)
        {
            chachaGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
            if (playerData.playerData.unlockedGuessing)
                chachaGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
        }
        else
        {
            chachaGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.textColor;
            chachaGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "chacha trail";
            chachaGameButton.transform.Find("Unlock Text").gameObject.SetActive(false);

        }
    }

    public void TempDecreaseWeight()
    {
        ModifyStat(Stat.Weight, -5, playerData.playerData);
    }

    public bool CheckDeactivateDoug()
    {
        if (playerData.playerData.weight <= 0 || playerData.playerData.boredom >= GlobalConfig.maxBoredom)
        {
            if (!deactivated)
            {
                bool deathByWeight = playerData.playerData.weight <= 0;
                deactivated = true;
                StartCoroutine(DeactivateDoug(deathByWeight));
                CheckDeactivateDoug();
            }
            return true;
        }
        else
        {
            doug.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            Debug.Log("activated!");
            return false;
        }
    }

    IEnumerator DeactivateDoug(bool deathByWeight)
    {
        doug.transform.eulerAngles = new Vector3(0f, 0f, 180f);
        Debug.Log("deactivated!");
        disableInteractionPanel.SetActive(true);
        string t = deactivationPanel.transform.Find("Deactivation Layout/Deactivation Text").GetComponent<TextMeshProUGUI>().text.Replace("{0}", (deathByWeight) ? "starvation" : "boredom")
        .Replace("{1}", Random.Range(int.Parse("100000", System.Globalization.NumberStyles.HexNumber), int.Parse("1000000", System.Globalization.NumberStyles.HexNumber)).ToString("X"));
        deactivationPanel.transform.Find("Deactivation Layout/Deactivation Text").GetComponent<TextMeshProUGUI>().text = t;
        yield return new WaitForSeconds(3.5f);
        disableInteractionPanel.SetActive(false);
        deactivationPanel.SetActive(true);
        playerData.ResetPlayerData(true);
        SaveGame();
    }

    public void StartRemoveDougCoroutine()
    {
        StartCoroutine(RemoveDoug());
    }

    public IEnumerator RemoveDoug()
    {
        LeanTween.moveLocalX(doug, 1500f, 1.2f).setEaseInQuart();
        yield return new WaitForSeconds(1.5f);
        doug.transform.localPosition = new Vector3(-1500f, 0f, 0f);
        UpdateBars();
        doug.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        LeanTween.moveLocalX(doug, 0, 1.2f).setEaseOutQuart();
    }


    //TODO: separate this from a scene-tied script?
    public void IncrementCoinz(float dCoinz)
    {
        playerData.playerData.coinz += dCoinz;
        coinzText.GetComponent<TextMeshProUGUI>().text = playerData.playerData.coinz.ToString("F1") + " coinz";
    }

    public void Woof()
    {
        IncrementCoinz(playerData.playerData.coinzPerSecond / GlobalConfig.incrementsPerSecond);
    }

}
