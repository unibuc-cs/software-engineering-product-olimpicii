using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    static PlayerController playerController;

    private void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerController.SplitBigEnemy();
            Destroy(gameObject);
        }
    }
}
