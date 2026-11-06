using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    [Header("Segment Prefabs (Optional)")]
    public GameObject[] segmentPrefabs;

    [Header("Generation Settings")]
    public float segmentLength = 50f;
    public int initialSegments = 5;
    public float spawnInterval = 3f; // PPT: Spawns random track segments ahead every 3 seconds

    [Header("Target Tracking")]
    public Transform playerTransform;
    public float despawnDistance = 60f;

    private float currentZ = 0f;
    private List<GameObject> activeSegments = new List<GameObject>();
    private int segmentCounter = 0;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        // Spawn initial safe stretch
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment(i == 0); // First segment is safe start
        }

        // Spawns random track segments ahead every 3 seconds
        StartCoroutine(SpawnSegmentsPeriodically());
    }

    void Update()
    {
        // Cleanup segments that player has passed
        if (playerTransform != null && activeSegments.Count > 0)
        {
            GameObject oldestSegment = activeSegments[0];
            if (oldestSegment != null && (playerTransform.position.z - oldestSegment.transform.position.z) > despawnDistance)
            {
                activeSegments.RemoveAt(0);
                Destroy(oldestSegment);
            }
        }
    }

    private IEnumerator SpawnSegmentsPeriodically()
    {
        WaitForSeconds waitTime = new WaitForSeconds(spawnInterval);
        while (true)
        {
            yield return waitTime;

            if (playerTransform != null && (currentZ - playerTransform.position.z) < (segmentLength * initialSegments))
            {
                SpawnSegment(false);
            }
        }
    }

    public void SpawnSegment(bool isSafeStart)
    {
        Vector3 spawnPos = new Vector3(0f, 0f, currentZ);
        GameObject segment = null;

        if (segmentPrefabs != null && segmentPrefabs.Length > 0)
        {
            int index = isSafeStart ? 0 : Random.Range(0, segmentPrefabs.Length);
            segment = Instantiate(segmentPrefabs[index], spawnPos, Quaternion.identity);
        }
        else if (RailwayThemeBuilder.Instance != null)
        {
            int pattern = isSafeStart ? 0 : Random.Range(1, 5);
            segment = RailwayThemeBuilder.Instance.BuildRailwaySegment(spawnPos, segmentLength, pattern);
        }
        else
        {
            segment = FallbackSegment(spawnPos);
        }

        segmentCounter++;
        activeSegments.Add(segment);
        currentZ += segmentLength;
    }

    private GameObject FallbackSegment(Vector3 position)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Segment_" + position.z;
        obj.transform.position = position + new Vector3(0f, -0.5f, segmentLength / 2f);
        obj.transform.localScale = new Vector3(14f, 1f, segmentLength);
        return obj;
    }
}
