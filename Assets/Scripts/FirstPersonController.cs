using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey) && !IsSliding;
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && (characterController.isGrounded || airJumps > 0) && !IsSliding;

    [Header("Functional Options")]
    [SerializeField] public bool canSprint = true;
    [SerializeField] public bool canJump = true;
    [SerializeField] public bool canExtraJump = true;
    [SerializeField] public bool willSlideOnSlopes = true;
    [SerializeField] public bool canMoveInAir = true;

    [Header("Functional Options")]
    [SerializeField] public KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] public KeyCode jumpKey = KeyCode.Space;

    [Header("Movement Parameters")]
    [SerializeField] public float walkSpeed = 3.0f;
    [SerializeField] public float sprintSpeed = 6.0f;
    [SerializeField] public float slopeSpeed = 4.5f;

    [Header("Jumping Parameters")]
    [SerializeField] public float jumpForce = 8.0f;
    [SerializeField] public int airJumpsMax = 1;
    private int airJumps;
    [SerializeField] public float airMultiplier = 1.0f;
    [SerializeField] public float gravity = 25.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] public float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] public float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] public float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] public float lowerLookLimit = 80.0f;

    // Sliding Parameters
    private Vector3 hitPointNormal;

    private bool IsSliding 
    {
        get
        {
            if(characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }

    }

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponentInChildren<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLock();

            if(canJump)
            {
                HandleJump();
            }

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        float moveDirectionY = moveDirection.y;

        if (characterController.isGrounded)
        {
            currentInput = new Vector2((IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
            moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        }
        else if(canMoveInAir) //in air
        {
            //currentInput = new Vector2((IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                currentInput = new Vector2((IsSprinting ? sprintSpeed : walkSpeed) * airMultiplier * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * airMultiplier * Input.GetAxis("Horizontal"));
                moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
            }
        }

        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLock()
    {
        SetAngle(
            rotationX - Input.GetAxis("Mouse Y") * lookSpeedY,
            transform.localEulerAngles.y + Input.GetAxis("Mouse X") * lookSpeedX
        );
    }

    public void SetAngle(float x, float y)
    {
        rotationX = Mathf.Clamp(x, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation = Quaternion.Euler(0, y, 0);
    }

    private void HandleJump()
    {
        if (ShouldJump && airJumps > 0 && canExtraJump)
        {
            airJumps--;
            moveDirection.y = jumpForce;
            
        }
        else if(ShouldJump)
        {
            airJumps = airJumpsMax;
            moveDirection.y = jumpForce;
        }
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if(willSlideOnSlopes && IsSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }
}
