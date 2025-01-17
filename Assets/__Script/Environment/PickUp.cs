using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum ItemType
{
    Flashlight,
    Key
}

public class PickUp : MonoBehaviour, IInteractable
{
    public InteractType interactType;
    public ItemType itemType;
    private string itemName;

    public GameObject playerFlashlight; // Reference to flashlight GameObject

    private PlayerInventory playerInventory;

    private void Start()
    {
        itemName = this.gameObject.name;

        // Assuming the player inventory is attached to the same GameObject as this script
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
    }

    public string GetInteractionText()
    {
        return $"Pick Up {itemName}";
    }

    public InteractType GetInteractType()
    {
        return interactType;
    }

    public void Interact()
    {
        switch (itemType)
        {
            case ItemType.Flashlight:
                AddFlashLightToInventory();
                break;

            case ItemType.Key:
                AddKeyToInventory();
                break;

            default:
                Debug.LogWarning("Unknown item type!");
                break;
        }

        this.gameObject.SetActive(false); // Deactivate the item
    }

    private void AddFlashLightToInventory()
    {
        playerFlashlight.gameObject.SetActive(true);
        playerInventory.AddItem(ItemType.Flashlight); // Add the flashlight to inventory
        Debug.Log("Added flashlight to the inventory");
      
    }


    private void AddKeyToInventory()
    {
        playerInventory.AddItem(ItemType.Key); // Add key to inventory
    }
}
