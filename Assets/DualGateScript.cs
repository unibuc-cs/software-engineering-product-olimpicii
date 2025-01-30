using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DualCheckpointGate : MonoBehaviour
{
    public ShootableGate breakableBarrel; 
    public GateOption punishmentGate; 

    public TextMeshPro punishmentText; 
    private bool barrelDestroyed = false; 

    private void Start()
    {
        UpdateGateLabels();
    }

    public void ConfigureGate(int difficultyLevel)
    {
        
        if (breakableBarrel != null)
        {
            breakableBarrel.health = Mathf.RoundToInt(10 + (difficultyLevel * 2)); 
            breakableBarrel.onDestroyed += OnBarrelDestroyed; 
        }

      
        if (punishmentGate != null)
        {
            punishmentGate.operation = OperationType.Subtract;
            punishmentGate.value = UnityEngine.Random.Range(5, 15); 
        }

        UpdateGateLabels();
    }

    private void UpdateGateLabels()
    {
        if (punishmentText != null && punishmentGate != null)
        {
            punishmentText.text = "- " + punishmentGate.value; 
        }
    }

 
    private void OnBarrelDestroyed()
    {
        barrelDestroyed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!barrelDestroyed)
            {
                Destroy(other.gameObject);
            }
             
        }
    }
}
