using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadSceneAsync((int) SceneIndexes.MAIN_MENU, LoadSceneMode.Single);   
    }
}
