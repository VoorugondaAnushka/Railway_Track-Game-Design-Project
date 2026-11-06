using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollisionDetect : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip collisionSound;

    [Header("Camera Shake Effect")]
    public Camera mainCamera;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;

    [Header("Fade Out")]
    public CanvasGroup fadeCanvasGroup;
    public Image fadeOverlay;

    private bool hasCollided = false;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision.gameObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        CheckCollision(hit.gameObject);
    }

    private void CheckCollision(GameObject obj)
    {
        if (hasCollided) return;

        if (obj.CompareTag("Obstacle") || obj.name.ToLower().Contains("obstacle") || obj.name.ToLower().Contains("train") || obj.name.ToLower().Contains("barrier"))
        {
            hasCollided = true;
            Debug.Log("[CollisionDetect] Hit Obstacle! Triggering collision flow...");

            // 1. Stop Player Movement
            PlayerMovement movement = GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.StopMoving();
            }

            // 2. Play Collision Sound
            if (collisionSound != null)
            {
                AudioSource.PlayClipAtPoint(collisionSound, transform.position, 1.0f);
            }

            // 3. Save progress on impact via SaveLoad
            if (MasterLevelInfo.Instance != null)
            {
                SaveLoad.SaveData(
                    MasterLevelInfo.Instance.coinCount,
                    MasterLevelInfo.Instance.gemCount,
                    MasterLevelInfo.Instance.distanceRun
                );
            }

            // 4. Start Camera Shake & Fade-out Coroutine
            StartCoroutine(CollisionRoutine());
        }
    }

    private IEnumerator CollisionRoutine()
    {
        // Camera Shake
        if (mainCamera != null)
        {
            Vector3 originalCamPos = mainCamera.transform.localPosition;
            float elapsedShake = 0f;

            while (elapsedShake < shakeDuration)
            {
                elapsedShake += Time.deltaTime;
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;
                mainCamera.transform.localPosition = originalCamPos + new Vector3(x, y, 0f);
                yield return null;
            }

            mainCamera.transform.localPosition = originalCamPos;
        }

        // Fade Out
        float fadeTime = 0.8f;
        float elapsedFade = 0f;

        while (elapsedFade < fadeTime)
        {
            elapsedFade += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedFade / fadeTime);

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = alpha;
            }
            else if (fadeOverlay != null)
            {
                Color c = fadeOverlay.color;
                c.a = alpha;
                fadeOverlay.color = c;
            }

            yield return null;
        }

        // Wait brief pause, then reload current scene to restart
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

