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

    private void Awake()
    {
        instance = this;

        if (currentScene == 0)
            SceneManager.LoadSceneAsync((int) SceneIndexes.MAIN_MENU, LoadSceneMode.Additive);
    }

    public void LoadGame()
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int) SceneIndexes.MAIN_MENU));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.HOME_SCREEN, LoadSceneMode.Additive));
        currentScene = (int) SceneIndexes.HOME_SCREEN;
        StartCoroutine(GetSceneLoadProgress());
    }

    public void SwitchScene(int newSceneIndex)
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync(currentScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive));
        currentScene = newSceneIndex;
        StartCoroutine(GetSceneLoadProgress());
    }

    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
                yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
