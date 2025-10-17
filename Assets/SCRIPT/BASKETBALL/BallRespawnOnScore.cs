using UnityEngine;
using Oculus.Interaction; // for Grabbable (Meta ISDK)

public class BallRespawnOnScore : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSocket;   // drag your BallSocket transform here
    [SerializeField] private float spawnDelay = 0.35f;

    private GameObject _currentBall;

    private void Start()
    {
        SpawnNewBall();
        // Optional: Subscribe to your hoop trigger event here instead of calling from the trigger.
        // HoopTrigger.OnScored += HandleScored;
    }

    // Call this from your Hoop trigger when you add score.
    public void HandleScored()
    {
        if (_currentBall != null) Destroy(_currentBall, 0.1f);
        Invoke(nameof(SpawnNewBall), spawnDelay);
    }

    private void SpawnNewBall()
    {
        _currentBall = Instantiate(ballPrefab, ballSocket.position, ballSocket.rotation);

        // Park it in the socket (doesn't fall)
        var rb = _currentBall.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        _currentBall.transform.SetParent(ballSocket, worldPositionStays: true);

        // Add helper that unfreezes on first grab
        var grabbable = _currentBall.GetComponentInChildren<Grabbable>();
        var onGrab = _currentBall.AddComponent<UnparkOnFirstGrab>();
        onGrab.Init(rb, grabbable);
    }
}
