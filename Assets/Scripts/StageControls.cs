using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageControls : MonoBehaviour
{
    [Header("Scenes")]
    public string nextSceneName = "Information";
    public string menuSceneName = "MainMenu";

    [Header("Selected Stage")]
    public string stageName = "Railway Escape";

    public void PlaySelectedStage()
    {
        Debug.Log($"[StageControls] Starting Stage: {stageName}");
        SceneManager.LoadScene(nextSceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}

