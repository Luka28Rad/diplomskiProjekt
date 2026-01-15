using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;

    [Header("Tuning")]
    public float distanceStep = 2.5f;
    public float heightStep = 1.5f;
    
    [Header("Random Range")]
    public int minDistance = 1;
    public int maxDistance = 3;
    public int minHeight = 1;
    public int maxHeight = 3;

    private bool isFirstSpawn = true;
    private GameObject currentTarget;

    void Start()
    {
        SpawnTarget();
    }

    void SpawnTarget()
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }

        int d, h;

        if (isFirstSpawn)
        {
            d = 1;
            h = 1;
            isFirstSpawn = false;
        }
        else
        {
            d = Random.Range(minDistance, maxDistance + 1);
            h = Random.Range(minHeight, maxHeight + 1);
            
            GameManager.Instance.SetTargetDistance(d);
            GameManager.Instance.SetTargetHeight(h);
        }

        Vector3 position = new Vector3(
            d * distanceStep + transform.position.x,
            h * heightStep + transform.position.y,
            0 + transform.position.z
        );

        currentTarget = Instantiate(targetPrefab, position, Quaternion.identity * targetPrefab.transform.rotation);
        
        Debug.Log($"Target spawned at distance: {d}, height: {h}");
    }
    
    public void RespawnTargetRandomly()
    {
        // Save current session data to CSV before respawning
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveOnTargetReset();
        }
        
        SpawnTarget();
    }
    
    public void ResetToFirstSpawn()
    {
        // Save current session data to CSV before reset
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveOnTargetReset();
        }
        
        isFirstSpawn = true;
        SpawnTarget();
    }
}
