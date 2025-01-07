using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector3(0, 0, speed);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("am nimerit cv");
    }
}
