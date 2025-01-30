using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public GameObject platformPrefab;
    public List<GameObject> checkpointGatePrefabs; // Checkpoint gate prefabs
    public GameObject shootableGatePrefab;        // Shootable gate prefab
    public GameObject dualCheckpointPrefab;       //dual gate
    public int platformLength = 20;
    public int initialPlatformCount = 5;
    public int gateDistanceToPlayer;
    public Transform playerTransform;
    public Transform gateContainer;
    public float gateSpacing = 50f;

    private Queue<GameObject> activePlatforms = new Queue<GameObject>();
    private int platformsSpawned;
    private int gatesSpawned;
    private int shootableGateCounter = 0;        

    private int shootableGateHealth = 5;         
    private float healthScalingFactor = 5;
    private int enemyScalingFactor = 5;
    private EnemyController enemyControllerScript;

    private int whatToSpawn = 0;

    void Start()
    {
        Transform enemyController = GameObject.Find("enemySpawnPoint").transform;
        enemyControllerScript = enemyController.GetComponent<EnemyController>();

        for (int i = 0; i < initialPlatformCount; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        if (playerTransform.position.z > platformsSpawned * platformLength)
        {
            platformsSpawned++;
            SpawnPlatform();
            DeletePlatform();
        }

        if (playerTransform.position.z > gatesSpawned * gateSpacing)
        {
            whatToSpawn++;

            switch (whatToSpawn)
            {
                case 1:
                    SpawnCheckpointGate(); break;
                case 2:
                    SpawnShootableGate(); break;
                case 3:
                    SpawnDualCheckpointGate(); break;
                case 4:
                    SpawnEnemies(); break;
                default:
                    whatToSpawn = 0; break;
            }

            gatesSpawned++;
        }
    }

    void SpawnEnemies()
    {
        enemyControllerScript.SpawnEnemies(enemyScalingFactor + 5);
        enemyScalingFactor += 5;
    }
    

    void SpawnPlatform()
    {
        Vector3 platformPosition = Vector3.forward * platformLength * platformsSpawned;
        GameObject newPlatform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);
        activePlatforms.Enqueue(newPlatform);
    }

    void SpawnCheckpointGate()
    {
        if (checkpointGatePrefabs.Count > 0)
        {
            GameObject selectedGate = checkpointGatePrefabs[UnityEngine.Random.Range(0, checkpointGatePrefabs.Count)];
            Vector3 gatePosition = new Vector3(0f, 0f, gateDistanceToPlayer + playerTransform.position.z);
            Quaternion gateRotation = Quaternion.Euler(0, 180, 0);

            GameObject gateInstance = Instantiate(selectedGate, gatePosition, gateRotation, gateContainer);
            gateInstance.transform.localScale = Vector3.one;

            CheckpointGate gateScript = gateInstance.GetComponent<CheckpointGate>();
            if (gateScript != null)
            {
              
                OperationType op1, op2;
                int val1, val2;
                do
                {
                    op1 = GetRandomOperation();
                    val1 = UnityEngine.Random.Range(2, 7);

                    op2 = GetRandomOperation();
                    val2 = UnityEngine.Random.Range(2, 7);
                }
                while (!AreChoicesBalanced(op1, val1, op2, val2));

                gateScript.ConfigureGate(op1, val1, op2, val2);
            }
        }
    }

    void SpawnShootableGate()
    {
        if (shootableGatePrefab != null)
        {
            Vector3 gatePosition = new Vector3(-4f, 0f, gateDistanceToPlayer + playerTransform.position.z);
            Quaternion gateRotation = Quaternion.Euler(270, 0, 0);

            GameObject shootableGate = Instantiate(shootableGatePrefab, gatePosition, gateRotation, gateContainer);
            shootableGate.transform.localScale = new Vector3(3.8f, 4f, 4f); 

            ShootableGate shootableGateScript = shootableGate.GetComponent<ShootableGate>();
            if (shootableGateScript != null)
            {
                shootableGateScript.health = shootableGateHealth;
                shootableGateHealth = Mathf.RoundToInt(shootableGateHealth + healthScalingFactor); 
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
        if (op1 == op2 ) return false;
        if (op1 == OperationType.Multiply && op2 == OperationType.Add && val1 > 3 && val2 < 10) return false;
        if (op1 == OperationType.Add && op2 == OperationType.Multiply && val2 > 3 && val1 < 10) return false;
        if (op1 == OperationType.Subtract && op2 == OperationType.Add && val1 > -val2) return false;
        return true;
    }

    void SpawnDualCheckpointGate()
    {
        if (dualCheckpointPrefab != null)
        {
            Vector3 gatePosition = new Vector3(0f, 0f, gateDistanceToPlayer + playerTransform.position.z);
            Quaternion gateRotation = Quaternion.Euler(0, 180, 0);


            GameObject dualGateInstance = Instantiate(dualCheckpointPrefab, gatePosition, gateRotation, gateContainer);
            dualGateInstance.transform.localScale = Vector3.one;

            DualCheckpointGate dualGateScript = dualGateInstance.GetComponent<DualCheckpointGate>();
            if (dualGateScript != null)
            {
                dualGateScript.ConfigureGate(gatesSpawned);
            }
        }
    }

}