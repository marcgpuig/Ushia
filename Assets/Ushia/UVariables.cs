public class UVariables
{
    /// Map
    public static double OSM_CHUNK_SIZE = 64;

    /// Debug
    public static bool OSM_DEBUG_NODES = true;
    public static bool OSM_DEBUG_ROADS = true;

    public static string mapzenAPIKey = "mapzen-HgL87jY";

    /// <summary>
    /// Load all Ushia Variables froma file given it's path.
    /// </summary>
    /// <param name="filePath">String with the path of the config file</param>
    /// <returns></returns>
    public static bool Load(string filePath)
    {
        // TODO: load from file
        return true;
    }
}
