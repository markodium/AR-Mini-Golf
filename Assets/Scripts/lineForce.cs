using UnityEngine;

public class lineForce : MonoBehaviour
{
    [SerializeField] private float shotPower;
    [SerializeField] private float stopVelocity = .05f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private AudioSource strokeSound;

    private bool isIdle;
    private bool isAiming;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        isAiming = false;
        lineRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        if (rigidbody.linearVelocity.magnitude < stopVelocity)
        {
            Stop();
        }

        ProcessAim();
    }

    // This keeps your original mouse-click behavior
    private void OnMouseDown()
    {
        if (isIdle)
        {
            isAiming = true;
        }
    }

    private void Update()
    {
        // Phone touch support
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && isIdle)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        isAiming = true;
                    }
                }
            }
        }
    }

    private void ProcessAim()
    {
        if (!isAiming || !isIdle)
            return;

        Vector2 screenPosition;
        bool released = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            screenPosition = touch.position;

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                released = true;
        }
        else
        {
            screenPosition = Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
                released = true;
        }

        Vector3? worldPoint = CastScreenPointRay(screenPosition);

        if (!worldPoint.HasValue)
            return;

        DrawLine(worldPoint.Value);

        if (released)
        {
            Shoot(worldPoint.Value);
        }
    }

    private void Shoot(Vector3 worldPoint)
    {
        isAiming = false;
        lineRenderer.enabled = false;

        Vector3 horizontalWorldPoint = new Vector3(worldPoint.x, transform.position.y, worldPoint.z);

        Vector3 direction = (horizontalWorldPoint - transform.position).normalized;
        float strength = Vector3.Distance(transform.position, horizontalWorldPoint);

        if (strokeSound != null)
        {
            strokeSound.Play();
        }

        rigidbody.AddForce(direction * strength * shotPower);
        isIdle = false;
    }

    private void DrawLine(Vector3 worldPoint)
    {
        Vector3[] positions =
        {
            transform.position,
            worldPoint
        };

        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = true;
    }

    private void Stop()
    {
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.linearVelocity = Vector3.zero;
        isIdle = true;
    }

    private Vector3? CastScreenPointRay(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
        {
            return hit.point;
        }

        return null;
    }
}