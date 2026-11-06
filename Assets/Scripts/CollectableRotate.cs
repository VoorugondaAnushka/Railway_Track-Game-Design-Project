using UnityEngine;

public class CollectableRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotateSpeed = 120f;
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis * rotateSpeed * Time.deltaTime, Space.World);
    }
}

