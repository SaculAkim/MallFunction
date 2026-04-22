using UnityEngine;

public class CameraTagFollow : MonoBehaviour
{
    [Header("Instellingen")]
    public string playerTag = "Player"; 
    public float detectionRange = 15f; 
    public float rotationSpeed = 3f;   

    [Header("Model Correctie")]
    // Vul hier de waarden in uit je screenshot als hij nog steeds verkeerd kijkt
    public Vector3 rotationOffset = new Vector3(0, 0, 0); 

    private Transform playerTransform;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRange)
        {
            // Richting naar de speler
            Vector3 direction = playerTransform.position - transform.position;
            
            // Maak de basis rotatie naar de speler
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Voeg de correctie van je model toe
            Quaternion finalRotation = lookRotation * Quaternion.Euler(rotationOffset);

            // Draai soepel
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * (rotationSpeed / 2));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}