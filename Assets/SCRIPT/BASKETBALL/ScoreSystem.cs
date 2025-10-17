using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int score;
    int streak;
    float timeLeft;
    public float roundSeconds = 60f;

    void Start() { timeLeft = roundSeconds; }

    void Update() { timeLeft = Mathf.Max(0, timeLeft - Time.deltaTime); }

    public void RegisterGoal(Transform rim, Vector3 releasePos, bool swish)
    {
        float meters = Vector3.Distance(releasePos, rim.position);
        int basePts = meters < 2f ? 1 : meters < 4f ? 2 : meters < 6f ? 3 : 4;

        float comboMult = 1f + 0.1f * streak;         // +10% per streak
        float swishMult = swish ? 1.25f : 1f;
        float timeBonus = Mathf.Lerp(1.2f, 1f, timeLeft / roundSeconds);

        int gained = Mathf.RoundToInt(basePts * comboMult * swishMult * timeBonus);
        score += gained;
        streak++;
        // TODO: VFX/Haptics here
    }

    public void Miss() { streak = 0; }
}