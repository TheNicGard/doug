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
            playerData.lastDate = System.DateTime.UtcNow;
            playerData.unlockedGuessing = false;
            playerData.unlockedChachaTrail = false;
            playerData.stardomBonus = 0;
            PlayerPrefs.SetInt("soundEnabled", 1);
            PlayerPrefs.SetInt("musicEnabled", 1);
            PlayerPrefs.SetInt("adsEnabled", 1);
        }
        
        playerData.hunger = 50;
        playerData.boredom = 50;
        playerData.weight = 75;
        playerData.love = 0;
        playerData.coinz = 0f;
        playerData.acquisitionDate = System.DateTime.UtcNow;
        playerData.guessingEasyHiScore = 0;
        playerData.guessingNormalHiScore = 0;
        playerData.guessingHardHiScore = 0;
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
        temp += "Unlocked Chacha Trail: " + playerData.unlockedChachaTrail.ToString() + "\n";

        return temp;
    }
}