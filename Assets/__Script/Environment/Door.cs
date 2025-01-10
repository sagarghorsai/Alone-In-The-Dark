using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private Animator animator;
    public bool isOpen = false;
    public InteractType interactType; // Set this in the Inspector for each object

    string txt = "";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        isOpen = !isOpen; // Toggle the state of the door
        animator.SetBool("Open", isOpen);
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
