using System.Collections.Generic;

public class GlobalConfig
{
    public static int major_version = 1;
    public static int minor_version = 0;
    
    public static int maxHunger = 100;
    public static int maxBoredom = 100;
    public static int maxWeight = 200;
    public static int maxLove = 100;
    public static int incrementsPerSecond = 4;
    public static int totalDepletionTimeInMinutes = 2160; // (36 hours)
    public static float depletionTickTime = (float)totalDepletionTimeInMinutes * 60f / maxHunger; // 2160 * 60 / 100 ~ 21.6 min
    public static UnityEngine.Color textColor = new UnityEngine.Color(50 / 255f, 50 / 255f, 50 / 255f);
    public static UnityEngine.Color disabledTextColor = new UnityEngine.Color(140 / 255f, 140 / 255f, 140 / 255f);
    public static float stardomChance = 0.1f;

    public static int easyTimesToSwap = 10;
    public static int normalTimesToSwap = 20;
    public static int hardTimesToSwap = 40;
    public static float easyswapSpeed = .4f;
    public static float normalswapSpeed = .3f;
    public static float hardswapSpeed = .2f;
    public static Dictionary<WallpaperNum, float> wallpaperCosts = new Dictionary<WallpaperNum, float>()
    {
        {WallpaperNum.PRINTER, 0f},
        {WallpaperNum.PRINTER_DAMAGED, 100f},
        {WallpaperNum.LINED, 10000f},
        {WallpaperNum.LINED_DAMAGED, 1000000f},
        {WallpaperNum.WALL_PAINT, 100000000f},
        {WallpaperNum.COTTON_CLOTH, 10000000000f}  
    };
    /* debug prices for wallpapers
    public static Dictionary<WallpaperNum, float> wallpaperCosts = new Dictionary<WallpaperNum, float>()
    {
        {WallpaperNum.PRINTER, 0f},
        {WallpaperNum.PRINTER_DAMAGED, 1f},
        {WallpaperNum.LINED, 2f},
        {WallpaperNum.LINED_DAMAGED, 3f},
        {WallpaperNum.WALL_PAINT, 4f},
        {WallpaperNum.COTTON_CLOTH, 5f}  
    };
    */
    public static string[] wallpaperNames = {"printer sheet", "damagde printer sheet", "lined paper", "damaged lined paper", "paint wall", "cotton clothe", "cookie paper"};
    public static string[] wallpaperFileNames = {"printer", "damaged_printer", "notebook", "damaged_notebook", "paint", "cloth", "cookie_paper"};
}