using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    [SerializeField]
    GameObject unlockPanel = null;

    private Vector3 dougSpriteDefaultScale = new Vector3(3f, 3f, 1f);
    private Vector3 dougSpriteDefaultPosition;
    private bool deactivated = false;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckDeactivateDoug", 0f, 60f * 1f);
        doug.transform.localScale = new Vector3(dougSpriteDefaultScale.x * GetDougWeightScale(), dougSpriteDefaultScale.y, dougSpriteDefaultScale.z);
        dougSpriteDefaultPosition = doug.transform.position;

        coinzText.GetComponent<TextMeshProUGUI>().text = PersistentGameManager.instance.playerData.playerData.coinz.ToString("F1") + " coinz";
        //unlockPanel.transform.Find("Unlock Layout/No Button/Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.textColor;
        UpdateText();
        UpdateMinigameText();
        UpdateBars();
        PersistentGameManager.instance.SaveGame();
        CheckDeactivateDoug();
    }

    // Update is called once per frame
    void Update()
    {
        coinzText.GetComponent<TextMeshProUGUI>().text = PersistentGameManager.instance.playerData.playerData.coinz.ToString("F1") + " coinz";
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
            case "Chacha Trail":
                if (PersistentGameManager.instance.playerData.playerData.unlockedChachaTrail)
                    MakePopup("NYI!");
                else if (PersistentGameManager.instance.playerData.playerData.unlockedGuessing)
                    OpenUnlockPanel();
                break;

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
        PlayerPrefs.SetInt("adsEnabled", 1);
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

    public void ToggleAds()
    {
        MakePopup("this does nothing!");
        if (PlayerPrefs.GetInt("adsEnabled") == 1)
        {
            PlayerPrefs.SetInt("adsEnabled", 0);
        }
        else
        {
            PlayerPrefs.SetInt("adsEnabled", 1);
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
        coinzText.GetComponent<TextMeshProUGUI>().text = PersistentGameManager.instance.playerData.playerData.coinz.ToString("F1") + " coinz";
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

        if (PersistentGameManager.instance.playerData.playerData.unlockedChachaTrail)
            unlockPanel.transform.Find("Unlock Text").GetComponent<TextMeshProUGUI>().text = "you shouldn't be able to see this!";
        else if (PersistentGameManager.instance.playerData.playerData.unlockedChachaTrail)
        {
            unlockPanel.transform.Find("Unlock Layout/Yes Button/Text (TMP)").GetComponent<TextMeshProUGUI>().color =
                (PersistentGameManager.instance.playerData.playerData.love > 100f) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
            unlockPanel.transform.Find("Unlock Text").GetComponent<TextMeshProUGUI>().text = "spend 100 luv to unlock Chacha Trail?";
        }
        else
        {
            unlockPanel.transform.Find("Unlock Layout/Yes Button/Text (TMP)").GetComponent<TextMeshProUGUI>().color =
                (PersistentGameManager.instance.playerData.playerData.love > 100f) ? GlobalConfig.textColor : GlobalConfig.disabledTextColor;
            unlockPanel.transform.Find("Unlock Text").GetComponent<TextMeshProUGUI>().text = "spend 100 luv to unlock Find the Bisco?";
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

        if (Random.Range(0, 4) == 3)
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
        float weight = (float)PersistentGameManager.instance.playerData.playerData.weight / (float)GlobalConfig.maxWeight;
        if (weight < 0.25)
            return (weight * 2f) + .5f;
        else if (weight > 0.75)
            return (weight * 8f) - 5f;
        else
            return 1f;
    }

    public string GetAgeOfDoug()
    {
        System.TimeSpan age = System.DateTime.UtcNow.Subtract(PersistentGameManager.instance.playerData.playerData.acquisitionDate);
        Debug.Log("now is " + System.DateTime.UtcNow.ToString() + ", acquisitionDate is " +
        PersistentGameManager.instance.playerData.playerData.acquisitionDate.ToString() + ", age is " + age.ToString());

        if (age.Hours < 1)
            return age.Minutes.ToString() + (age.Minutes == 1 ? " minute old" : " minutes old");
        else if (age.Days < 1)
            return age.Hours.ToString() + (age.Hours == 1 ? " hour, " : " hours, ") + age.Minutes.ToString() + (age.Minutes == 1 ? " minute old" : " minutes old");
        else
            return age.Hours.ToString() + (age.Days == 1 ? " day, " : " days, ") + age.Hours.ToString() + (age.Hours == 1 ? " hour old" : " hours old");
    }

    public void UnlockMinigame(int num)
    {
        bool unlockedGuessing = PersistentGameManager.instance.playerData.playerData.unlockedGuessing;
        bool unlockedChachaTrail = PersistentGameManager.instance.playerData.playerData.unlockedChachaTrail;

        if (num == 1 && !unlockedGuessing && PersistentGameManager.instance.playerData.playerData.love >= 100)
        {
            PersistentGameManager.instance.ModifyStat(Stat.Love, -105);
            PersistentGameManager.instance.playerData.playerData.unlockedGuessing = true;
            UpdateText();
            UpdateMinigameText();
            PersistentGameManager.instance.SaveGame();
            Debug.Log("unlocked guessing: " + PersistentGameManager.instance.playerData.playerData.unlockedGuessing.ToString());
            unlockPanel.SetActive(false);
            GoToScene("Guessing");
        }

        else if (num == 2 && !unlockedChachaTrail && unlockedGuessing && PersistentGameManager.instance.playerData.playerData.love >= 100)
        {
            PersistentGameManager.instance.ModifyStat(Stat.Love, -105);
            PersistentGameManager.instance.playerData.playerData.unlockedChachaTrail = true;
            UpdateText();
            UpdateMinigameText();
            PersistentGameManager.instance.SaveGame();
            Debug.Log("unlocked chacha: " + PersistentGameManager.instance.playerData.playerData.unlockedChachaTrail.ToString());
            unlockPanel.SetActive(false);
            GoToScene("Chacha Trail");
        }
    }

    public void UpdateMinigameText()
    {
        bool unlockedGuessing = PersistentGameManager.instance.playerData.playerData.unlockedGuessing;
        bool unlockedChachaTrail = PersistentGameManager.instance.playerData.playerData.unlockedChachaTrail;

        if (unlockedGuessing)
        {
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.textColor;
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "find the bisco";
            guessingGameButton.transform.Find("Unlock Text").gameObject.SetActive(false);

            if (unlockedChachaTrail)
            {
                chachaGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.textColor;
                chachaGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "chacha trail";
                chachaGameButton.transform.Find("Unlock Text").gameObject.SetActive(false);
            }
            else
            {
                chachaGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
                chachaGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
            }
        }
        else
        {
            guessingGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
            guessingGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
            chachaGameButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = GlobalConfig.disabledTextColor;
            chachaGameButton.transform.Find("Unlock Text").gameObject.SetActive(true);
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
        string t = deactivationPanel.transform.Find("Deactivation Layout/Deactivation Text").GetComponent<TextMeshProUGUI>().text.Replace("{0}", (deathByWeight) ? "starvation" : "boredom")
        .Replace("{1}", Random.Range(int.Parse("100000", System.Globalization.NumberStyles.HexNumber), int.Parse("1000000", System.Globalization.NumberStyles.HexNumber)).ToString("X"));
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
        LeanTween.moveLocalX(doug, 1500f, 1.2f).setEaseInQuart();
        yield return new WaitForSeconds(1.5f);
        doug.transform.localPosition = new Vector3(-1500f, 0f, 0f);
        UpdateBars();
        doug.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        LeanTween.moveLocalX(doug, 0, 1.2f).setEaseOutQuart();
    }

    public void DebugButton()
    {
        PersistentGameManager.instance.ModifyStat(Stat.Weight, -200);
        UpdateBars();
    }
}
