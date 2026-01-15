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
    public int targetDistance = 1;     
    public int targetHeight = 1;       
    public float gravityStrength; 
    public BallType ballType;
    public float ballMass;
    public float threshold;

    // CSV Data
    private List<int> currentTestScores = new List<int>();

    private string csvFilePath;
    private const string CSV_FILENAME = "TestData.csv";
    private int userID;

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
    
    public void SetTargetDistance(int value) //ne koristimo vise direktno vec samo indirektno kroz TargetSpawner
    {
        targetDistance = value;
    }

    public void SetTargetHeight(int value)  //ne koristimo vise direktno vec samo indirektno kroz TargetSpawner
    {
        targetHeight = value;
    }
    
    public void SetGravity(string value) //ne koristimo vise
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

    public void SetThreshold(int index)
    {
        threshold = 0.5f; //default

        if (index == 0)
        {
            threshold = 0.5f; //suvisno, ali nvz
        }
        else if (index == 1)
        {
            threshold += 0.1f;
        }
        else if (index == 2)
        {
            threshold -= 0.1f;
        }
    }

    public void SetMass(string value) //ne koristimo vise
    {
        if (float.TryParse(value, out float m))
        {
            ballMass = m;
        }
    }

    public void SetUserID(string value)
    {
        if (int.TryParse(value, out int m))
        {
            userID = m;
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
    Spear,
    Bowling,
}
