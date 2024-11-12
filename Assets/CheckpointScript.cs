using System;
using TMPro;
using UnityEngine;

public class CheckpointGate : MonoBehaviour
{
    public OperationType option1Operation;
    public int value1;
    public OperationType option2Operation;
    public int value2;

    public TextMeshPro option1Text;
    public TextMeshPro option2Text;

    private void Start()
    {
        UpdateGateLabels();
    }

    private void UpdateGateLabels()
    {
        if (option1Text != null)
        {
            option1Text.text = GetOperationText(option1Operation, value1);
        }

        if (option2Text != null)
        {
            option2Text.text = GetOperationText(option2Operation, value2);
        }
    }

    private string GetOperationText(OperationType operation, int value)
    {
        switch (operation)
        {
            case OperationType.Add:
                return $"+{value}";
            case OperationType.Multiply:
                return $"x{value}";
            case OperationType.Subtract:
                return $"-{value}";
            case OperationType.SqrtAdd:
                return $"√x + {value}";
            default:
                return "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("am intrat");
        if (other.CompareTag("Player"))
        {
            
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (other.bounds.Intersects(transform.Find("GateOption1").GetComponent<Collider>().bounds))
            {
                Debug.Log("am intrat poarta 1");
                playerController.ModifySoldiers(option1Operation, value1);
            }
            else if (other.bounds.Intersects(transform.Find("GateOption2").GetComponent<Collider>().bounds))
            {
                Debug.Log("am intrat poarta 2");
                playerController.ModifySoldiers(option2Operation, value2);
            }
        }
    }
}
