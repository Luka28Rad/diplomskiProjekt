using UnityEngine;

public class TargetRadiusGizmo : MonoBehaviour
{
    [Header("Rings")]
    public float outerRadius = 1f;
    public int rings = 10;
    public int segments = 64;

    [Header("Visuals")]
    public Color ringColor = Color.green;

    [Header("In-Game Hit Visuals")]
    public GameObject hitMarkerPrefab;
    public float hitMarkerSize = 0.05f;
    public float markerLifespan = 5f;

    [Header("Offset")]
    public float offsetX = -0.07f;

    [Header("Ball Spawner")]
    public BallSpawner ballSpawner;

    private void Awake()
    {
        if (ballSpawner == null)
        {
            ballSpawner = FindObjectOfType<BallSpawner>();
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 center = transform.position + Vector3.right * offsetX;
        Gizmos.color = ringColor;
        for (int ring = 0; ring < rings; ring++)
        {
            float t = (float)(rings - ring) / rings;
            float radius = outerRadius * t;
            DrawCircle(center, Vector3.up, Vector3.forward, radius);
        }
    }

    private void DrawCircle(Vector3 center, Vector3 axisY, Vector3 axisZ, float radius)
    {
        float angleStep = 2f * Mathf.PI / segments;
        Vector3 prevPoint = center + axisY * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 nextPoint = center + (Mathf.Cos(angle) * axisY + Mathf.Sin(angle) * axisZ) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("ThrowablePoints")) return;

        Vector3 center = transform.position + Vector3.right * offsetX;

        Vector3 contactPoint = collision.GetContact(0).point;

        Vector2 hit2D = new Vector2(contactPoint.y - center.y, contactPoint.z - center.z);
        float distance = hit2D.magnitude;

        if (distance <= outerRadius)
        {
            float ringStep = outerRadius / rings;
            int score = rings - Mathf.FloorToInt(distance / ringStep);
            Debug.Log($"Hit target! Score: {score}");
        }

        if (hitMarkerPrefab != null)
        {
            GameObject marker = Instantiate(hitMarkerPrefab, contactPoint, Quaternion.identity);
            marker.transform.localScale = Vector3.one * hitMarkerSize;

            Destroy(marker, markerLifespan);
        }

        collision.gameObject.transform.position = ballSpawner.transform.position;
        collision.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        collision.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}