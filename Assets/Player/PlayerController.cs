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

        PullSoldiersCloser();

        //Vector3 targetPosition = new Vector3(targetXPosition, transform.position.y, transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        makeBigSoldiers();
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
        float spacing = 1.8f;

        for (int i = 0; i < count; i++)
        {
            int row = i / gridCols;
            int col = i % gridCols;

            Vector3 spawnOffset = new Vector3((col - gridCols / 2f) * spacing, 0, (row - gridRows / 2f) * spacing);
            Vector3 spawnPosition = transform.position + spawnOffset;


            GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
            soldiers.Add(soldier);
            soldier.transform.parent = transform;
        }
    }

    void SpawnBigSoldiers(int count)  // cam pucheala codu aici dar ar dura mai mult sa l fac sa accepte orice tip ca ar trb sa fac un hashmap cu liste sau n am alta idee :(
    {

        int gridRows = Mathf.CeilToInt(Mathf.Sqrt(count));
        int gridCols = Mathf.CeilToInt((float)count / gridRows);
        float spacing = 1.8f;

        for (int i = 0; i < count; i++)
        {
            int row = i / gridCols;
            int col = i % gridCols;

            Vector3 spawnOffset = new Vector3((col - gridCols / 2f) * spacing, 0, (row - gridRows / 2f) * spacing);
            Vector3 spawnPosition = transform.position + spawnOffset;


            GameObject soldier = Instantiate(bigSoldierPrefab, spawnPosition, Quaternion.identity);
            bigSoldiers.Add(soldier);
            soldier.transform.parent = transform;
        }
    }


    void PullSoldiersCloser()
    {
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
        }

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
