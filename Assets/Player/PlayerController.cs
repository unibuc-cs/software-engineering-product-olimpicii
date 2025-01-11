using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pullStrength;
    public int bigToSmallSoldierRatio;

    public GameObject soldierPrefab;
    public GameObject bigSoldierPrefab;
    private Rigidbody rb;
    private List<GameObject> soldiers = new List<GameObject>();
    private List<GameObject> bigSoldiers = new List<GameObject>();
    public float moveSpeed = 5f;
    public float forwardSpeed = 10f;
    public float laneWidth = 3f;
    public float playerMoveSpeed;

    private float targetXPosition = 0f;

    void Start()
    {
        SpawnSoldiers(1); // Spawn initial soldier
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Continuous forward movement
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }


        ClampPlayerPosition();
        PullSoldiersCloser();

        //Vector3 targetPosition = new Vector3(targetXPosition, transform.position.y, transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        makeBigSoldiers();
    }

    void ClampPlayerPosition()
    {
        // Get the player's current position on the X-axis
        float currentX = transform.position.x;

        // Clamp the player's x position between the defined minX and maxX
        // However, instead of clamping to the initial values, we will adjust to the extreme values dynamically
        float clampedX = Mathf.Clamp(currentX, -2.70f, 4f);

        // If the player's position exceeds the X boundaries, snap to the extremity (current maximum or minimum)
        if (currentX <= -2.70f)
        {
            clampedX = -2.70f;
        }
        else if (currentX >= 4f)
        {
            clampedX = 4f;
        }

        // Apply the new clamped position to the player's position
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    void makeBigSoldiers()
    {
        if(soldiers.Count > bigToSmallSoldierRatio)
        {
            int bigSoldiersToSpawn = soldiers.Count / bigToSmallSoldierRatio;

            for (int i = 0; i < bigSoldiersToSpawn * bigToSmallSoldierRatio; i++)
            {
                Destroy(soldiers[i]);
            }
            soldiers.RemoveRange(0, bigSoldiersToSpawn * bigToSmallSoldierRatio);

            SpawnBigSoldiers(bigSoldiersToSpawn);
        }
    }

    public void SplitBigEnemy()
    {
        SpawnSoldiers(bigToSmallSoldierRatio);
    }

    void MoveLeft()
    {
        transform.Translate(Vector3.left * playerMoveSpeed * Time.deltaTime);
    }

    void MoveRight()
    {
        transform.Translate(Vector3.right * playerMoveSpeed * Time.deltaTime);
    }

    public void ModifySoldiers(OperationType operation, int value)
    {
        int currentCount = soldiers.Count;

        int newSoldierCount = currentCount;

        switch (operation)
        {
            case OperationType.Add:
                newSoldierCount = currentCount + value;
                break;
            case OperationType.Multiply:
                newSoldierCount = currentCount * value;
                break;
            case OperationType.Subtract:
                newSoldierCount = Mathf.Max(currentCount - value, 0);
                break;
            case OperationType.SqrtAdd:
                newSoldierCount = Mathf.CeilToInt(Mathf.Sqrt(currentCount)) + value;
                break;
        }

        int difference = newSoldierCount - currentCount;
        if (difference > 0)
            SpawnSoldiers(difference);
        else
            RemoveSoldiers(-difference);

 


    }

    void SpawnSoldiers(int count)
    {

        int gridRows = Mathf.CeilToInt(Mathf.Sqrt(count));
        int gridCols = Mathf.CeilToInt((float)count / gridRows);
        float spacing = Mathf.Max(0.8f, 5f / Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            int row = i / gridCols;
            int col = i % gridCols;

            Vector3 spawnOffset = new Vector3((col - gridCols / 2f) * spacing, 0, (row - gridRows / 2f) * spacing);
            Vector3 spawnPosition = transform.position + spawnOffset;

            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -2.70f, 4f);


            GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
            soldiers.Add(soldier);
            soldier.transform.parent = transform;

        }
    }

    void SpawnBigSoldiers(int count)  // cam pucheala codu aici dar ar dura mai mult sa l fac sa accepte orice tip ca ar trb sa fac un hashmap cu liste sau n am alta idee :(
    {

        int gridRows = Mathf.CeilToInt(Mathf.Sqrt(count));
        int gridCols = Mathf.CeilToInt((float)count / gridRows);
        float spacing = Mathf.Max(0.8f, 5f / Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            int row = i / gridCols;
            int col = i % gridCols;

            Vector3 spawnOffset = new Vector3((col - gridCols / 2f) * spacing, 0, (row - gridRows / 2f) * spacing);
            Vector3 spawnPosition = transform.position + spawnOffset;


            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -2.70f, 4f);
            GameObject soldier = Instantiate(bigSoldierPrefab, spawnPosition, Quaternion.identity);
            bigSoldiers.Add(soldier);
            soldier.transform.parent = transform;
        }
    }


    void PullSoldiersCloser()
    {
        float clampedPlayerX = Mathf.Clamp(transform.position.x, -2.70f, 4f);
        transform.position = new Vector3(clampedPlayerX, transform.position.y, transform.position.z);

        // Calculate the allowed width of the road
        float maxAllowedWidth = 4f - (-2.70f);

        // Process small soldiers
        if (soldiers.Count > 0)
        {
            float idealSpacing = 0.3f; // Ideal spacing between soldiers
            float spacing = Mathf.Min(idealSpacing, maxAllowedWidth / Mathf.Max(1, soldiers.Count)); // Dynamically adjust spacing

            float totalWidth = spacing * (soldiers.Count - 1);
            float centerOffset = -totalWidth / 2f; // Center soldiers closer to the player

            for (int i = 0; i < soldiers.Count; i++)
            {
                GameObject soldier = soldiers[i];

                if (soldier == null)
                {
                    soldiers.RemoveAt(i);
                    i--;
                    continue;
                }

                Rigidbody rb = soldier.GetComponent<Rigidbody>();

                // Pull soldier towards the player
                Vector3 direction = (transform.position - soldier.transform.position).normalized;
                rb.AddForce(direction * pullStrength, ForceMode.Impulse);

                // Calculate target position with compact spacing
                float targetX = clampedPlayerX + centerOffset + (i * spacing);
                float clampedX = Mathf.Clamp(targetX, -2.70f, 4f);

                Vector3 soldierPosition = soldier.transform.position;
                soldierPosition.x = clampedX;
                soldier.transform.position = soldierPosition;
            }
        }

        // Process big soldiers
        if (bigSoldiers.Count > 0)
        {
            float idealSpacing = 0.7f; // Ideal spacing for bigger soldiers
            float spacing = Mathf.Min(idealSpacing, maxAllowedWidth / Mathf.Max(1, bigSoldiers.Count)); // Dynamically adjust spacing

            float totalWidth = spacing * (bigSoldiers.Count - 1);
            float centerOffset = -totalWidth / 2f; // Center soldiers closer to the player

            for (int i = 0; i < bigSoldiers.Count; i++)
            {
                GameObject soldier = bigSoldiers[i];

                if (soldier == null)
                {
                    bigSoldiers.RemoveAt(i);
                    i--;
                    continue;
                }

                Rigidbody rb = soldier.GetComponent<Rigidbody>();

                // Pull soldier towards the player
                Vector3 direction = (transform.position - soldier.transform.position).normalized;
                rb.AddForce(direction * pullStrength, ForceMode.Impulse);

                // Calculate target position with compact spacing
                float targetX = clampedPlayerX + centerOffset + (i * spacing);
                float clampedX = Mathf.Clamp(targetX, -2.70f, 4f);

                Vector3 soldierPosition = soldier.transform.position;
                soldierPosition.x = clampedX;
                soldier.transform.position = soldierPosition;
            }
        }
    }








    public void RemoveSoldiers(int count)
    {
        for (int i = 0; i < count && soldiers.Count > 0; i++)
        {
            GameObject soldierToRemove = soldiers[soldiers.Count - 1];
            soldiers.Remove(soldierToRemove);
            Destroy(soldierToRemove);
        }
    }
}
