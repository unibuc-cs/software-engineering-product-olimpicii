using TMPro;
using UnityEngine;

public class ShootableGate : MonoBehaviour
{
    public int health = 10;
    public GameObject destructionEffect;
    public TextMeshPro healthText;
    public delegate void OnDestroyed();
    public event OnDestroyed onDestroyed; 

    void Start()
    {
        UpdateHealthText();
    }



    void TakeDamage(int damage)
    {
        health -= damage;
        //Debug.Log("damage");
        UpdateHealthText();


        if (health <= 0)
        {
            DestroyGate();
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }
    }

    void DestroyGate()
    {

        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }
        onDestroyed?.Invoke();
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
            //Debug.Log("glont lovit");

        }

        if (other.CompareTag("Soldier"))
        {
            Destroy(other.gameObject);

        }
    }
}