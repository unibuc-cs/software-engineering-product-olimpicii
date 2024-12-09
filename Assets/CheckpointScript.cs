using System;
using TMPro;
using UnityEngine;

public class CheckpointGate : MonoBehaviour
{
    public GateOption gateOption1; // Reference to GateOption1 script
    public GateOption gateOption2; // Reference to GateOption2 script

    public TextMeshPro option1Text;
    public TextMeshPro option2Text;

    private void Start()
    {
        UpdateGateLabels();
    }

    public void ConfigureGate(OperationType option1Op, int option1Val, OperationType option2Op, int option2Val)
    {
        if (gateOption1 != null)
        {
            gateOption1.operation = option1Op;
            gateOption1.value = option1Val;
        }

        if (gateOption2 != null)
        {
            gateOption2.operation = option2Op;
            gateOption2.value = option2Val;
        }

        UpdateGateLabels();
    }

    private void UpdateGateLabels()
    {
        if (option1Text != null && gateOption1 != null)
        {
            option1Text.text = GetOperationText(gateOption1.operation, gateOption1.value);
        }

        if (option2Text != null && gateOption2 != null)
        {
            option2Text.text = GetOperationText(gateOption2.operation, gateOption2.value);
        }
    }

    private string GetOperationText(OperationType operation, int value)
    {
        return operation switch
        {
            OperationType.Add => $"+{value}",
            OperationType.Multiply => $"x{value}",
            OperationType.Subtract => $"-{value}",
            OperationType.SqrtAdd => $"√x + {value}",
            _ => ""
        };
    }
}
