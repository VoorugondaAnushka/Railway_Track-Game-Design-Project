using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GemCollect : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip gemSound;

    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            isCollected = true;

            // Notify Level Info
            if (MasterLevelInfo.Instance != null)
            {
                MasterLevelInfo.Instance.AddGem(1);
            }

            // Play sound
            if (gemSound != null)
            {
                AudioSource.PlayClipAtPoint(gemSound, transform.position, 1.0f);
            }

            // Shrink and rise effect
            StartCoroutine(ShrinkAndRiseCoroutine());
        }
    }

    private IEnumerator ShrinkAndRiseCoroutine()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 initialPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            transform.position = initialPos + new Vector3(0f, t * 2.0f, 0f);

            yield return null;
        }

        Destroy(gameObject);
    }
}

