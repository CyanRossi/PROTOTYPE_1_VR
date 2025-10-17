using UnityEngine;
public class HoopGoal : MonoBehaviour
{
    public Transform rimCenter;
    public float swishRadius = 0.18f;
    public ScoreSystem scoreSystem;

    void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb == null) return;
        if (rb.linearVelocity.y >= 0f) return; // must be moving downward

        var ball = other.GetComponent<VirtualHoopsBall>();
        if (ball == null) return;

        float dist = Vector3.Distance(rimCenter.position, rb.worldCenterOfMass);
        bool swish = !ball.touchedRimThisShot && dist < swishRadius;

        scoreSystem?.RegisterGoal(rimCenter, ball.lastReleasePosition, swish);
        ball.ResetShotState();
    }
}