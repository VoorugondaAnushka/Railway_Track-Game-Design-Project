using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0f, 4.5f, -6.5f);
    public float smoothSpeed = 10f;
    public float pitchAngle = 20f;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        transform.rotation = Quaternion.Euler(pitchAngle, 0f, 0f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Smoothly follow target primarily along Z, while softly damping X
        Vector3 desiredPosition = new Vector3(
            Mathf.Lerp(transform.position.x, target.position.x * 0.5f, smoothSpeed * Time.deltaTime),
            target.position.y + offset.y,
            target.position.z + offset.z
        );

        transform.position = desiredPosition;
    }
}

