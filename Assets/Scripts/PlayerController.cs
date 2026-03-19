using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    public Transform playerCamera;
    public Transform groundCheck;     
    public LayerMask groundMask;      

    [Header("Movement")]
    public float walkSpeed = 7f;      
    public float jumpForce = 12f;     
    public float mouseSensitivity = 2f;

    [Header("Hard Physics")]
    public float gravity = -60f;        
    public float groundDistance = 0.2f; 
    
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Head Wobble")]
    public bool useHeadBob = true;
    public float bobFrequency = 12f;    
    public float bobVerticalAmount = 0.06f;   
    public float bobHorizontalAmount = 0.1f; 
    public float tiltAmount = 2.0f;          

    private float xRotation = 0f;
    private Vector3 defaultCameraPos; 
    private float bobTimer;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        defaultCameraPos = playerCamera.localPosition;
    }

    void Update()
    {
        // 1. DE GROND CHECK (Verbeterd)
        // We gebruiken nu OverlapSphere om te kijken wat er onder ons zit
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);
        
        isGrounded = false;
        foreach (var col in colliders)
        {
            // Als we iets raken dat NIET de speler zelf is, dan zijn we grounded
            if (col.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, playerCamera.localRotation.eulerAngles.z);
        transform.Rotate(Vector3.up * mouseX);

        // 3. Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);

        // 4. JUMP (Nu echt alleen als isGrounded true is)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpForce;
        }

        // 5. Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 6. Head Wobble
        if (useHeadBob) HandleHeadWobble();
    }

    private void HandleHeadWobble()
    {
        bool isMoving = (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f) && isGrounded;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float newX = Mathf.Sin(bobTimer) * bobHorizontalAmount;
            float newY = Mathf.Cos(bobTimer * 2) * bobVerticalAmount; 
            float newZ = -Mathf.Sin(bobTimer) * tiltAmount;

            playerCamera.localPosition = new Vector3(defaultCameraPos.x + newX, defaultCameraPos.y + newY, defaultCameraPos.z);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, newZ);
        }
        else
        {
            bobTimer = 0;
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, defaultCameraPos, Time.deltaTime * 10f);
            playerCamera.localRotation = Quaternion.Slerp(playerCamera.localRotation, Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * 10f);
        }
    }

    // Dit tekent het bolletje in de Editor zodat je kunt zien waar de check plaatsvindt
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groundDistance);
        }
    }
}