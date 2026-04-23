using UnityEngine;

[RequireComponent(typeof(Flashlight))]
[RequireComponent(typeof(Rigidbody))]
public class ItemPickup : MonoBehaviour
{
    [Header("Instellingen")]
    public string playerTag = "Player";    // Zorg dat je speler de tag 'Player' heeft
    public KeyCode pickupKey = KeyCode.E;   // De toets om op te pakken
    public float pickupDistance = 3.0f;     // Hoe dichtbij je moet staan

    [Header("Referenties")]
    public Transform holdParent;            // Sleep hier je 'FlashlightHoldPlace' in (onder de camera)

    private Flashlight flashlightScript;
    private Rigidbody rb;
    private Transform playerTransform;

    void Awake()
    {
        flashlightScript = GetComponent<Flashlight>();
        rb = GetComponent<Rigidbody>();

        // Voorkom dat de zaklamp direct door de map valt bij start
        rb.isKinematic = true; 
        rb.useGravity = false;
    }

    void Start()
    {
        // Zoek de speler in de scene
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("FOUT: Geen object gevonden met tag 'Player'. Pas de tag van je speler aan!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Check de afstand tussen de speler en de zaklamp
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= pickupDistance)
        {
            // Optioneel: Hier kun je een 'Press E' UI tonen
            if (Input.GetKeyDown(pickupKey))
            {
                DoPickup();
            }
        }
    }

    void DoPickup()
    {
        if (holdParent == null)
        {
            Debug.LogError("FOUT: Geen 'Hold Parent' toegewezen in de Inspector van de zaklamp!");
            return;
        }

        // 1. Schakel physics volledig uit zodat hij niet uit je hand valt
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.detectCollisions = false;

        // 2. Roep de PickUp functie aan uit jouw originele Flashlight script
        // Dit regelt de parenting en zet de positie op Vector3.zero
        flashlightScript.PickUp(holdParent);

        // 3. Forceer de schaal naar 1 (zoals gevraagd)
        transform.localScale = Vector3.one; 

        // 4. Verwijder dit script van de zaklamp zodat je niet opnieuw 'E' kunt drukken
        Debug.Log("Zaklamp succesvol opgepakt!");
        Destroy(this);
    }
}