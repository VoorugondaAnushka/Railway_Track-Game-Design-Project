using UnityEngine;

public class TrainMover : MonoBehaviour
{
    [Header("Train Movement")]
    public float oncomingSpeed = 9.5f; // Speed oncoming towards player along -Z
    public bool isMoving = true;

    private Transform playerTransform;

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if (!isMoving) return;

        if (playerTransform == null)
        {
            FindPlayer();
        }

        // Move train oncoming towards player along the negative Z axis
        transform.position += Vector3.back * oncomingSpeed * Time.deltaTime;

        // If train has passed behind the player, destroy to conserve resources
        if (playerTransform != null && transform.position.z < (playerTransform.position.z - 35f))
        {
            Destroy(gameObject);
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else if (PlayerMovement.Instance != null)
        {
            playerTransform = PlayerMovement.Instance.transform;
        }
    }
}
