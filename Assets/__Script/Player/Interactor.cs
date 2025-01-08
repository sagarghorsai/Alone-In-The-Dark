using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public interface IInteractable
{
    void Interact();
    string GetInteractionText(); // Returns the interaction text for this object
}




public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public TextMeshProUGUI InteractionText; // Reference to the TMP UI element

    private void Update()
    {
        Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                // Display the interaction text from the interactable object
                InteractionText.text = interactObj.GetInteractionText();
                InteractionText.gameObject.SetActive(true);

                // Check if the player interacts
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactObj.Interact();
                }
            }
            else
            {
                // Hide the text if not looking at an interactable object
                InteractionText.gameObject.SetActive(false);
            }
        }
        else
        {
            // Hide the text if no object is hit
            InteractionText.gameObject.SetActive(false);
        }
    }
}
