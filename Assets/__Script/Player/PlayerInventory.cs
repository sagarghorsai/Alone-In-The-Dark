using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemType> inventory = new List<ItemType>(); // List to store picked-up items.

    // Add item to inventory
    public void AddItem(ItemType item)
    {
        if (!inventory.Contains(item)) // Avoid duplicates
        {
            inventory.Add(item);
            Debug.Log($"{item} added to inventory.");
        }
        else
        {
            Debug.Log($"{item} is already in the inventory.");
        }
    }

    // Check if an item is in the inventory
    public bool HasItem(ItemType item)
    {
        return inventory.Contains(item);
    }
}
