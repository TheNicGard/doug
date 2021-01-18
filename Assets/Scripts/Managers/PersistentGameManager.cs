using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentGameManager : MonoBehaviour
{
    public static PersistentGameManager instance;
    [SerializeField]
    GameObject loadingScreen = null;
    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public int currentScene = 0;
    public SaveData playerData;
    public ClickerData[] videos = null;
    [SerializeField]
    TextAsset clickerData = null;
    [SerializeField]
    public AudioManager audioManager;

    void Start()
    {
        initialize();
    }

    void Awake()
    {

    }

    public void initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(instance);
        DontDestroyOnLoad(loadingScreen);

        playerData = new SaveData();
        playerData.playerData = new PlayerData();
        if (!SerializationManager.DoesFileExist("save"))
        {
            playerData.ResetPlayerData(false);
            SaveGame();
        }
        LoadSave();
        LoadClickerData();

        int minutes = (int) System.DateTime.Now.Subtract(playerData.playerData.lastDate).TotalMinutes;
        for (int i = 0; i < minutes; i++)
            UpdateStats();
        InvokeRepeating("UpdateStats", 60f, 60f * 1f);
        InvokeRepeating("ClickerTick", 0.0f, 1.0f / GlobalConfig.incrementsPerSecond);
        InvokeRepeating("DepleteStardom", 15f, 15f * 1f);
        PersistentGameManager.instance.playerData.playerData.coinz += minutes * 60f * CoinzPerSecond(false);

        PersistentGameManager.instance.audioManager.ToggleMusicVolume(PlayerPrefs.GetInt("musicEnabled") == 1);
        PersistentGameManager.instance.audioManager.ToggleSoundFXVolume(PlayerPrefs.GetInt("soundEnabled") == 1);

        currentScene = (int) SceneManager.GetActiveScene().buildIndex;

        LoadButtonSounds();
        GetSceneMusic();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadButtonSounds();
        ChangeWallpaper((int) instance.playerData.playerData.currentWallpaper);
    }

    public void LoadGame()
    {
        currentScene = (int) SceneIndexes.MAIN_MENU;
        SwitchScene((int) SceneIndexes.HOME_SCREEN);
    }

    public void SwitchScene(int newSceneIndex)
    {
        SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Single);
        currentScene = newSceneIndex;
        GetSceneMusic();
        if (currentScene == (int) SceneIndexes.MAIN_MENU)
            initialize();
    }

    public void SaveGame()
    {
        playerData.playerData.lastDate = System.DateTime.Now;
        SerializationManager.Save("save", playerData);
    }

    public void LoadSave()
    {
        playerData = SerializationManager.Load("save") as SaveData;
    }

    public void ModifyStat(Stat stat, int amount)
    {
        switch (stat)
        {
            case Stat.Hunger:
                if (playerData.playerData.hunger + amount > GlobalConfig.maxHunger)
                    playerData.playerData.hunger = GlobalConfig.maxHunger;
                else if (playerData.playerData.hunger + amount < 0)
                    playerData.playerData.hunger = 0;
                else playerData.playerData.hunger += amount;
                break;
            case Stat.Boredom:
                if (playerData.playerData.boredom + amount > GlobalConfig.maxBoredom)
                    playerData.playerData.boredom = GlobalConfig.maxBoredom;
                else if (playerData.playerData.boredom + amount < 0)
                    playerData.playerData.boredom = 0;
                else playerData.playerData.boredom += amount;
                break;
            case Stat.Weight:
                if (playerData.playerData.weight + amount > GlobalConfig.maxWeight)
                    playerData.playerData.weight = GlobalConfig.maxWeight;
                else if (playerData.playerData.weight + amount < 0)
                    playerData.playerData.weight = 0;
                else playerData.playerData.weight += amount;
                break;
            case Stat.Love:
                if (playerData.playerData.love + amount < 0)
                    playerData.playerData.love = 0;
                else playerData.playerData.love += amount;
                break;
            case Stat.Stardom:
                if (playerData.playerData.stardomBonus + amount < 0)
                    playerData.playerData.stardomBonus = 0;
                else playerData.playerData.stardomBonus += amount;
                break;
        }
    }

    private int updateStatsCounter = 0;
    private int depletionTickTime = (int) (GlobalConfig.depletionTickTime / 60f);
    public void UpdateStats()
    {
        switch (currentScene)
        {
            case (int) SceneIndexes.MAIN_MENU:
            case (int) SceneIndexes.HOME_SCREEN:
                if (updateStatsCounter++ == depletionTickTime)
                {
                    updateStatsCounter = 0;
                    // do rest of updates
                    if (playerData.playerData.hunger >= GlobalConfig.maxHunger)
                        ModifyStat(Stat.Weight, -1);
                    ModifyStat(Stat.Hunger, 1);
                    ModifyStat(Stat.Boredom, 1);
                    ModifyStat(Stat.Love, -1);
                    ModifyStat(Stat.Stardom, -4);
                }
                break;
            case (int) SceneIndexes.CLICKER:
                ModifyStat(Stat.Boredom, -3);
                ModifyStat(Stat.Weight, -1);
                break;
            case (int) SceneIndexes.GUESSING:
                ModifyStat(Stat.Weight, -1);
                break;
        }

        SaveGame();
    }

    void OnDisable()
    {
        SaveGame();
    }

    public void ChangeWallpaper(int num)
    {
        if (num >= 0 && num <= (int) WallpaperNum.COTTON_CLOTH && num <= (int) instance.playerData.playerData.unlockedWallpaper)
            GameObject.FindGameObjectsWithTag("Background")[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<UnityEngine.Sprite>(GlobalConfig.wallpaperFileNames[num]);
        else
            GameObject.FindGameObjectsWithTag("Background")[0].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<UnityEngine.Sprite>(GlobalConfig.wallpaperFileNames[0]);
    }

    /*~~~~~ Clicker Management ~~~~~*/
    [System.Serializable]
    public class ClickerData
    {
        public string title;
        public float requiredCoinz;
        public float coinzPerSecond;
    }

    public void IncrementCoinz(float dCoinz)
    {
        playerData.playerData.coinz += dCoinz;
    }

    public void ClickerTick()
    {
        IncrementCoinz(CoinzPerSecond(true) / GlobalConfig.incrementsPerSecond);
    }

    public void DepleteStardom()
    {
        ModifyStat(Stat.Stardom, -1);
    }

    public float CoinzPerSecond(bool useStardom)
    {
        float coinzPerSecond = 0f;
        for (int i = 0; i < videos.Length; i++) {
            
            coinzPerSecond += videos[i].coinzPerSecond * 
            playerData.playerData.clickerVideos[i] * 
            playerData.playerData.clickerVideosComments[i];
        }
        if (useStardom)
            coinzPerSecond *= (1f + (playerData.playerData.stardomBonus / 100f));
        return coinzPerSecond;
    }

    public void LoadClickerData()
    {
        videos = JsonHelper.FromJson<ClickerData>(clickerData.text);
    }

    public void GetSceneMusic()
    {
        switch (currentScene)
        {
            case (int) SceneIndexes.HOME_SCREEN:
            case (int) SceneIndexes.GUESSING:
                audioManager.PlaySound("wholesome");
                break;
            case (int) SceneIndexes.CLICKER:
                audioManager.PlaySound("carefree");
                break;
        }
    }

    public void LoadButtonSounds()
    {
        GameObject[] go = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject g in go)
        {   
            if (g.tag == "Button")
            {
                g.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {PersistentGameManager.instance.audioManager.PlaySound("click");});
            }
            else if (g.tag == "Close Button")
            {
                g.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {PersistentGameManager.instance.audioManager.PlaySound("close_click");});
            }
        }
    }

    public void LoadWallpaperButtons()
    {
        GameObject[] go = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject g in go)
        {   
            if (g.tag == "Button" && g.name.Contains("Wallpaper"))
            {
                int wallpaperButtonNum = System.Convert.ToInt32(g.name[g.name.Length - 1].ToString());
                g.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {PersistentGameManager.instance.ChangeWallpaper(wallpaperButtonNum - 1);});
            }
        }
    }
}
