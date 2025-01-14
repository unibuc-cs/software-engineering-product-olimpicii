using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;


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
        UpdateScoreTable();
    }

    private void UpdateScoreTable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.UpdateScore(soldiers.Count + bigSoldiers.Count);
        }
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

        if (soldiers.Count == 0 && bigSoldiers.Count == 0)
        {
            gameManager.EndGame();  
        }

        makeBigSoldiers();
        UpdateScoreTable();
    }

    void ClampPlayerPosition()
    {
        
        float currentX = transform.position.x;

        // Clamp the player's x position 
        float clampedX = Mathf.Clamp(currentX, -2.70f, 4f);

        // If the player's position exceeds the X boundaries
        if (currentX <= -2.70f)
        {
            clampedX = -2.70f;
        }
        else if (currentX >= 4f)
        {
            clampedX = 4f;
        }

        
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
        transform.Translate(Vector3.left * playerMoveSpeed * Time.deltaTime * 0.65f);
    }

    void MoveRight()
    {
        transform.Translate(Vector3.right * playerMoveSpeed * Time.deltaTime * 0.65f);
    }

    public void ModifySoldiers(OperationType operation, int value)
    {
        int currentCount = soldiers.Count + bigSoldiers.Count * bigToSmallSoldierRatio;

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

        // allowed width
        float maxAllowedWidth = 4f - (-2.70f);

        
        if (soldiers.Count > 0)
        {
            float idealSpacing = 0.3f; 
            float spacing = Mathf.Min(idealSpacing, maxAllowedWidth / Mathf.Max(1, soldiers.Count)); 

            float totalWidth = spacing * (soldiers.Count - 1);
            float centerOffset = -totalWidth / 2f; // center soldiers

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

                
                Vector3 direction = (transform.position - soldier.transform.position).normalized;
                rb.AddForce(direction * pullStrength, ForceMode.Impulse);

                
                float targetX = clampedPlayerX + centerOffset + (i * spacing);
                float clampedX = Mathf.Clamp(targetX, -2.70f, 4f);

                Vector3 soldierPosition = soldier.transform.position;
                soldierPosition.x = clampedX;
                soldier.transform.position = soldierPosition;
            }
        }

        
        if (bigSoldiers.Count > 0)
        {
            float idealSpacing = 0.7f; 
            float spacing = Mathf.Min(idealSpacing, maxAllowedWidth / Mathf.Max(1, bigSoldiers.Count)); 

            float totalWidth = spacing * (bigSoldiers.Count - 1);
            float centerOffset = -totalWidth / 2f; 

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

                
                Vector3 direction = (transform.position - soldier.transform.position).normalized;
                rb.AddForce(direction * pullStrength, ForceMode.Impulse);

                
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
        while (bigSoldiers.Count > 0 && count > 0)
        {
            Destroy(bigSoldiers[bigSoldiers.Count - 1]);
            bigSoldiers.RemoveAt(bigSoldiers.Count - 1);

            count -= bigToSmallSoldierRatio;
        }

        if (count < 0)
        {
            SpawnSoldiers(-count);
            return;
        }

        for (int i = 0; i < count && soldiers.Count > 0; i++)
        {
            GameObject soldierToRemove = soldiers[soldiers.Count - 1];
            soldiers.Remove(soldierToRemove);
            Destroy(soldierToRemove);
        }
    }
}
