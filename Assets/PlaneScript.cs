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
        // Ensure gates spawn with a probability and prefabs are available
        if (UnityEngine.Random.value > 0.5f && gatePrefabs.Count > 0)
        {
            // Randomly select a gate prefab
            GameObject selectedGate = gatePrefabs[UnityEngine.Random.Range(0, gatePrefabs.Count)];

            // Calculate gate position on the platform
            Vector3 gatePosition = platformTransform.position + new Vector3(0f, 0.5f, platformLength / 2f);
            Quaternion gateRotation = Quaternion.Euler(0, 180, 0);

            // Instantiate the gate at the calculated position and rotation
            GameObject gateInstance = Instantiate(selectedGate, gatePosition, gateRotation, gateContainer);
            gateInstance.transform.localScale = Vector3.one;

            // Retrieve the CheckpointGate script from the gate instance
            CheckpointGate gateScript = gateInstance.GetComponent<CheckpointGate>();
            if (gateScript != null)
            {
                // Variables for gate options
                OperationType option1Op, option2Op;
                int option1Val, option2Val;

                // Generate and validate balanced choices for the gate
                do
                {
                    option1Op = GetRandomOperation();
                    option1Val = UnityEngine.Random.Range(1, 10);

                    option2Op = GetRandomOperation();
                    option2Val = UnityEngine.Random.Range(1, 10);
                }
                while (!AreChoicesBalanced(option1Op, option1Val, option2Op, option2Val));

                // Configure the gate with the generated options
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
        // Randomly select an operation from the enum
        Array values = Enum.GetValues(typeof(OperationType));
        return (OperationType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    private bool AreChoicesBalanced(OperationType op1, int val1, OperationType op2, int val2)
    {
       
        // Avoid both options being the same
        if (op1 == op2 && val1 == val2) return false;

        // Avoid one choice being clearly superior (like a large multiplier vs. a small addition)
        if (op1 == OperationType.Multiply && op2 == OperationType.Add && val1 > 3 && val2 < 10) return false;
        if (op1 == OperationType.Add && op2 == OperationType.Multiply && val2 > 3 && val1 < 10) return false;

        // Ensure the options provide meaningful trade-offs
        if (op1 == OperationType.Subtract && op2 == OperationType.Add && val1 > -val2) return false;

        // If using complex operations like Logarithmic, ensure values make sense
        if ((op1 == OperationType.Logarithmic || op2 == OperationType.Logarithmic) && (val1 < 1 || val2 < 1)) return false;

        return true; // Passes balance check
    }
}