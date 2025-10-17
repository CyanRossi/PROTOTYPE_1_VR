using UnityEngine;

[RequireComponent(typeof(Dart))]
public class DartStickOnHit : MonoBehaviour
{
    public LayerMask dartboardMask;
    public float stickDepth = 0.01f; // 1 cm
    public AudioSource hitSfx;

    bool stuck = false;

    void OnCollisionEnter(Collision c)
    {
        if (stuck) return;

        if (((1 << c.collider.gameObject.layer) & dartboardMask) != 0)
        {
            var zones = c.collider.GetComponentInParent<DartboardCalibrated>();
            if (zones != null)
            {
                ContactPoint cp = c.GetContact(0);
                Vector3 p = cp.point;

                // Get detailed score
                int score = zones.GetScoreAtPoint(p);
                DartRing ring; int sector;

                zones.DebugResolveHit(p, out ring, out sector); // add this helper below
                GameEvents.OnDartScored?.Invoke(score, ring, sector, p);
                GameEvents.OnLastHitText?.Invoke(BuildHitText(score, ring, sector));
            }

            StickToSurface(c);
        }
    }

    string BuildHitText(int score, DartRing ring, int sector)
    {
        if (ring == DartRing.InnerBull) return "INNER BULL (50)";
        if (ring == DartRing.OuterBull) return "OUTER BULL (25)";
        if (ring == DartRing.Miss) return "MISS";
        if (sector == 0) return score.ToString();

        string mult = ring == DartRing.Triple ? "T" : ring == DartRing.Double ? "D" : "S";
        return $"{mult}-{sector}  (+{score})";
    }

    void StickToSurface(Collision c)
    {
        stuck = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        var cp = c.GetContact(0);
        transform.position = cp.point + (-cp.normal * stickDepth);
        transform.rotation = Quaternion.LookRotation(-cp.normal, Vector3.up);

        if (hitSfx) hitSfx.Play();
    }
}
