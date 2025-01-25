using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private Animator animator;
    public bool isOpen = false;
    public InteractType interactType; // Set this in the Inspector for each object

    public string DoorOpenSFX = "DoorOpen"; // Name of the SFX for opening the door
    public string DoorCloseSFX = "DoorClose"; // Name of the SFX for closing the door

    string txt = "";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        isOpen = !isOpen; // Toggle the state of the door
        animator.SetBool("Open", isOpen);

        // Play the appropriate SFX
        if (isOpen)
        {
            AudioManager.Instance.PlaySFX(DoorOpenSFX);
        }
        else
        {
            AudioManager.Instance.PlaySFX(DoorCloseSFX);
        }
    }

    public string GetInteractionText()
    {
        txt = isOpen ? "Close" : "Open";
        return interactType == InteractType.Hold ? $"{txt} Door" : $"{txt} Door";
    }

    public InteractType GetInteractType()
    {
        return interactType; // Return the interact type for this object
    }
}
