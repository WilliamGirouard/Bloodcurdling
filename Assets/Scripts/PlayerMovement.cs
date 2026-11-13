using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -19.62f;

    [Header("Crouch Settings")]
    public float standingHeight = 1.8f;
    public float crouchingHeight = 0.9f;
    public float crouchTransitionSpeed = 10f;
    private bool isCrouching = false;

    [Header("References")]
    public Transform playerCamera;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standingHeight;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        HandleMovement();
        HandleCrouch();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Read WASD directly from the new Input System
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.wKey.isPressed) moveZ += 1f;

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;

        float currentSpeed = walkSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (Keyboard.current.leftShiftKey.isPressed)
        {
            currentSpeed = sprintSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump with Spacebar
        if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

        if (playerCamera != null)
        {
            float targetCamY = isCrouching ? (crouchingHeight * 0.75f) : (standingHeight * 0.85f);
            Vector3 camPos = playerCamera.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCamY, crouchTransitionSpeed * Time.deltaTime);
            playerCamera.localPosition = camPos;
        }
    }

    private void HandleInteraction()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (playerCamera != null)
            {
                Ray ray = new Ray(playerCamera.position, playerCamera.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
                {
                    Debug.Log("Interacted with: " + hit.collider.name);
                }
            }
        }
    }
}