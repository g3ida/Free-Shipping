using UnityEngine;
using UnityEditor;

public interface IInteractableObject
{
    void Interact(PlayerManager player, RaycastHit2D hit);
}