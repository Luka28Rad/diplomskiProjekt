using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Samples;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Study Info")]
    public string userID = "Unknown";
    public StudyScenario scenario;
    public ThresholdVariant thresholdVariant;

    [Header("Base Parameters")]
    public float baseReleaseThreshold = 0.3f;
    public float gravityStrength = -9.81f;

    [Header("Target Setup")]
    [Range(1, 3)] public int targetDistance = 2;
    [Range(1, 3)] public int targetHeight = 2;

    [Header("Mass Presets")]
    public float tennisMass = 0.057f;
    public float bowlingMass = 7.2f;
    public float spearMass = 1.2f;

    [Header("Runtime (auto from scenario)")]
    public float activeBallMass;
    public ThrowingStyle activeThrowingStyle;

    [Header("Runtime Throws")]
    public int throwsPerScenario = 5;
    public int currentThrow = 0;

    private List<int> scores = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyScenarioPreset()
    {
        switch (scenario)
        {
            case StudyScenario.TennisOverhand:
                activeBallMass = tennisMass;
                activeThrowingStyle = ThrowingStyle.Baseball;
                break;

            case StudyScenario.TennisUnderhand:
                activeBallMass = tennisMass;
                activeThrowingStyle = ThrowingStyle.Underhand;
                break;

            case StudyScenario.BowlingUnderhand:
                activeBallMass = bowlingMass;
                activeThrowingStyle = ThrowingStyle.Underhand;
                break;

            case StudyScenario.SpearOverhand:
                activeBallMass = spearMass;
                activeThrowingStyle = ThrowingStyle.Spear;
                break;
        }
    }

    public float GetActiveThreshold()
    {
        return thresholdVariant switch
        {
            ThresholdVariant.Plus => baseReleaseThreshold + 0.1f,
            ThresholdVariant.Minus => baseReleaseThreshold - 0.1f,
            _ => baseReleaseThreshold
        };
    }


    public float GetActiveMass()
    {
        return activeBallMass;
    }

    public void StartScenario()
    {
        scores.Clear();
        currentThrow = 0;
        SceneManager.LoadScene("SampleScene");
    }

    public void AddScore(int score)
    {
        scores.Add(score);
        currentThrow++;

        if (currentThrow >= throwsPerScenario)
        {
            CSVDataManager.Instance.SaveCurrentTest(scores);
            SceneManager.LoadScene("MainMenu");
        }
    }
}



public enum StudyScenario
{
    TennisOverhand,
    TennisUnderhand,
    BowlingUnderhand,
    SpearOverhand
}

public enum ThresholdVariant
{
    Base,
    Plus,
    Minus
}
