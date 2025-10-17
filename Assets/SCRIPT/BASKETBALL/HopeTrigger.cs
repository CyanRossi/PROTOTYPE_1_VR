using UnityEngine;

public class HopeTrigger : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ScoreManager.Instance.AddScore(1);
            Destroy(other.gameObject);

            GameObject newBall = Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);
            if (newBall.GetComponent<OVRGrabbable>() == null)
            {
                newBall.AddComponent<OVRGrabbable>();
            }
        }
    }
}
