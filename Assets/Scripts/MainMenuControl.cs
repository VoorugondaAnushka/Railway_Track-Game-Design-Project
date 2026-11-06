using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuControl : MonoBehaviour
{
    [Header("Top-Right Score Display")]
    public TextMeshProUGUI topScoreTMP;

    [Header("Saved Progress UI (TextMeshPro)")]
    public TextMeshProUGUI savedCoinsTMP;
    public TextMeshProUGUI savedGemsTMP;
    public TextMeshProUGUI savedDistanceTMP;

    [Header("Saved Progress UI (Standard Text Fallback)")]
    public Text savedCoinsText;
    public Text savedGemsText;
    public Text savedDistanceText;

    [Header("Navigation")]
    public string stageSelectSceneName = "StageSelect";

    void Start()
    {
        // Load saved values from PlayerPrefs
        SaveLoad.LoadData();

        // Update UI
        if (topScoreTMP != null) topScoreTMP.text = SaveLoad.savedDistance.ToString();
        if (savedCoinsTMP != null) savedCoinsTMP.text = SaveLoad.savedCoins.ToString();
        if (savedGemsTMP != null) savedGemsTMP.text = SaveLoad.savedGems.ToString();
        if (savedDistanceTMP != null) savedDistanceTMP.text = SaveLoad.savedDistance.ToString();

        if (savedCoinsText != null) savedCoinsText.text = SaveLoad.savedCoins.ToString();
        if (savedGemsText != null) savedGemsText.text = SaveLoad.savedGems.ToString();
        if (savedDistanceText != null) savedDistanceText.text = SaveLoad.savedDistance.ToString();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(stageSelectSceneName);
    }

    public void ResetProgress()
    {
        SaveLoad.ResetData();
        Start(); // refresh display
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

