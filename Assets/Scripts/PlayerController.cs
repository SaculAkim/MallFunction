using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Beweging Instellingen")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Kijk Instellingen")]
    public Camera playerCamera;
    public float mouseSensitivity = 200f;
    public float lookUpLimit = -90f;
    public float lookDownLimit = 90f;

    [Header("Zoom Instellingen")]
    public float normalFOV = 60f;   // Standaard beeldhoek
    public float zoomFOV = 30f;     // Beeldhoek tijdens inzoomen (lager is verder inzoomen)
    public float zoomSpeed = 10f;   // Hoe vloeiend de zoom gaat

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Cursor verbergen en vastzetten
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
        
        // Zorg dat de camera start op de normale FOV
        playerCamera.fieldOfView = normalFOV;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleZoom();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, lookUpLimit, lookDownLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Shift voor sprinten
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Springen (Spatiebalk)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleZoom()
    {
        // Bepaal de doel-FOV op basis van of 'C' wordt ingedrukt
        float targetFOV = Input.GetKey(KeyCode.C) ? zoomFOV : normalFOV;

        // Gebruik Lerp om de overgang vloeiend te maken
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }
}