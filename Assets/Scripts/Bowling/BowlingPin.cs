using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    public bool isFallen = false;
    public float fallThreshold = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (!isFallen && Vector3.Angle(transform.up, Vector3.up) > fallThreshold)
        {
            isFallen = true;
        }
    }

    public void ResetPin()
    {
        isFallen = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = startRotation;
        gameObject.SetActive(true);
    }
}