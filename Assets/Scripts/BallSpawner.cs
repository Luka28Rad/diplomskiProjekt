using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples;

public class BallSpawner : MonoBehaviour
{
    public GameObject tennis;
    public GameObject football;
    public GameObject bowling;
    public GameObject spear;
    public GameObject prefab;

    void Start()
    {
        ApplyGravity();
        SpawnBall();
    }

    void ApplyGravity()
    {
        Physics.gravity = new Vector3(0f, GameManager.Instance.gravityStrength, 0f);

    }

    public void SpawnBall()
    {

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
        ball.GetComponent<EnhancedThrowable>().m_ReleaseThreshold = GameManager.Instance.threshold;
    }
    
    public void RespawnBall()
    {
        GameObject ball = Instantiate(prefab, transform.position, Quaternion.identity);
        ball.GetComponent<Rigidbody>().mass =
            Mathf.Clamp(GameManager.Instance.ballMass, 0.1f, 20f);
    }
}
