using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private Animator animator;
    public bool isOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        isOpen = !isOpen; // Toggle the isOpen state
        animator.SetBool("Open", isOpen); // Update the animator parameter
    }

    public string GetInteractionText()
    {
        return "Press E to Open/Close Door";
    }
}
