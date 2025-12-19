using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;

    [Header("Tuning")]
    public float distanceStep = 2.5f;
    public float heightStep = 1.5f;

    void Start()
    {
        SpawnTarget();
    }

    void SpawnTarget()
    {
        int d = Mathf.Clamp(GameManager.Instance.targetDistance, 1, 3);
        int h = Mathf.Clamp(GameManager.Instance.targetHeight, 1, 3);

        Vector3 position = new Vector3(
            d * distanceStep + transform.position.x,
            h * heightStep + transform.position.y,
            0 + transform.position.z
        );

        Instantiate(targetPrefab, position, Quaternion.identity * targetPrefab.transform.rotation);
    }
}
