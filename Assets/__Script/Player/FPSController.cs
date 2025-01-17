using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);

    [Header("Zoom Setting")]
    [SerializeField] private bool canZoom = true;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse2;
    [SerializeField] private float timerToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;

    private float defaultFOV;
    private Coroutine zoomRoutine;

    [Header("Movement Setting")]
    [SerializeField] private float walkSpd = 4.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Sprint Setting")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftControl;
    [SerializeField] private float sprintSpd = 8.0f;

    [Header("Footstep Sounds")]
    [SerializeField] private string woodFootstepSoundName = "WoodStep";
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;

    private float footstepTimer;

    [Header("HeadBob Setting")]
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;

    public bool isHiding;

    private float defaultYPos = 0f;
    private float timer;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpdX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpdY = 2.0f;
    [SerializeField, Range(1, 100)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80.0f;

    private Camera playerCam;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    public GameObject arms;
    public GameObject inventory;
    public PlayerInventory playerInventory;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCam = GetComponentInChildren<Camera>();
        defaultYPos = playerCam.transform.localPosition.y;
        defaultFOV = playerCam.fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            HandleFootsteps();

            if (canUseHeadbob)
            {
                HandleHeadBob();
            }
            if (canZoom)
            {
                HandleZoom();
            }

            ApplyFinalMovement();
        }
    }

    void HandleMovementInput()
    {
        currentInput = new Vector2(
            (IsSprinting ? sprintSpd : walkSpd) * Input.GetAxis("Vertical"),
            (IsSprinting ? sprintSpd : walkSpd) * Input.GetAxis("Horizontal")
        );

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
                        (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpdY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpdX, 0);
    }

    void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCam.transform.localPosition.z
            );
        }
    }

    void HandleFootsteps()
    {
        if (!characterController.isGrounded || characterController.velocity.magnitude <= 0.1f)
        {
            footstepTimer = 0f; // Reset timer when not moving
            return;
        }

        float stepInterval = IsSprinting ? sprintStepInterval : walkStepInterval;
        footstepTimer += Time.deltaTime;

        if (footstepTimer >= stepInterval)
        {
            PlayFootstep();
            footstepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (AudioManager.Instance != null)
        {
            float playbackSpeed = IsSprinting ? sprintSpeedMultiplier : 1f;
            AudioManager.Instance.PlaySFX(woodFootstepSoundName, playbackSpeed);
        }
        else
        {
            Debug.LogWarning("AudioManager instance is missing!");
        }
    }

    void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    void ApplyFinalMovement()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCam.fieldOfView;

        float timeElapsed = 0;

        while (timeElapsed < timerToZoom)
        {
            playerCam.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timerToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCam.fieldOfView = targetFOV;
        zoomRoutine = null;
    }

    public void EnablePlayer()
    {
        CanMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisablePlayer()
    {
        CanMove = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HandleArmsAndInventoryVisibility(bool isHiding)
    {
        if (isHiding || !playerInventory.HasItem(ItemType.Flashlight))
        {
            // Hide arms and inventory if hiding or the player lacks a flashlight
            if (arms.activeSelf || inventory.activeSelf)
            {
                arms.SetActive(false);
                inventory.SetActive(false);
            }
        }
        else
        {
            // Show arms and inventory if not hiding and the player has a flashlight
            if (!arms.activeSelf || !inventory.activeSelf)
            {
                arms.SetActive(true);
                inventory.SetActive(true);
            }
        }
    }



}
