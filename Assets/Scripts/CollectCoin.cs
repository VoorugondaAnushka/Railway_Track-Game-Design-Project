using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectCoin : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip collectSound;

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
                MasterLevelInfo.Instance.AddCoin(1);
            }

            // Play sound
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, 1.0f);
            }

            // Shrink and rise effect
            StartCoroutine(ShrinkAndRiseCoroutine());
        }
    }

    private IEnumerator ShrinkAndRiseCoroutine()
    {
        // Disable collider immediately
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 initialPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Shrink scale to 0
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            // Rise upwards
            transform.position = initialPos + new Vector3(0f, t * 1.5f, 0f);

            yield return null;
        }

        Destroy(gameObject);
    }
}

