using System;
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
    public Transform gateContainer;

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
        if (UnityEngine.Random.value > 0.5f && gatePrefabs.Count > 0)
        {
            GameObject selectedGate = gatePrefabs[UnityEngine.Random.Range(0, gatePrefabs.Count)];
            Vector3 gatePosition = platformTransform.position + new Vector3(0f, 0.5f, platformLength / 2f);
            Quaternion gateRotation = Quaternion.Euler(0, 180, 0);

            GameObject gateInstance = Instantiate(selectedGate, gatePosition, gateRotation, gateContainer);
            gateInstance.transform.localScale = Vector3.one;

            CheckpointGate gateScript = gateInstance.GetComponent<CheckpointGate>();
            if (gateScript != null)
            {
                OperationType option1Op = (OperationType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(OperationType)).Length);
                OperationType option2Op = (OperationType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(OperationType)).Length);
                int option1Val = UnityEngine.Random.Range(1, 10);
                int option2Val = UnityEngine.Random.Range(1, 10);

                gateScript.ConfigureGate(option1Op, option1Val, option2Op, option2Val);
            }
        }
    }

    void DeletePlatform()
    {
        GameObject oldPlatform = activePlatforms.Dequeue();

        foreach (Transform child in gateContainer)
        {
            if (child.position.z < oldPlatform.transform.position.z)
            {
                Destroy(child.gameObject);
            }
        }

        Destroy(oldPlatform);
    }
}