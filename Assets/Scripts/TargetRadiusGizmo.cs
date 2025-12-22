using UnityEngine;

[ExecuteAlways]
public class TargetRadiusGizmo : MonoBehaviour
{
    [Header("Rings")]
    public float outerRadius = 1f;
    public int rings = 10;
    public int segments = 64;

    [Header("Visuals")]
    public Color ringColor = Color.green;

    [Header("Offset")]
    public float offsetX = -0.07f; // move in -X direction

    [Header("Hit Visuals")]
    public float hitGizmoSize = 0.05f;
    public Color hitColor = Color.red;

    [Header("Ball Spawner")]
    public BallSpawner ballSpawner; // assign your BallSpawner here

    // Store hits
    private struct HitInfo
    {
        public Vector3 position;
        public int score;
    }
    private readonly System.Collections.Generic.List<HitInfo> hits = new();
    
    private void Awake()
    {
        // Automatically find BallSpawner in the scene if not assigned
        if (ballSpawner == null)
        {
            ballSpawner = FindObjectOfType<BallSpawner>();
            if (ballSpawner == null)
                Debug.LogWarning("No BallSpawner found in the scene!");
        }
    }

    private void OnDrawGizmos()
    {
        // Draw target rings
        Vector3 center = transform.position + Vector3.right * offsetX;
        Vector3 axisY = Vector3.up;
        Vector3 axisZ = Vector3.forward;

        Gizmos.color = ringColor;
        for (int ring = 0; ring < rings; ring++)
        {
            float t = (float)(rings - ring) / rings;
            float radius = outerRadius * t;
            DrawCircle(center, axisY, axisZ, radius);
        }

        // Draw hit markers
        Gizmos.color = hitColor;
        foreach (var hit in hits)
        {
            Gizmos.DrawSphere(hit.position, hitGizmoSize);
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
        Vector3 hitPos = collision.transform.position;

        // Distance in YZ plane
        Vector2 hit2D = new Vector2(hitPos.y - center.y, hitPos.z - center.z);
        float distance = hit2D.magnitude;

        if (distance > outerRadius)
        {
            Debug.Log("Missed target!");
        }
        else
        {
            // Calculate score based on ring
            float ringStep = outerRadius / rings;
            int score = rings - Mathf.FloorToInt(distance / ringStep);
            Debug.Log($"Hit target! Score: {score}");

            // Store hit for gizmo drawing
            hits.Add(new HitInfo { position = hitPos, score = score });
        }

        // Destroy the collided object
        Destroy(collision.gameObject);

        // Spawn a new ball using the assigned BallSpawner
        if (ballSpawner != null)
        {
            ballSpawner.RespawnBall();
        }
        else
        {
            Debug.LogWarning("BallSpawner reference not assigned in TargetRadiusGizmo!");
        }
    }
}
