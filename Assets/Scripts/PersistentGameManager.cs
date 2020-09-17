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
    public bool loadedGame = false;
    public SaveData playerData;

    private void Awake()
    {
        instance = this;

        playerData = new SaveData();
        playerData.playerData = new PlayerData();
        if (!SerializationManager.DoesFileExist("save"))
        {
            playerData.ResetPlayerData(false);
            SaveGame();
        }
        LoadSave();
        InvokeRepeating("SaveGame", 60f, 60f * 5f);

        if (currentScene == (int) SceneIndexes.MANAGER)
            SceneManager.LoadSceneAsync((int) SceneIndexes.MAIN_MENU, LoadSceneMode.Additive);
    }

    public void LoadGame()
    {
        currentScene = 1;
        SwitchScene((int) SceneIndexes.HOME_SCREEN);
    }

    public void SwitchScene(int newSceneIndex)
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync(currentScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive));
        currentScene = newSceneIndex;
        StartCoroutine(GetSceneLoadProgress(newSceneIndex == (int) SceneIndexes.HOME_SCREEN));
    }

    public IEnumerator GetSceneLoadProgress(bool initialLoad)
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
                yield return null;
        }

        loadingScreen.SetActive(false);
        if (initialLoad)
            loadedGame = true;
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

    void OnDisable()
    {
        SaveGame();
    }
}
