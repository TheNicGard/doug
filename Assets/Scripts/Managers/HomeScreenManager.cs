using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class HomeScreenManager : MonoBehaviour
{
    public enum Food
    {
        FoodA, FoodB, FoodC
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
    GameObject flippyGameButton = null;

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
    [SerializeField]
    GameObject unlockPanel = null;
    [SerializeField]
    GameObject buyText = null;
    [SerializeField]
    List<GameObject> wallpaperButtons = new List<GameObject>();

    private Vector3 dougSpriteDefaultScale = new Vector3(3f, 3f, 1f);
    private Vector3 dougSpriteDefaultPosition;
    private bool deactivated = false;
    private string[] uselessStats = {"charisma", "sassiness", "boldness", "reading", "flame resistance", "sprinting speed", "moxie", "special", "bagel", "patience", "linguistic ability"};

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckDeactivateDoug", 0f, 60f * 1f);
        doug.transform.localScale = new Vector3(dougSpriteDefaultScale.x * GetDougWeightScale(), dougSpriteDefaultScale.y, dougSpriteDefaultScale.z);
        dougSpriteDefaultPosition = doug.transform.position;

        if (PersistentGameManager.instance != null)
        {
            PersistentGameManager.instance.SaveGame();
            coinzText.GetComponent<TextMeshProUGUI>().text = ClickerManager.ConvertToShortNumber(PersistentGameManager.instance.playerData.playerData.coinz) + " coinz";
            PersistentGameManager.instance.LoadWallpaperButtons();   
            UpdateText();
            UpdateMinigameText();
            UpdateBars();
            CheckDeactivateDoug();
            UpdateWallpaperButtons();
        }
    }

    // Update is called once per frame
    void Update()
    {
        coinzText.GetComponent<TextMeshProUGUI>().text = ClickerManager.ConvertToShortNumber(PersistentGameManager.instance.playerData.playerData.coinz) + " coinz";
    }

    public void GoToScene(string scene_name)
    {
        switch (scene_name)
        {
            case "Clicker":
                PersistentGameManager.instance.SwitchScene((int) SceneIndexes.CLICKER);
                break;
            case "Guessing":
                if (PersistentGameManager.instance.playerData.playerData.unlockedGuessing)
                    PersistentGameManager.instance.SwitchScene((int) SceneIndexes.GUESSING);
                else
                    OpenUnlockPanel();
                break;
            case "Flippy":
                if (PersistentGameManager.instance.playerData.playerData.unlockedFlippy)
                    PersistentGameManager.instance.SwitchScene((int) SceneIndexes.FLIPPY);
                else if (PersistentGameManager.instance.playerData.playerData.unlockedGuessing)
                    OpenUnlockPanel();
                break;
        }
    }

    public void unlockWallpaper()
    {
        WallpaperNum currentWallpaper = PersistentGameManager.instance.playerData.playerData.unlockedWallpaper;
        float nextCost = (currentWallpaper < WallpaperNumMethods.getMaxPurchasableWallpaperNum()) ? GlobalConfig.wallpaperCosts[currentWallpaper + 1] : 0;

        if (currentWallpaper != WallpaperNumMethods.getMaxPurchasableWallpaperNum() && PersistentGameManager.instance.playerData.playerData.coinz >= nextCost) 
        {
            PersistentGameManager.instance.playerData.playerData.unlockedWallpaper++;
            PersistentGameManager.instance.playerData.playerData.coinz -= nextCost;
            PersistentGameManager.instance.SaveGame();
            PersistentGameManager.instance.playerData.playerData.currentWallpaper = PersistentGameManager.instance.playerData.playerData.unlockedWallpaper;
            PersistentGameManager.instance.ChangeWallpaper((int) PersistentGameManager.instance.playerData.playerData.currentWallpaper);
            MakePopup("wallpaper unlocked!");
            DisablePanel("Store Panel");
            UpdateWallpaperButtons();
        }
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

        if (panelName == "Store Panel")
        {
            WallpaperNum currentWallpaper = PersistentGameManager.instance.playerData.playerData.unlockedWallpaper;
            float nextCost = (currentWallpaper < WallpaperNumMethods.getMaxPurchasableWallpaperNum()) ? GlobalConfig.wallpaperCosts[currentWallpaper + 1] : 0;

            buyText.GetComponent<TextMeshProUGUI>().color =
                (PersistentGameManager.instance.playerData.playerData.coinz >= ((currentWallpaper < WallpaperNumMethods.getMaxPurchasableWallpaperNum()) ? GlobalConfig.wallpaperCosts[currentWallpaper + 1] : 0))
                ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;

            string t = "buy {0}:\n{1} coinz";

            if ((int) currentWallpaper < (int) WallpaperNumMethods.getMaxPurchasableWallpaperNum())
                t = t.Replace("{0}", GlobalConfig.wallpaperNames[(int) currentWallpaper + 1]);
            else
                t = "all wallpapers have been bought!";
            t = t.Replace("{1}", ClickerManager.ConvertToShortNumber(nextCost));

            buyText.GetComponent<TextMeshProUGUI>().text = t;
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
        PersistentGameManager.instance.playerData.ResetPlayerData(false);
        PlayerPrefs.SetInt("soundEnabled", 1);
        PlayerPrefs.SetInt("musicEnabled", 1);
        PersistentGameManager.instance.audioManager.StopMusic();
        PersistentGameManager.instance.SwitchScene((int) SceneIndexes.MAIN_MENU);
    }

    public void ToggleSound()
    {
        if (PlayerPrefs.GetInt("soundEnabled") == 1)
        {
            PlayerPrefs.SetInt("soundEnabled", 0);
            PersistentGameManager.instance.audioManager.ToggleSoundFXVolume(false);
        }
        else
        {
            PlayerPrefs.SetInt("soundEnabled", 1);
            PersistentGameManager.instance.audioManager.ToggleSoundFXVolume(true);
        }
        
        UpdateText();
    }

    public void ToggleMusic()
    {
        if (PlayerPrefs.GetInt("musicEnabled") == 1)
        {
            PlayerPrefs.SetInt("musicEnabled", 0);
            PersistentGameManager.instance.audioManager.ToggleMusicVolume(false);
        }
        else
        {
            PlayerPrefs.SetInt("musicEnabled", 1);
            PersistentGameManager.instance.audioManager.ToggleMusicVolume(true);
        }

        UpdateText();
    }

    public void OpenUnlockPanel()
    {
        UpdateText();
        unlockPanel.transform.Find("Unlock Layout/Yes Button").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        unlockPanel.transform.Find("Unlock Layout/Yes Button").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                if (PersistentGameManager.instance.playerData.playerData.unlockedGuessing)
                    UnlockMinigame(2);
                else
                    UnlockMinigame(1);
            });
        unlockPanel.SetActive(true);
    }

    public void UpdateText()
    {
        coinzText.GetComponent<TextMeshProUGUI>().text = ClickerManager.ConvertToShortNumber(PersistentGameManager.instance.playerData.playerData.coinz) + " coinz";
        ageText.GetComponent<TextMeshProUGUI>().text = GetAgeOfDoug();
        if (PlayerPrefs.GetInt("soundEnabled") == 1)
            toggleSoundButtonText.GetComponent<TextMeshProUGUI>().text = "sound: on";
        else
            toggleSoundButtonText.GetComponent<TextMeshProUGUI>().text = "sound: off";
        if (PlayerPrefs.GetInt("musicEnabled") == 1)
            toggleMusicButtonText.GetComponent<TextMeshProUGUI>().text = "music: on";
        else
            toggleMusicButtonText.GetComponent<TextMeshProUGUI>().text = "music: off";

        if (PersistentGameManager.instance.playerData.playerData.unlockedFlippy)
            unlockPanel.transform.Find("Unlock Text").GetComponent<TextMeshProUGUI>().text = "you shouldn't be able to see this!";
        else if (PersistentGameManager.instance.playerData.playerData.unlockedGuessing)
        {
            unlockPanel.transform.Find("Unlock Layout/Yes Button/Text (TMP)").GetComponent<TextMeshProUGUI>().color =
                (PersistentGameManager.instance.playerData.playerData.love > 100f) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
            unlockPanel.transform.Find("Unlock Text").GetComponent<TextMeshProUGUI>().text = "spend 100 luv to unlock flippy doug?";
        }
        else
        {
            unlockPanel.transform.Find("Unlock Layout/Yes Button/Text (TMP)").GetComponent<TextMeshProUGUI>().color =
                (PersistentGameManager.instance.playerData.playerData.love > 100f) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
            unlockPanel.transform.Find("Unlock Text").GetComponent<TextMeshProUGUI>().text = "spend 100 luv to unlock find the bisco?";
        }    
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
        float weight = (float) PersistentGameManager.instance.playerData.playerData.weight / (float)GlobalConfig.maxWeight;
        PersistentGameManager.instance.audioManager.PlaySound("woof", pitchMult: (weight < 0.25) ? (weight * -0.8f) + 1.2f : ((weight > 0.75) ? (weight * -0.8f) + 1.6f : 1f));

        if (Random.Range(0f, 1f) < 0.33f)
        {
            MakePopupHeart();
            PersistentGameManager.instance.ModifyStat(Stat.Love, 1);
        }

        Random.InitState(System.DateTime.Now.Millisecond);
        LeanTween.move(doug, Random.insideUnitCircle * 0.5f, 0.75f).setFrom(dougSpriteDefaultPosition).setEasePunch();
    }

    public void UseFood(int food)
    {
        switch ((Food) food)
        {
            case Food.FoodA:
                PersistentGameManager.instance.ModifyStat(Stat.Hunger, -1);
                PersistentGameManager.instance.ModifyStat(Stat.Weight, 1);
                MakePopup("-1 hungy\n+1 weiht");
                break;
            case Food.FoodB:
                PersistentGameManager.instance.ModifyStat(Stat.Hunger, -2);
                PersistentGameManager.instance.ModifyStat(Stat.Weight, 2);
                PersistentGameManager.instance.ModifyStat(Stat.Love, 1);
                MakePopup("-2 hungy\n+2 weiht\n+1 luv");
                break;
            case Food.FoodC:
                PersistentGameManager.instance.ModifyStat(Stat.Hunger, -3);
                PersistentGameManager.instance.ModifyStat(Stat.Love, -1);
                MakePopup("-3 hungy\n-1 luv");
                break;
        }
        UpdateBars();
    }

    public void UpdateBars()
    {
        modifyBar((float)PersistentGameManager.instance.playerData.playerData.hunger / GlobalConfig.maxHunger, "hungy: " + PersistentGameManager.instance.playerData.playerData.hunger.ToString() + "/" + GlobalConfig.maxHunger.ToString(), hungerStat);
        modifyBar((float)PersistentGameManager.instance.playerData.playerData.boredom / GlobalConfig.maxBoredom, "bord: " + PersistentGameManager.instance.playerData.playerData.boredom.ToString() + "/" + GlobalConfig.maxBoredom.ToString(), boredomStat);
        modifyBar((float)PersistentGameManager.instance.playerData.playerData.weight / (float)GlobalConfig.maxWeight, "weiht: " + PersistentGameManager.instance.playerData.playerData.weight.ToString() + "/" + GlobalConfig.maxWeight.ToString(), weightStat);
        if (PersistentGameManager.instance.playerData.playerData.love > GlobalConfig.maxLove)
            modifyBar(1.0f, "luv: " + GlobalConfig.maxLove.ToString() + "+/" + GlobalConfig.maxLove.ToString(), loveStat);
        else
            modifyBar((float)PersistentGameManager.instance.playerData.playerData.love / GlobalConfig.maxLove, "luv: " + PersistentGameManager.instance.playerData.playerData.love.ToString() + "/" + GlobalConfig.maxLove.ToString(), loveStat);
        doug.transform.localScale = new Vector3(dougSpriteDefaultScale.x * GetDougWeightScale(), dougSpriteDefaultScale.y, dougSpriteDefaultScale.z);
    } 

    public float GetDougWeightScale()
    {
        float weight = 0.5f;
        if (PersistentGameManager.instance != null)
            weight = (float) PersistentGameManager.instance.playerData.playerData.weight / (float) GlobalConfig.maxWeight;

        if (weight < 0.25)
            return (weight * 2f) + .5f;
        else if (weight > 0.75)
            return (weight * 8f) - 5f;
        else
            return 1f;
    }

    public string GetAgeOfDoug()
    {
        Debug.Log("acquisitionDate is " +  PersistentGameManager.instance.playerData.playerData.acquisitionDate.date.ToString() +
                    "\nnow is " + System.DateTime.UtcNow.ToString());

        System.TimeSpan age = System.DateTime.UtcNow.Subtract(PersistentGameManager.instance.playerData.playerData.acquisitionDate.date);
        Debug.Log("\nage is " + age.ToString());
        
        if (age.TotalDays < 1)
        {
            if (age.Hours == 0)
                return age.Minutes.ToString() + (age.Minutes == 1 ? " minute old" : " minutes old");
            else
                return age.Hours.ToString() + (age.Hours == 1 ? " hour, " : " hours, ") + age.Minutes.ToString() + (age.Minutes == 1 ? " minute old" : " minutes old");
        }
        else
            return ((int) age.TotalDays).ToString() + (age.TotalDays == 1 ? " day, " : " days, ") + age.Hours.ToString() + (age.Hours == 1 ? " hour old" : " hours old");
    }

    public void UnlockMinigame(int num)
    {
        bool unlockedGuessing = PersistentGameManager.instance.playerData.playerData.unlockedGuessing;
        bool unlockedFlippy = PersistentGameManager.instance.playerData.playerData.unlockedFlippy;

        if (num == 1 && !unlockedGuessing && PersistentGameManager.instance.playerData.playerData.love >= 100)
        {
            PersistentGameManager.instance.ModifyStat(Stat.Love, -105);
            PersistentGameManager.instance.playerData.playerData.unlockedGuessing = true;
            UpdateText();
            UpdateMinigameText();
            PersistentGameManager.instance.SaveGame();
            unlockPanel.SetActive(false);
            GoToScene("Guessing");
        }

        else if (num == 2 && !unlockedFlippy && unlockedGuessing && PersistentGameManager.instance.playerData.playerData.love >= 100)
        {
            PersistentGameManager.instance.ModifyStat(Stat.Love, -105);
            PersistentGameManager.instance.playerData.playerData.unlockedFlippy = true;
            UpdateText();
            UpdateMinigameText();
            PersistentGameManager.instance.SaveGame();
            unlockPanel.SetActive(false);
            GoToScene("Flippy");
        }
    }

    public void UpdateMinigameText()
    {
        bool unlockedGuessing = PersistentGameManager.instance.playerData.playerData.unlockedGuessing;
        bool unlockedChachaTrail = PersistentGameManager.instance.playerData.playerData.unlockedFlippy;

        if (unlockedGuessing)
        {
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.textColor;
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "find the bisco";
            guessingGameButton.transform.Find("Unlock Text").gameObject.SetActive(false);

            if (unlockedChachaTrail)
            {
                flippyGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.textColor;
                flippyGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "flippy doug";
                flippyGameButton.transform.Find("Unlock Text").gameObject.SetActive(false);
            }
            else
            {
                flippyGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
                flippyGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
            }
        }
        else
        {
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
            guessingGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
            flippyGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
            flippyGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
        }        
    }

    public bool CheckDeactivateDoug()
    {
        if (PersistentGameManager.instance.playerData.playerData.weight <= 0 || PersistentGameManager.instance.playerData.playerData.boredom >= GlobalConfig.maxBoredom)
        {
            if (!deactivated)
            {
                bool deathByWeight = PersistentGameManager.instance.playerData.playerData.weight <= 0;
                deactivated = true;
                StartCoroutine(DeactivateDoug(deathByWeight));
                CheckDeactivateDoug();
            }
            return true;
        }
        else
        {
            doug.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            return false;
        }
    }

    IEnumerator DeactivateDoug(bool deathByWeight)
    {
        doug.transform.eulerAngles = new Vector3(0f, 0f, 180f);
        disableInteractionPanel.SetActive(true);

        string t = "it appears that your doug has been \"deactivated\" due to {0}!\n\nyou are being delivered a new doug:\n\ndoug #{1}\n{2}";
        t = t.Replace("{0}", (deathByWeight) ? "starvation" : "boredom");
        t = t.Replace("{1}", Random.Range(int.Parse("100000", System.Globalization.NumberStyles.HexNumber),
                                          int.Parse("1000000", System.Globalization.NumberStyles.HexNumber)).ToString("X"));
        System.Random rnd = new System.Random();
        string stat1 = uselessStats[rnd.Next(uselessStats.Length)];
        string stat2, stat3;
        do stat2 = uselessStats[rnd.Next(uselessStats.Length)];
        while (stat2.Equals(stat1));
        do stat3 = uselessStats[rnd.Next(uselessStats.Length)];
        while (stat3.Equals(stat1) || stat3.Equals(stat2));
        t = t.Replace("{2}", stat1 + ": " + rnd.Next(3, 21).ToString() + "\n" +
                             stat2 + ": " + rnd.Next(3, 21).ToString() + "\n" +
                             stat3 + ": " + rnd.Next(3, 21).ToString());
        deactivationPanel.transform.Find("Deactivation Layout/Deactivation Text").GetComponent<TextMeshProUGUI>().text = t;

        yield return new WaitForSeconds(3.5f);
        disableInteractionPanel.SetActive(false);
        deactivationPanel.SetActive(true);
        PersistentGameManager.instance.playerData.ResetPlayerData(true);
        PersistentGameManager.instance.SaveGame();
    }

    public void StartRemoveDougCoroutine()
    {
        StartCoroutine(RemoveDoug());
    }

    public IEnumerator RemoveDoug()
    {
        deactivated = false;
        LeanTween.moveLocalX(doug, 1500f, 1.2f).setEaseInQuart();
        yield return new WaitForSeconds(1.5f);
        doug.transform.localPosition = new Vector3(-1500f, 0f, 0f);
        UpdateBars();
        doug.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        LeanTween.moveLocalX(doug, 0, 1.2f).setEaseOutQuart();
    }

    public void UpdateWallpaperButtons()
    {
        for (int i = 1; i < (int) WallpaperNumMethods.getMaxWallpaperNum() + 1; i++)
        {
            if ((int) PersistentGameManager.instance.playerData.playerData.unlockedWallpaper >= i)
            {
                wallpaperButtons[i].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = GlobalConfig.wallpaperNames[i];
                wallpaperButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(GlobalConfig.wallpaperFileNames[i]);
            }
            else
            {
                wallpaperButtons[i].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "?";
            }
        }
    }

    public void DebugButton()
    {
        
    }
}
