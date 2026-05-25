using UnityEngine;

public abstract class EventTrigger : MonoBehaviour, IInteractable
{
    public GameObject GameObject => gameObject;

    public abstract void Interact();
}

//EventTrigger Implementation Example:
//public class Level3SecretSwitchTrigger : EventTrigger
//{
//    public static Action OnEventRaised;

//    public override void Interact()
//    {
//        OnEventRaised?.Invoke();
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//            Interact();
//    }
//}
