using UnityEngine;
using Oculus.Interaction; // For Grabbable

public class UnparkOnFirstGrab : MonoBehaviour
{
    private Rigidbody rb;
    private Grabbable grabbable;
    private bool hasBeenGrabbed = false;

    public void Init(Rigidbody rigidbody, Grabbable grab)
    {
        rb = rigidbody;
        grabbable = grab;

        // Subscribe to grab events
        grabbable.WhenPointerEventRaised += OnGrabEvent;
    }

    private void OnGrabEvent(PointerEvent evt)
    {
        if (hasBeenGrabbed) return;

        if (evt.Type == PointerEventType.Select) // When grabbed
        {
            hasBeenGrabbed = true;
            rb.isKinematic = false;   // Enable physics so ball can be thrown
            rb.transform.SetParent(null); // Detach from socket
        }
    }

    private void OnDestroy()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised -= OnGrabEvent;
    }
}
