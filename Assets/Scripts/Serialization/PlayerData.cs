﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int savefileVersionMajor;
    public int savefileVersionMinor;
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
    public bool unlockedChachaTrail;
    public int stardomBonus;
    public int guessingEasyHiScore;
    public int guessingNormalHiScore;
    public int guessingHardHiScore;
    public WallpaperNum unlockedWallpaper;
    public WallpaperNum currentWallpaper;
}