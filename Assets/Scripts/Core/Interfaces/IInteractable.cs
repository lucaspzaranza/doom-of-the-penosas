using UnityEngine;

public interface IInteractable
{
    GameObject GameObject { get; }

    public void Interact();
}
