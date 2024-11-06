using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject soldierPrefab;
    public List<GameObject> soldiers = new List<GameObject>();
    public float moveSpeed = 5f;
    public float forwardSpeed = 10f;
    public float laneWidth = 3f; // Width of each "lane" distance

    private float targetXPosition = 0f; 

    void Start()
    {
        SpawnSoldiers(1); // Spawn initial soldiers
    }

    void Update()
    {
        // Continuous forward movement
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

     
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }

        Vector3 targetPosition = new Vector3(targetXPosition, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }


    void MoveLeft()
    {
        
        targetXPosition -= laneWidth;
    }

 
    void MoveRight()
    {
        targetXPosition += laneWidth;
    }

    public void ModifySoldiers(string operation, int value)
    {
        int currentCount = soldiers.Count;
        if (operation == "multiply")
        {
            int newCount = currentCount * value;
            SpawnSoldiers(newCount - currentCount);
        }
        else if (operation == "add")
        {
            SpawnSoldiers(value);
        }
    }

    void SpawnSoldiers(int count)
    {
        int gridRows = Mathf.CeilToInt(Mathf.Sqrt(count)); 
        int gridCols = Mathf.CeilToInt((float)count / gridRows); // sa fie in grup soldatii
        float spacing = 1.5f; 

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
