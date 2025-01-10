using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PickUp : MonoBehaviour, IInteractable
{
    public InteractType interactType;
    string txt = "";
    string itemName;

    private void Start()
    {
        itemName = this.gameObject.name;
    }


    public string GetInteractionText()
    {
        txt = itemName;
        return interactType == InteractType.Hold ? $"Pick Up{itemName}" : $"Pick Up{itemName}";
    }

    public InteractType GetInteractType()
    {
        return interactType; // Return the interact type for this object
    }

    public void Interact()
    {
        this.gameObject.SetActive(false);
    }
}
