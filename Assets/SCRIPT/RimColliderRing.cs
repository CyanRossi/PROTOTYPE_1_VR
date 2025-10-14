using UnityEngine;

[ExecuteAlways]
public class RimColliderRing : MonoBehaviour
{
    [Range(6, 32)] public int segments = 12;
    public float rimRadius = 0.229f;   // set this to the measured radius from Step 3
    public float tubeRadius = 0.014f;  // thickness of collision tube
    public PhysicsMaterial rimMaterial;
    public string rimLayer = "Rim";
    const string SEG_PREFIX = "RimSeg_";

    void OnEnable() { Rebuild(); }
    void OnValidate() { Rebuild(); }

    public void Rebuild()
    {
        if (segments < 6) segments = 6;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var ch = transform.GetChild(i);
            if (ch.name.StartsWith(SEG_PREFIX))
#if UNITY_EDITOR
                DestroyImmediate(ch.gameObject);
#else
                Destroy(ch.gameObject);
#endif
        }

        float step = Mathf.PI * 2f / segments;
        for (int i = 0; i < segments; i++)
        {
            float theta = (i + 0.5f) * step;
            var seg = new GameObject($"{SEG_PREFIX}{i}");
            seg.transform.SetParent(transform, false);

            Vector3 center = new Vector3(Mathf.Cos(theta) * rimRadius, 0f, Mathf.Sin(theta) * rimRadius);
            seg.transform.localPosition = center;

            Vector3 tangent = new Vector3(-Mathf.Sin(theta), 0f, Mathf.Cos(theta));
            seg.transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);

            var cap = seg.AddComponent<CapsuleCollider>();
            cap.direction = 2; // Z axis
            cap.radius = tubeRadius;
            float chord = 2f * rimRadius * Mathf.Sin(step * 0.5f);
            cap.height = Mathf.Max(0.001f, chord + tubeRadius * 2f);
            cap.material = rimMaterial;

            int layer = LayerMask.NameToLayer(rimLayer);
            if (layer >= 0) seg.layer = layer;
        }
    }
}
