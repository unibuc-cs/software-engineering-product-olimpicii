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
    public Transform gateContainer;
    public float gateSpacing = 40f; 

    private Queue<GameObject> activePlatforms = new Queue<GameObject>();
    private float spawnZ = 0f;
    private float safeZone = 30f;
    private float nextGateZ = 0f; 

    void Start()
    {
        nextGateZ = gateSpacing; 

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

        if (!initialSpawn && spawnZ >= nextGateZ)
        {
            SpawnGate(newPlatform.transform);
            nextGateZ += gateSpacing; 
        }

        spawnZ += platformLength;
    }

    void SpawnGate(Transform platformTransform)
    {
        if (gatePrefabs.Count > 0)
        {

            GameObject selectedGate = gatePrefabs[UnityEngine.Random.Range(0, gatePrefabs.Count)];


            Vector3 gatePosition = platformTransform.position + new Vector3(0f, 0.5f, platformLength / 2f);
            Quaternion gateRotation = Quaternion.Euler(0, 180, 0);

            GameObject gateInstance = Instantiate(selectedGate, gatePosition, gateRotation, gateContainer);
            gateInstance.transform.localScale = Vector3.one;

            CheckpointGate gateScript = gateInstance.GetComponent<CheckpointGate>();
            if (gateScript != null)
            {
                OperationType option1Op, option2Op;
                int option1Val, option2Val;

                do
                {
                    option1Op = GetRandomOperation();
                    option1Val = UnityEngine.Random.Range(1, 10);

                    option2Op = GetRandomOperation();
                    option2Val = UnityEngine.Random.Range(1, 10);
                }
                while (!AreChoicesBalanced(option1Op, option1Val, option2Op, option2Val));

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

    private OperationType GetRandomOperation()
    {
        Array values = Enum.GetValues(typeof(OperationType));
        return (OperationType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    private bool AreChoicesBalanced(OperationType op1, int val1, OperationType op2, int val2)
    {
        if (op1 == op2 && val1 == val2) return false;
        if (op1 == OperationType.Multiply && op2 == OperationType.Add && val1 > 3 && val2 < 10) return false;
        if (op1 == OperationType.Add && op2 == OperationType.Multiply && val2 > 3 && val1 < 10) return false;
        if (op1 == OperationType.Subtract && op2 == OperationType.Add && val1 > -val2) return false;

        return true;
    }
}
