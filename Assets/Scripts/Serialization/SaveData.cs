using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveData
{
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            if (_current == null)
            {
                _current = new SaveData();
            }
            return _current;
        }
    }

    public PlayerData playerData;

    public void ResetPlayerData(bool dougIsDeactivated)
    {
        if (!dougIsDeactivated)
        {
            PlayerPrefs.DeleteAll();
            playerData.clickerVideos = new int [12] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            playerData.clickerVideosComments = new int [12] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
            playerData.lastDate = new SerializableDate(){date = System.DateTime.UtcNow};
            playerData.unlockedGuessing = false;
            playerData.unlockedFlippy = false;
            playerData.stardomBonus = 0;
            playerData.unlockedWallpaper = (WallpaperNum) 0;
            playerData.currentWallpaper = (WallpaperNum) 0;
            playerData.guessingEasyHiScore = 0;
            playerData.guessingNormalHiScore = 0;
            playerData.guessingHardHiScore = 0;
            PlayerPrefs.SetInt("soundEnabled", 1);
            PlayerPrefs.SetInt("musicEnabled", 1);
        }
        
        playerData.hunger = 50;
        playerData.boredom = 50;
        playerData.weight = 75;
        playerData.love = 0;
        playerData.coinz = 0f;

        Debug.Log("[RESETTING, OLD]\nacquisitionDate is " + PersistentGameManager.instance.playerData.playerData.acquisitionDate.ToString() +
                    "\nnow is " + System.DateTime.UtcNow.ToString());
        playerData.acquisitionDate = new SerializableDate(){date = System.DateTime.UtcNow};
        Debug.Log("[RESETTING, NEW]\nacquisitionDate is " + PersistentGameManager.instance.playerData.playerData.acquisitionDate.ToString() +
                    "\nnow is " + System.DateTime.UtcNow.ToString());

        playerData.savefileVersion = Application.version;
    }

    override public string ToString()
    {
        string temp = "";

        temp +=
        "Hunger: " + playerData.hunger + "\n" +
        "Boredom: " + playerData.boredom + "\n" +
        "Weight: " + playerData.weight + "\n" +
        "Love: " + playerData.love + "\n" +
        "Coinz: " + playerData.coinz + "\n";
        
        temp += "Videos: {";
        for (int i = 0; i < playerData.clickerVideos.Length; i++)
            temp += playerData.clickerVideos[i].ToString() + ", ";
        temp += "}\n";

        temp += "Comments: {";
        for (int i = 0; i < playerData.clickerVideosComments.Length; i++)
            temp += playerData.clickerVideosComments[i].ToString() + ", ";
        temp += "}\n";

        temp += "Unlocked Guessing Game: " + playerData.unlockedGuessing.ToString() + "\n";
        temp += "Unlocked Flippy: " + playerData.unlockedFlippy.ToString() + "\n";

        return temp;
    }
}