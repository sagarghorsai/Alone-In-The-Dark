using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight; // Reference to the Light component
    public PlayerInventory inventory; // Reference to the player's inventory
    private bool isOn = false; // Tracks if the flashlight is currently on

    private void Update()
    {
        // Check if the player has a flashlight in their inventory
        if (inventory.HasItem(ItemType.Flashlight))
        {
            // Toggle flashlight state with right mouse button
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isOn = !isOn;
                flashlight.gameObject.SetActive(isOn);
            }
        }
    }
}
