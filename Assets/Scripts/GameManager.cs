using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

    // CSV Data
    private List<int> currentTestScores = new List<int>();

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
    
    public void AddScore(int score)
    {
        currentTestScores.Add(score);
        Debug.Log($"Score added: {score}. Total throws: {currentTestScores.Count}");
    }
    
    private void OnApplicationQuit()
    {
        Debug.Log("GameManager: Application quitting - saving CSV data");
        SaveTestDataToCSV();
    }
    
    void OnDestroy()
    {
        Debug.Log("GameManager: OnDestroy - saving CSV data");
        SaveTestDataToCSV();
    }
    
    private void SaveTestDataToCSV()
    {
        // Use CSVDataManager instead of handling CSV directly
        if (CSVDataManager.Instance != null)
        {
            CSVDataManager.Instance.SaveCurrentTest(currentTestScores);
            currentTestScores.Clear();
        }
    }
    
    public void SetTargetDistance(int value)
    {
        targetDistance = value;
    }

    public void SetTargetHeight(int value)
    {
        targetHeight = value;
    }
    
    public void SetGravity(string value)
    {
        if (float.TryParse(value, out float g))
        {
            gravityStrength = g;
        }
    }
    
    public void SetBallType(int index)
    {
        ballType = (BallType)index;
    }
    
    public void SetMass(string value)
    {
        if (float.TryParse(value, out float m))
        {
            ballMass = m;
        }
    }
    
    public void StartGame()
    {
        currentTestScores.Clear();
        Debug.Log("Starting new game - scores reset");
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
