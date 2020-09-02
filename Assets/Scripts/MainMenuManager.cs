using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private SaveData playerData;

    // Start is called before the first frame update
    void Start()
    {
        playerData = new SaveData();
        playerData.playerData = new PlayerData();

        if (!SerializationManager.DoesFileExist("save"))
        {
            playerData.ResetPlayerData();
            SerializationManager.Save("save", playerData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Pointer.current.press.wasPressedThisFrame)
        {
            SceneManager.LoadScene("HomeScreen");
        }
    }
}
