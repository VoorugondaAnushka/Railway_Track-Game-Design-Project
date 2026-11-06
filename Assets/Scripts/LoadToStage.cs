using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadToStage : MonoBehaviour
{
    [Header("Scene Navigation")]
    public string targetSceneName = "GamePlay";
    public float waitDuration = 3.5f;

    [Header("Fade Out Transition")]
    public Image fadeOverlay;
    public float fadeDuration = 0.8f;

    void Start()
    {
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        // Wait on the information / controls screen
        yield return new WaitForSeconds(waitDuration);

        // Fade out
        if (fadeOverlay != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                Color c = fadeOverlay.color;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadeOverlay.color = c;
                yield return null;
            }
        }

        // Load GamePlay
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

