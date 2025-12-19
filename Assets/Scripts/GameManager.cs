using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Throw Setup")]
    public int targetDistance;     
    public int targetHeight;       
    public float gravityStrength; 
    public BallType ballType;
    public float ballMass;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SetTargetDistance(int value)
    {
        GameManager.Instance.targetDistance = value;
    }

    public void SetTargetHeight(int value)
    {
        GameManager.Instance.targetHeight = value;
    }
    public void SetGravity(string value)
    {
        if (float.TryParse(value, out float g))
        {
            GameManager.Instance.gravityStrength = g;
        }
    }
    public void SetBallType(int index)
    {
        GameManager.Instance.ballType = (BallType)index;
    }
    public void SetMass(string value)
    {
        if (float.TryParse(value, out float m))
        {
            GameManager.Instance.ballMass = m;
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

}

public enum BallType
{
    Tennis,
    Football,
    Bowling,
    Spear
}
