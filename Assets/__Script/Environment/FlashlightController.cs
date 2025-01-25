using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight; // Reference to the Light component
    public PlayerInventory inventory; // Reference to the player's inventory
    public string flashlightOnSFX = "FlashlightOn"; // Name of the SFX for turning on
    public string flashlightOffSFX = "FlashlightOff"; // Name of the SFX for turning off

    private bool isOn = false; // Tracks if the flashlight is currently on

    private void Update()
    {
        // Check if the player has a flashlight in their inventory
        if (inventory != null && inventory.HasItem(ItemType.Flashlight))
        {
            // Toggle flashlight state with right mouse button
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isOn = !isOn;
                flashlight.gameObject.SetActive(isOn);

                // Play the appropriate sound effect
                if (AudioManager.Instance != null)
                {
                    if (isOn)
                    {
                        AudioManager.Instance.PlaySFX(flashlightOnSFX);
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX(flashlightOffSFX);
                    }
                }
            }
        }
    }
}
