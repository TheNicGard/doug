public class GlobalConfig
{
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
}