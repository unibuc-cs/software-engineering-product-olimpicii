using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    static public GameObject player;
    public float enemySpeed = 1f;
    public float pullStrength = 0.06f;
    private Rigidbody rb;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody not found! Please add a Rigidbody component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    void EnemyMovement()
    {
        PullToPlayer();
        MoveForward();
    }

    void PullToPlayer()
    {
        Vector3 direction = (player.transform.position - rb.position).normalized;
        rb.AddForce(direction * pullStrength, ForceMode.Impulse);
    }

    void MoveForward()
    {
        rb.AddForce(new Vector3(0f, 0f, enemySpeed));
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        GameObject otherObject = collision.gameObject;

        switch (tag)
        {
            case "Bullet":
                Destroy(gameObject);
                Destroy(collision.gameObject);
                break;
            case "Soldier":
                Destroy(gameObject);
                Destroy(collision.gameObject);
                break;
            default:
                Debug.Log("you've just hit a " + tag);
                break;
        }


    }

}

