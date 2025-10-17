using UnityEngine;
using System.Collections;

public class DartsManager : MonoBehaviour
{
    public Dart dartPrefab;
    public Transform leftHandHolder;
    public Transform[] leftHandSlots; // size 3
    public Transform rightHandSocket;

    int dartsRemaining;
    int sticksThisRack;
    int totalScore;

    void OnEnable()
    {
        GameEvents.OnDartThrown += OnThrown;
        GameEvents.OnDartScored += OnScored;
    }
    void OnDisable()
    {
        GameEvents.OnDartThrown -= OnThrown;
        GameEvents.OnDartScored -= OnScored;
    }

    void Start()
    {
        totalScore = 0;
        SpawnThree();
        GameEvents.OnTotalScoreChanged?.Invoke(totalScore);
        GameEvents.OnDartsRemainingChanged?.Invoke(dartsRemaining);
    }

    void SpawnThree()
    {
        ClearLeftHand();
        dartsRemaining = 3;
        sticksThisRack = 0;

        for (int i = 0; i < 3; i++)
        {
            var dart = Instantiate(dartPrefab, leftHandSlots[i].position, leftHandSlots[i].rotation, leftHandHolder);
            var toggle = dart.GetComponent<GrabbableToggle>();
            toggle.rightHandSocket = rightHandSocket;
        }

        GameEvents.OnDartsRemainingChanged?.Invoke(dartsRemaining);
        GameEvents.OnRackRefilled?.Invoke();
    }

    void OnThrown(Dart _)
    {
        dartsRemaining = Mathf.Max(0, dartsRemaining - 1);
        GameEvents.OnDartsRemainingChanged?.Invoke(dartsRemaining);
        if (dartsRemaining == 0)
            StartCoroutine(WaitAndRefill());
    }

    void OnScored(int score, DartRing ring, int sector, Vector3 _)
    {
        sticksThisRack++;
        totalScore += score;
        GameEvents.OnTotalScoreChanged?.Invoke(totalScore);
    }

    IEnumerator WaitAndRefill()
    {
        // wait until all 3 are stuck (scored)
        while (sticksThisRack < 3) yield return null;
        yield return new WaitForSeconds(3f);

        // destroy all darts not under left hand holder
        foreach (var d in FindObjectsByType<Dart>(FindObjectsSortMode.None))
            if (d.transform.parent != leftHandHolder) Destroy(d.gameObject);

        SpawnThree();
    }

    void ClearLeftHand()
    {
        for (int i = leftHandHolder.childCount - 1; i >= 0; i--)
            Destroy(leftHandHolder.GetChild(i).gameObject);
    }
}
