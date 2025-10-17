using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dart : MonoBehaviour
{
    [Header("Throw Tuning")]
    public float baseThrowPower = 3.5f;   // scales hand velocity
    [Range(12f, 40f)] public float weightGrams = 22f;

    Rigidbody rb;
    TrailRenderer trail;
    bool inHand = false;
    Transform followTarget;
    Vector3 lastPos;
    Vector3 velocitySmoothed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        ApplyWeight();
        SetKinematic(true);
    }

    public void ApplyWeight()
    {
        rb.mass = weightGrams / 1000f;
        rb.linearDamping = Mathf.Lerp(0.02f, 0.08f, (weightGrams - 12f) / 28f);
        rb.angularDamping = 0.05f;
    }

    void FixedUpdate()
    {
        if (!inHand || followTarget == null) return;

        // hard follow to socket for stable aim
        rb.MovePosition(followTarget.position);
        rb.MoveRotation(followTarget.rotation);

        // sample smoothed hand velocity
        Vector3 v = (transform.position - lastPos) / Time.fixedDeltaTime;
        velocitySmoothed = Vector3.Lerp(velocitySmoothed, v, 0.35f);
        lastPos = transform.position;
    }

    public void PickUp(Transform socket)
    {
        followTarget = socket;
        inHand = true;
        SetKinematic(true);
        if (trail) { trail.Clear(); trail.emitting = false; }
        lastPos = transform.position;
        velocitySmoothed = Vector3.zero;
    }

    public void Release(float powerMultiplier = 1f)
    {
        inHand = false;
        SetKinematic(false);

        Vector3 throwVel = velocitySmoothed * baseThrowPower * Mathf.Clamp(powerMultiplier, 0.3f, 2.5f);
        rb.linearVelocity = throwVel;

        if (throwVel.sqrMagnitude > 0.01f)
            rb.MoveRotation(Quaternion.LookRotation(throwVel.normalized, Vector3.up));

        if (trail) trail.emitting = true;
        Invoke(nameof(StopTrail), 2.0f);
    }

    void StopTrail() { if (trail) trail.emitting = false; }

    void SetKinematic(bool on)
    {
        rb.isKinematic = on;
        rb.useGravity = !on;
    }
}
