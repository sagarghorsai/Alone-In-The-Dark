using UnityEngine;

public class Hide : MonoBehaviour, IInteractable
{
    public InteractType interactType;
    public Camera mainCamera; // Reference to the main camera
    public Transform hidingPosition; // Position/rotation for hiding
    public FPSController playerController; // Reference to the FPSController script
    public float transitionSpeed = 2f; // Speed of the transition
    public KeyCode exitKey = KeyCode.E; // Key to exit hiding

    private bool isHiding = false;
    private bool transitioning = false;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    public float rotationSpeed = 5f;

    public GameObject interactor;
    private Vector2 currentRotation; // Stores the current rotation of the camera

    public string GetInteractionText()
    {
        return isHiding ? "Exit" : "Hide";
    }

    public InteractType GetInteractType()
    {
        return interactType;
    }

    public void Interact()
    {
        if (!transitioning)
        {
            if (!isHiding)
            {
                // Save the original camera position, rotation, and current rotation
                originalCameraPosition = mainCamera.transform.position;
                originalCameraRotation = mainCamera.transform.rotation;
                currentRotation = mainCamera.transform.localEulerAngles;
            }

            isHiding = !isHiding; // Toggle hiding state
            StartCoroutine(SmoothTransition(isHiding));
        }
    }

    private void Update()
    {
        playerController.isHiding = this.isHiding;

        if (isHiding && Input.GetKeyDown(exitKey) && !transitioning)
        {
            isHiding = false; // Exit hiding
            StartCoroutine(SmoothTransition(false));
        }

        // Allow the player to look around while hiding
        if (isHiding && !transitioning)
        {
            LookAround();
        }
    }

    private void LookAround()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Update current rotation
        currentRotation.x -= mouseY;
        currentRotation.y += mouseX;

        // Clamp the vertical rotation to prevent flipping
        currentRotation.x = Mathf.Clamp(currentRotation.x, -90f, 90f);

        // Apply the rotation to the camera
        mainCamera.transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0f);
    }


    private System.Collections.IEnumerator SmoothTransition(bool hiding)
    {
        transitioning = true;

        // Disable player movement during transition
        if (hiding)
        {
            playerController.DisablePlayer();
        }

        Vector3 targetPosition = hiding ? hidingPosition.position : originalCameraPosition;
        Quaternion targetRotation = hiding ? hidingPosition.rotation : originalCameraRotation;

        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * transitionSpeed;

            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed);

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;

        // Re-enable player movement after transition
        if (!hiding)
        {
            playerController.EnablePlayer();
        }

        transitioning = false;
    }

    public bool IsHiding()
    {
        return isHiding;
    }
}
