using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string savefileVersion;
    public int hunger;
    public int boredom;
    public int weight;
    public int love;
    public float coinz;
    public int[] clickerVideos;
    public int[] clickerVideosComments;
    public SerializableDate lastDate;
    public SerializableDate acquisitionDate;
    public bool unlockedGuessing;
    public bool unlockedFlippy;
    public int stardomBonus;
    public int guessingEasyHiScore;
    public int guessingNormalHiScore;
    public int guessingHardHiScore;
    public int flippyHiScore;
    public WallpaperNum unlockedWallpaper;
    public WallpaperNum currentWallpaper;
}