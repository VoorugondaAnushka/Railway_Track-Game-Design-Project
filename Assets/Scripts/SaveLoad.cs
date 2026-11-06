using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public const string COIN_KEY = "COINSAVE";
    public const string GEM_KEY = "GEMSAVE";
    public const string DISTANCE_KEY = "DISTANCESAVE";

    public static int savedCoins = 0;
    public static int savedGems = 0;
    public static int savedDistance = 0;

    void Awake()
    {
        LoadData();
    }

    public static void SaveData(int coinsToAdd, int gemsToAdd, int currentDistance)
    {
        LoadData();

        savedCoins += coinsToAdd;
        savedGems += gemsToAdd;

        // Keep highest distance or cumulative distance
        if (currentDistance > savedDistance)
        {
            savedDistance = currentDistance;
        }

        PlayerPrefs.SetInt(COIN_KEY, savedCoins);
        PlayerPrefs.SetInt(GEM_KEY, savedGems);
        PlayerPrefs.SetInt(DISTANCE_KEY, savedDistance);
        PlayerPrefs.Save();

        Debug.Log($"[SaveLoad] Progress saved! Total Coins: {savedCoins}, Gems: {savedGems}, Best Distance: {savedDistance}");
    }

    public static void LoadData()
    {
        savedCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
        savedGems = PlayerPrefs.GetInt(GEM_KEY, 0);
        savedDistance = PlayerPrefs.GetInt(DISTANCE_KEY, 0);
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteKey(COIN_KEY);
        PlayerPrefs.DeleteKey(GEM_KEY);
        PlayerPrefs.DeleteKey(DISTANCE_KEY);
        PlayerPrefs.Save();
        savedCoins = 0;
        savedGems = 0;
        savedDistance = 0;
    }
}

