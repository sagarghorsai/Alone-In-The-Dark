using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
	private Animator animator;
	public bool isOpen = false;
	public InteractType interactType; // Set in the Inspector for the door's interaction type

	[Header("Sound FX")]
	public string DoorOpenSFX = "DoorOpen";   // SFX for opening the door
	public string DoorCloseSFX = "DoorClose"; // SFX for closing the door

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void Interact()
	{
		isOpen = !isOpen;
		animator.SetBool("Open", isOpen);

		if (isOpen)
		{
			AudioManager.Instance?.PlaySFX(DoorOpenSFX); // Play door open sound
			RoomManager.Instance.SpawnRandomRoom();

			// Trigger camera shake
			CameraShake.Instance?.TriggerShake();
		}
		else
		{
			AudioManager.Instance?.PlaySFX(DoorCloseSFX); // Play door close sound
		}
	}


	public string GetInteractionText()
	{
		return isOpen ? "Close Door" : "Open Door";
	}

	public InteractType GetInteractType()
	{
		return interactType;
	}
}
