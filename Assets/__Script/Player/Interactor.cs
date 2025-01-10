using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public interface IInteractable
{
    void Interact();
    string GetInteractionText(); // Returns the interaction text for this object
    InteractType GetInteractType(); // New method to return the interaction type

}

public enum InteractType
{
    Click,
    Hold
}



public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public TextMeshProUGUI InteractionText; // Reference to the TMP UI element
    public Image holdProgressBar; // Reference to the UI Slider (Progress Bar)
    public float holdDuration = 2f; // Duration to hold the key for a Hold interaction
    public GameObject InteractorKey;

    private float holdTimer = 0f; // Timer for hold interaction

    private void Start()
    {
        InteractorKey.SetActive(false);
    }


    private void Update()
    {
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
        {

            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                // Display interaction text
                InteractionText.text = interactObj.GetInteractionText();
                InteractionText.gameObject.SetActive(true);
                InteractorKey.SetActive(true);

                // Handle interaction based on type
                if (interactObj.GetInteractType() == InteractType.Click)
                {
                    if (Input.GetKeyDown(KeyCode.E)) // Click interaction
                    {
                        interactObj.Interact();
                    }
                }
                else if (interactObj.GetInteractType() == InteractType.Hold)
                {
                    if (Input.GetKey(KeyCode.E)) // Holding interaction
                    {
                        holdTimer += Time.deltaTime; // Increase the timer while the key is held down

                        // Update the progress bar fill based on the hold time
                        holdProgressBar.fillAmount = holdTimer / holdDuration;

                        // Check if the hold duration is met
                        if (holdTimer >= holdDuration)
                        {
                            interactObj.Interact(); // Trigger interaction
                            holdTimer = 0f; // Reset the timer after interacting
                            holdProgressBar.fillAmount = 0f; // Reset progress bar
                        }
                    }
                    else
                    {
                        // Reset the timer and progress bar if the key is released before the required time
                        holdTimer = 0f;
                        holdProgressBar.fillAmount = 0f;
                    }
                }
            }
            else
            {
                InteractorKey.SetActive(false);
                InteractionText.gameObject.SetActive(false);
                holdProgressBar.fillAmount = 0f; // Reset progress bar when not looking at interactable object
            }
        }
        else
        {
            InteractorKey.SetActive(false);
            InteractionText.gameObject.SetActive(false);
            holdProgressBar.fillAmount = 0f; // Reset progress bar if no object is hit
        }
    }
}
