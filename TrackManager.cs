
using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    [Header("Track Settings")]
    public GameObject trackPrefab;
    public Transform player;
    public float segmentLength = 30f;
    public int initialSegments = 5;
    public float spawnAheadDistance = 60f;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public int minCoinsPerSegment = 3;
    public int maxCoinsPerSegment = 8;
    public float coinHeight = 1.2f;

    [Header("Obstacle Settings")]
    public GameObject obstaclePrefab;
    public GameObject slideObstaclePrefab;

    [Range(0f, 1f)]
    public float obstacleChance = 0.7f;

    [Range(0f, 1f)]
    public float slideObstacleChance = 0.35f;

    public float obstacleHeight = 0.75f;
    public float slideObstacleHeight = 1.35f;
    public float obstacleSpacing = 8f;

    [Header("Train Settings")]
    public GameObject trainPrefab;
    [Range(0f, 1f)]
    public float trainChance = 0.3f;
    public float trainHeight = 0f;
    public float trainLength = 12f;

    private float nextSpawnZ = 0f;

    private readonly Queue<GameObject> activeSegments =
        new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (player == null || trackPrefab == null)
            return;

        // Spawn track ahead of the player
        while (nextSpawnZ <
               player.position.z + spawnAheadDistance)
        {
            SpawnSegment();
        }

        // Remove old segments and their contents
        while (activeSegments.Count > 0)
        {
            GameObject oldestSegment = activeSegments.Peek();

            if (oldestSegment.transform.position.z +
                segmentLength < player.position.z - segmentLength)
            {
                Destroy(activeSegments.Dequeue());
            }
            else
            {
                break;
            }
        }
    }

    void SpawnSegment()
    {
        Vector3 spawnPosition =
            new Vector3(0f, -0.5f, nextSpawnZ);

        GameObject segment = Instantiate(
            trackPrefab,
            spawnPosition,
            Quaternion.identity
        );

        activeSegments.Enqueue(segment);

        // Generate the train first to reserve its space
        bool trainSpawned = SpawnTrain(segment.transform, nextSpawnZ);

        SpawnCoins(segment.transform, nextSpawnZ);

        // Avoid spawning other obstacles in the train's segment
        if (!trainSpawned)
        {
            SpawnObstacles(segment.transform, nextSpawnZ);
        }

        nextSpawnZ += segmentLength;
    }

    bool SpawnTrain(Transform segment, float segmentStartZ)
    {
        if (trainPrefab == null || Random.value > trainChance)
            return false;

        // Choose one lane for the train
        int lane = Random.Range(-1, 2);
        float x = lane * 3f;

        Vector3 position = new Vector3(
            x,
            trainHeight,
            segmentStartZ + segmentLength / 2f
        );

        GameObject train = Instantiate(
            trainPrefab,
            position,
            Quaternion.identity
        );

        train.transform.SetParent(segment, true);

        return true;
    }

    void SpawnCoins(Transform segment, float segmentStartZ)
    {
        if (coinPrefab == null)
            return;

        int coinCount = Random.Range(
            minCoinsPerSegment,
            maxCoinsPerSegment + 1
        );

        for (int i = 0; i < coinCount; i++)
        {
            int lane = Random.Range(-1, 2);
            float x = lane * 3f;

            float z = segmentStartZ +
                Random.Range(3f, segmentLength - 3f);

            Vector3 position = new Vector3(
                x, coinHeight, z
            );

            GameObject coin = Instantiate(
                coinPrefab,
                position,
                Quaternion.identity
            );

            coin.transform.SetParent(segment, true);
        }
    }

    void SpawnObstacles(Transform segment, float segmentStartZ)
    {
        if (obstaclePrefab == null &&
            slideObstaclePrefab == null)
            return;

        for (float offset = 6f;
             offset < segmentLength - 3f;
             offset += obstacleSpacing)
        {
            if (Random.value > obstacleChance)
                continue;

            int lane = Random.Range(-1, 2);

            float x = lane * 3f;
            float z = segmentStartZ + offset;

            bool useSlideObstacle =
                slideObstaclePrefab != null &&
                (obstaclePrefab == null ||
                 Random.value < slideObstacleChance);

            GameObject prefab = useSlideObstacle
                ? slideObstaclePrefab
                : obstaclePrefab;

            float y = useSlideObstacle
                ? slideObstacleHeight
                : obstacleHeight;

            GameObject obstacle = Instantiate(
                prefab,
                new Vector3(x, y, z),
                Quaternion.identity
            );

            obstacle.transform.SetParent(segment, true);
        }
    }
}
