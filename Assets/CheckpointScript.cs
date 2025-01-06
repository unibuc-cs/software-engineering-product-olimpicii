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

    public void ConfigureGateWithLogic(int difficultyLevel)
    {
        OperationType option1Op = GetRandomOperation();
        OperationType option2Op = GetRandomOperation();

        int baseValue = Mathf.Max(1, difficultyLevel); // Difficulty scales with player progress
        int option1Val = GenerateValue(option1Op, baseValue);
        int option2Val = GenerateValue(option2Op, baseValue);

        while (!AreChoicesBalanced(option1Op, option1Val, option2Op, option2Val))
        {
            option1Op = GetRandomOperation();
            option1Val = GenerateValue(option1Op, baseValue);

            option2Op = GetRandomOperation();
            option2Val = GenerateValue(option2Op, baseValue);
        }

        ConfigureGate(option1Op, option1Val, option2Op, option2Val);
    }

    private int GenerateValue(OperationType operation, int baseValue)
    {
        return operation switch
        {
            OperationType.Add => UnityEngine.Random.Range(baseValue, baseValue + 5),
            OperationType.Multiply => UnityEngine.Random.Range(2, 5),
            OperationType.Subtract => -UnityEngine.Random.Range(1, baseValue),
            OperationType.SqrtAdd => UnityEngine.Random.Range(baseValue, baseValue + 3),
            OperationType.Logarithmic => Mathf.RoundToInt(Mathf.Log(UnityEngine.Random.Range(baseValue, baseValue + 20), 2)), // Log base 2
            OperationType.Equation => UnityEngine.Random.Range(1, 10) * UnityEngine.Random.Range(1, 5), // Simple equation multiplier
            _ => 0
        };
    }

    private string GetOperationText(OperationType operation, int value)
    {
        return operation switch
        {
            OperationType.Add => $"+{value}",
            OperationType.Multiply => $"x{value}",
            OperationType.Subtract => $"-{value}",
            OperationType.SqrtAdd => $"√x + {value}",
            OperationType.Logarithmic => $"Log2({value})",
            OperationType.Equation => $"x({value})", // Representation of equation
            _ => ""
        };
    }
    private OperationType GetRandomOperation()
    {
        Array operations = Enum.GetValues(typeof(OperationType));
        return (OperationType)operations.GetValue(UnityEngine.Random.Range(0, operations.Length));
    }
    private bool AreChoicesBalanced(OperationType op1, int val1, OperationType op2, int val2)
    {
        // Example rules for balance:
        // Avoid both options being the same
        if (op1 == op2 && val1 == val2) return false;

        // Avoid one choice being clearly superior (like a large multiplier vs. a small addition)
        if (op1 == OperationType.Multiply && op2 == OperationType.Add && val1 > 3 && val2 < 10) return false;
        if (op1 == OperationType.Add && op2 == OperationType.Multiply && val2 > 3 && val1 < 10) return false;

        // Ensure the options provide meaningful trade-offs
        if (op1 == OperationType.Subtract && op2 == OperationType.Add && val1 > -val2) return false;

        // If using complex operations like Logarithmic, ensure values make sense
        if ((op1 == OperationType.Logarithmic || op2 == OperationType.Logarithmic) && (val1 < 1 || val2 < 1)) return false;

        return true; // Passes balance check
    }
}
