using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierShoot : MonoBehaviour
{
    public GameObject bullet;
    public Transform soldier;

    public float timeBetweenShots = 1f; // 1 secunda
    private float timeSinceLastShot; // timer

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (timeSinceLastShot >= timeBetweenShots)
        {
            Debug.Log("mno");
            Instantiate(bullet, soldier.position, soldier.rotation);

            timeSinceLastShot = 0f;
        }
    }
}
