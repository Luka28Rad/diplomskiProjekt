using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;

    [Header("Tuning")]
    private float distanceStep = 2.5f;
    private float heightStep = 1f;
    
    [Header("Random Range")]
    private int minDistance = 5;
    private int maxDistance = 10;
    private int minHeight = 1;
    private int maxHeight = 3;

    private bool isFirstSpawn = true;
    private GameObject currentTarget;

    [SerializeField]
    private GameObject bowlingManager;


    void Start()
    {
        if (GameManager.Instance.ballType == BallType.Bowling)
        {
            enableBowling();
        }
        else
        {
            SpawnTarget();
        }

    }

    private void enableBowling()
    {
        bowlingManager.SetActive(true);
    }
    private void disableBowling()
    {
        bowlingManager.SetActive(false);
    }
    
    

    void SpawnTarget() //sada vise ne unistavamo metu kad se pogodi nego samo promjeni polozaj
                       //to nam omogucuje da kad se pogodi i stavi marker, on se promjenio zajedno s metom
    {
        int d, h;

        if (isFirstSpawn)
        {
            d = minDistance;
            h = minHeight;
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
            d + 0,
            h + 0,
            transform.position.z
        );

        if (currentTarget == null)
        {
            // First time: create it
            currentTarget = Instantiate(
                targetPrefab,
                position,
                targetPrefab.transform.rotation
            );
        }
        else
        {
            // Target exists: just move it
            currentTarget.transform.position = position;
        }

        Debug.Log($"Target positioned at distance: {d}, height: {h}");
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
