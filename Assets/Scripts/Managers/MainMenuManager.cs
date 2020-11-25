using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        PersistentGameManager.instance.LoadGame();
    }
}
