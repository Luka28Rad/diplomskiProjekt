using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [Header("Target Spawner")]
    public TargetSpawner targetSpawner;
    
    private int numberOfHits;
    private int maxNumberOfTries;

    private void Awake()
    {
        numberOfHits = 0;
        maxNumberOfTries = 10;
        if (ballSpawner == null)
        {
            ballSpawner = FindObjectOfType<BallSpawner>();
        }
        
        if (targetSpawner == null)
        {
            targetSpawner = FindObjectOfType<TargetSpawner>();
        }
        
        Debug.Log("TargetRadiusGizmo initialized");
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

        int score = 0;
        
        if (distance <= outerRadius)
        {
            float ringStep = outerRadius / rings;
            score = rings - Mathf.FloorToInt(distance / ringStep);
            Debug.Log($"Hit target! Score: {score}");
            numberOfHits++;
        }
        else
        {
            Debug.Log($"Missed target! Score: {score}");
        }
        
        if (GameManager.Instance != null && GameManager.Instance.ballType != BallType.Bowling)
        {
            GameManager.Instance.AddScore(score);
        }

        if (hitMarkerPrefab != null)
        {
            GameObject marker;
            if (score > 0) //ako je pogođena meta, poveži marker s targetom
            {
                marker = Instantiate(hitMarkerPrefab, contactPoint, Quaternion.identity, gameObject.transform);

            }
            else //u protivnom ostavi marker gdje je bio -- imati na umu da možda ako udari rub mete,
                 //neće se micati s metom već će ostati u zraku jer je score i dalje 0
            {
                marker = Instantiate(hitMarkerPrefab, contactPoint, Quaternion.identity);
            }
            marker.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
            Destroy(marker, markerLifespan);
        }

        collision.gameObject.transform.position = ballSpawner.transform.position;
        collision.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        collision.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        if (GameManager.Instance.ballType != BallType.Bowling)
        {
            if (targetSpawner != null && numberOfHits <= maxNumberOfTries)
            {
                targetSpawner.RespawnTargetRandomly();
                Debug.Log("Target respawned at new random position");
            }
            else
            {
                numberOfHits = 0;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}