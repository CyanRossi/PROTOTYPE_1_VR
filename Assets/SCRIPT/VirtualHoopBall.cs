using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VirtualHoopsBall : MonoBehaviour
{
    [HideInInspector] public bool touchedRimThisShot;
    [HideInInspector] public Vector3 lastReleasePosition;
    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    public void OnGrabbed() { touchedRimThisShot = false; }

    public void OnReleased(Vector3 releasePos, Vector3 releaseVelocity)
    {
        lastReleasePosition = releasePos;
        rb.linearVelocity = releaseVelocity;
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.collider.gameObject.layer == LayerMask.NameToLayer("Rim"))
            touchedRimThisShot = true;
    }

    public void ResetShotState() { touchedRimThisShot = false; }
}