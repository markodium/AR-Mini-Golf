using UnityEngine;

public class HoleTrigger : MonoBehaviour
{

    [SerializeField] private AudioSource winSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            other.transform.position = transform.position;

            ARPlaceCourse manager = FindFirstObjectByType<ARPlaceCourse>();

            if (winSound != null)
            {
                winSound.Play();
            }

            if (manager != null)
            {
                manager.ShowWinPanel();
            }
        }
    }
}
