using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float crouchSpeed = 2.5f;
    public float mouseSensitivity = 2.0f;
    public float maxStamina = 100.0f;
    public float staminaDecreaseRate = 0f;
    public float jumpForce = 5.0f;
    public float gravity = 9.8f;
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;
    public Camera playerCamera;
    private CharacterController characterController;
    private float currentStamina;
    private float verticalSpeed = 0.0f;
    private bool isGrounded = false;
    private float cameraVerticalAngle = 0.0f;
    private bool isCrouching = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentStamina = maxStamina;
    }

    void Update()
    {
        float moveSpeed;
        
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            moveSpeed = runSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveSpeed = crouchSpeed;
            Crouch();
        }
        else
        {
            moveSpeed = walkSpeed;
            StandUp();
        }
        
        float forwardSpeed = Input.GetAxis("Vertical") * moveSpeed;
        float strafeSpeed = Input.GetAxis("Horizontal") * moveSpeed;

        Vector3 speed = new Vector3(strafeSpeed, 0, forwardSpeed);
        speed = transform.rotation * speed;

        if (characterController.isGrounded)
        {
            verticalSpeed = -gravity * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && !isCrouching)
            {
                verticalSpeed = jumpForce;
            }
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }

        speed.y = verticalSpeed;

        characterController.Move(speed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        cameraVerticalAngle -= mouseY;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -90f, 90f);
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentStamina -= staminaDecreaseRate * Time.deltaTime;
        }

        isGrounded = characterController.isGrounded;
    }

    void Crouch()
    {
        if (!isCrouching)
        {
            characterController.height = crouchHeight;
            isCrouching = true;
        }
    }

    void StandUp()
    {
        if (isCrouching)
        {
            characterController.height = standHeight;
            isCrouching = false;
        }
    }
}
