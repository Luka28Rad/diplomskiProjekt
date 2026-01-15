using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples;

public class BallSpawner : MonoBehaviour
{
    public GameObject tennis;
    public GameObject bowling;
    public GameObject spear;

    void Start()
    {
        Physics.gravity =
            new Vector3(0, GameManager.Instance.gravityStrength, 0);

        Spawn();
    }

    void Spawn()
    {
        GameObject prefab = null;

        switch (GameManager.Instance.scenario)
        {
            case StudyScenario.TennisOverhand:
            case StudyScenario.TennisUnderhand:
                prefab = tennis;
                break;

            case StudyScenario.BowlingUnderhand:
                prefab = bowling;
                break;

            case StudyScenario.SpearOverhand:
                prefab = spear;
                break;
        }

        GameObject ball =
            Instantiate(prefab, transform.position, Quaternion.identity);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.mass = GameManager.Instance.activeBallMass;

        var et = ball.GetComponent<EnhancedThrowable>();
        et.SetThrowingStyle(GameManager.Instance.activeThrowingStyle);
        et.SetReleaseThreshold(GameManager.Instance.GetActiveThreshold());
    }
}
