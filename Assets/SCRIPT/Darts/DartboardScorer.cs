using UnityEngine;
using System;

public class DartboardScorer : MonoBehaviour
{
    [Header("Board Dimensions (meters)")]
    public float boardRadius = 0.225f;       // total radius
    public float outerBullR = 0.0318f;
    public float innerBullR = 0.0127f;
    public float tripleRingInnerR = 0.107f;
    public float tripleRingOuterR = 0.115f;
    public float doubleRingInnerR = 0.170f;
    public float doubleRingOuterR = 0.180f;

    [Header("Refs")]
    public Transform boardCenter; // Empty at exact bull center on the board plane
    public ScoreManager scoreManager;

    // Standard dartboard order clockwise, 0° at "20" sector (top)
    private int[] sectorOrder = new int[] { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };

    public void RegisterHit(Vector3 worldHitPoint)
    {
        // Convert to local 2D coords on board plane (x,y on board face)
        Vector3 local = boardCenter.InverseTransformPoint(worldHitPoint);
        // Assume board faces -Z or +Z; we only care about x,y radius on plane
        Vector2 p = new Vector2(local.x, local.y);
        float r = p.magnitude;

        // Outside?
        if (r > boardRadius) { scoreManager.AddScore(0); return; }

        int score = 0;
        // Bulls
        if (r <= innerBullR) score = 50;
        else if (r <= outerBullR) score = 25;
        else
        {
            // Which ring?
            bool isTriple = (r >= tripleRingInnerR && r <= tripleRingOuterR);
            bool isDouble = (r >= doubleRingInnerR && r <= doubleRingOuterR);

            // Angle: 0° straight up (world +Y), clockwise positive
            float angleRad = Mathf.Atan2(p.x, p.y); // note swapped to set 0 at +Y
            float angleDeg = angleRad * Mathf.Rad2Deg;
            if (angleDeg < 0) angleDeg += 360f;

            // 20 sectors → 18° per sector; 0°=center of 20
            int sectorIndex = Mathf.RoundToInt(angleDeg / 18f) % 20;
            int baseVal = sectorOrder[sectorIndex];

            score = baseVal;
            if (isTriple) score *= 3;
            else if (isDouble) score *= 2;
        }

        scoreManager.AddScore(score);
    }
}
