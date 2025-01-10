using UnityEngine;

public class Hide : MonoBehaviour, IInteractable
{
    public InteractType interactType;
    public Camera mainCamera; // Reference to the main camera
    public Transform hidingPosition; // Position/rotation for hiding
    public FPSController playerController; // Reference to the FPSController script
    public float transitionSpeed = 2f; // Speed of the transition

    private bool isHiding = false;
    private bool transitioning = false;
    private string interactionText = "Hide";

    public string GetInteractionText()
    {
        return interactionText;
    }

    public InteractType GetInteractType()
    {
        return interactType;
    }

    public void Interact()
    {
        if (!transitioning)
        {
            isHiding = !isHiding; // Toggle hiding state
            StartCoroutine(SmoothTransition(isHiding));
        }
    }

    private System.Collections.IEnumerator SmoothTransition(bool hiding)
    {
        transitioning = true;

        // Disable player during the transition
        if (hiding)
        {
            playerController.DisablePlayer();
        }

        Vector3 targetPosition = hiding ? hidingPosition.position : mainCamera.transform.position;
        Quaternion targetRotation = hiding ? hidingPosition.rotation : mainCamera.transform.rotation;

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

        // Re-enable player after transitioning
        if (!hiding)
        {
            playerController.EnablePlayer();
        }

        transitioning = false;
    }
}
