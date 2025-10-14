using UnityEngine;

public class ThrowTuner : MonoBehaviour
{
    [Header("Tuning")]
    public float releaseSpeedScale = 1.05f;   // 1.0–1.2
    public float upwardBias = 0.15f;          // adds arc
    public float maxSpeed = 12f;              // clamp m/s
    public int samples = 6;                   // sample window

    readonly Vector3[] v = new Vector3[16];
    int head;
    Rigidbody rb;
    bool held;

    void Awake() => rb = GetComponent<Rigidbody>();

    // Call these from your grab component events:
    public void OnGrabbed() { held = true; head = 0; }
    public void OnReleased(Transform handOrController)
    {
        held = false;
        var vel = AverageVel() * releaseSpeedScale;
        vel += Vector3.up * upwardBias * vel.magnitude;
        if (vel.magnitude > maxSpeed) vel = vel.normalized * maxSpeed;
        rb.linearVelocity = vel;

        // spin helps backboard bank shots feel right
        var spin = Vector3.Cross(handOrController.forward, Vector3.up) * 10f;
        rb.angularVelocity = spin;
    }

    public void FeedVelocitySample(Vector3 currentVelocity)
    {
        if (!held) return;
        v[head % samples] = currentVelocity;
        head++;
    }

    Vector3 AverageVel()
    {
        int n = Mathf.Min(samples, head);
        if (n == 0) return Vector3.zero;
        Vector3 s = Vector3.zero;
        for (int i = 0; i < n; i++) s += v[i];
        return s / n;
    }
}
