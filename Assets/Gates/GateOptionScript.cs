using UnityEngine;

public class GateOption : MonoBehaviour
{
    public OperationType operation;
    public int value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{name} triggered!");
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ModifySoldiers(operation, value);
            }
        }
    }
}

