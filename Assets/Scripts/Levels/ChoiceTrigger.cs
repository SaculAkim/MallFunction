using UnityEngine;

public class ChoiceTrigger : MonoBehaviour
{
    public AnomalyManager manager; 
    
    [Header("Instellingen")]
    [Tooltip("Geef de ene muur ID 0 en de andere muur ID 1")]
    public int triggerID; 

    [Header("Teleport naar:")]
    public Transform spawnPoint; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (manager != null && spawnPoint != null)
            {
                // Stuur door welke muur is geraakt en waar we heen moeten
                manager.PlayerDecided(triggerID, spawnPoint);
            }
        }
    }
}