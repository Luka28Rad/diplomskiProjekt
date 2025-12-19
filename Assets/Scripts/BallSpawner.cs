using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject tennis;
    public GameObject football;
    public GameObject bowling;
    public GameObject spear;

    void Start()
    {
        ApplyGravity();
        SpawnBall();
    }

    void ApplyGravity()
    {
        float g = Mathf.Clamp(GameManager.Instance.gravityStrength, -20f, -1f);
        Physics.gravity = new Vector3(0, g, 0);
    }

    void SpawnBall()
    {
        GameObject prefab = tennis;

        switch (GameManager.Instance.ballType)
        {
            case BallType.Tennis:
                prefab = tennis;
                break;
            case BallType.Bowling:
                prefab = bowling;
                break;
            case BallType.Spear:
                prefab = spear;
                break;
        }

        GameObject ball = Instantiate(prefab, transform.position, Quaternion.identity);
        ball.GetComponent<Rigidbody>().mass =
            Mathf.Clamp(GameManager.Instance.ballMass, 0.1f, 20f);
    }
}
