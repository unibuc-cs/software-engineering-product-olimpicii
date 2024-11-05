using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public GameObject platformPrefab;
    public List<GameObject> gatePrefabs;
    public int platformLength = 20;
    public int initialPlatformCount = 5;
    public Transform playerTransform;
    private Queue<GameObject> activePlatforms = new Queue<GameObject>();
    private float spawnZ = 0f;
    private float safeZone = 30f;

    void Start()
    {
        for (int i = 0; i < initialPlatformCount; i++)
            SpawnPlatform(true);
    }

    void Update()
    {
        if (playerTransform.position.z - safeZone > (spawnZ - initialPlatformCount * platformLength))
        {
            SpawnPlatform(false);
            DeletePlatform();
        }
    }

    void SpawnPlatform(bool initialSpawn)
    {
        GameObject newPlatform = Instantiate(platformPrefab, Vector3.forward * spawnZ, Quaternion.identity);
        activePlatforms.Enqueue(newPlatform);

        if (!initialSpawn)
            SpawnGate(newPlatform.transform);

        spawnZ += platformLength;
    }

    void SpawnGate(Transform platformTransform)
    {
        if (Random.value > 0.5f && gatePrefabs.Count > 0)
        {
            GameObject selectedGate = gatePrefabs[Random.Range(0, gatePrefabs.Count)];
            float gateX = Random.Range(-3f, 3f);
            Vector3 gatePosition = platformTransform.position + new Vector3(gateX, 1f, platformLength / 2f);
            Instantiate(selectedGate, gatePosition, Quaternion.identity, platformTransform);
        }
    }

    void DeletePlatform()
    {
        GameObject oldPlatform = activePlatforms.Dequeue();
        Destroy(oldPlatform);
    }
}
