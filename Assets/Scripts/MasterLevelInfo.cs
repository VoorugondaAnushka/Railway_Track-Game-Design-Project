using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MasterLevelInfo : MonoBehaviour
{
    public static MasterLevelInfo Instance;

    [Header("Current Run Data")]
    public int coinCount = 0;
    public int gemCount = 0;
    public int distanceRun = 0;

    [Header("UI References (TextMeshPro or standard Text)")]
    public TextMeshProUGUI coinDisplayTMP;
    public TextMeshProUGUI gemDisplayTMP;
    public TextMeshProUGUI distanceDisplayTMP;

    public Text coinDisplayText;
    public Text gemDisplayText;
    public Text distanceDisplayText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        UpdateHUD();
    }

    public void AddCoin(int amount = 1)
    {
        coinCount += amount;
        UpdateHUD();
    }

    public void AddGem(int amount = 1)
    {
        gemCount += amount;
        UpdateHUD();
    }

    public void UpdateDistance(int distance)
    {
        distanceRun = distance;
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        // TextMeshPro update
        if (coinDisplayTMP != null) coinDisplayTMP.text = coinCount.ToString();
        if (gemDisplayTMP != null) gemDisplayTMP.text = gemCount.ToString();
        if (distanceDisplayTMP != null) distanceDisplayTMP.text = distanceRun.ToString();

        // Legacy UI Text update fallback
        if (coinDisplayText != null) coinDisplayText.text = coinCount.ToString();
        if (gemDisplayText != null) gemDisplayText.text = gemCount.ToString();
        if (distanceDisplayText != null) distanceDisplayText.text = distanceRun.ToString();
    }
}

