using System.Linq;

public enum WallpaperNum
{
    PRINTER,
    PRINTER_DAMAGED,
    LINED,
    LINED_DAMAGED,
    WALL_PAINT,
    COTTON_CLOTH,
    COOKIE_PAPER
}

static class WallpaperNumMethods
{
    public static WallpaperNum getMaxWallpaperNum()
    {
        return System.Enum.GetValues(typeof(WallpaperNum)).Cast<WallpaperNum>().Max();
    }
}