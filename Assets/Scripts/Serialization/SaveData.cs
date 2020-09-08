﻿using UnityEngine;
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

    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteAll();
        playerData.hunger = 50;
        playerData.boredom = 50;
        playerData.weight = 75;
        playerData.love = 0;
        playerData.coinz = 0f;
        playerData.coinzPerSecond = 0f;
        playerData.clickerVideos = new int [12] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        playerData.clickerVideosComments = new int [12] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        playerData.lastDate = System.DateTime.Now;
        playerData.acquisitionDate = System.DateTime.Now;
        playerData.unlockedGuessing = false;
        playerData.unlockedChachaTrail = false;
        playerData.stardomBonus = 0;
        PlayerPrefs.SetInt("soundEnabled", 1);
        PlayerPrefs.SetInt("musicEnabled", 1);
        PlayerPrefs.SetInt("adsEnabled", 1);
    }

    override public string ToString()
    {
        string temp = "";

        temp +=
        "Hunger: " + playerData.hunger + "\n" +
        "Boredom: " + playerData.boredom + "\n" +
        "Weight: " + playerData.weight + "\n" +
        "Love: " + playerData.love + "\n" +
        "Coinz: " + playerData.coinz + "\n" +
        "Coinz Per Second: " + playerData.coinzPerSecond + "\n";
        
        temp += "Videos: {";
        for (int i = 0; i < playerData.clickerVideos.Length; i++)
            temp += playerData.clickerVideos[i].ToString() + ", ";
        temp += "}\n";

        temp += "Comments: {";
        for (int i = 0; i < playerData.clickerVideosComments.Length; i++)
            temp += playerData.clickerVideosComments[i].ToString() + ", ";
        temp += "}\n";

        return temp;
    }
}