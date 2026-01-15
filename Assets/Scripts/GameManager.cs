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
        
        // Initialize CSV
        InitializeCSV();
    }
    
    private void InitializeCSV()
    {
        csvFilePath = Path.Combine(Application.dataPath, "..", CSV_FILENAME);
        csvFilePath = Path.GetFullPath(csvFilePath);
        
        Debug.Log($"CSV will be saved at: {csvFilePath}");
        
        if (!File.Exists(csvFilePath))
        {
            CreateCSVWithHeaders();
        }
    }
    
    private void CreateCSVWithHeaders()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, false))
            {
                writer.WriteLine("DateTime,TargetDistance,TargetHeight,GravityStrength,BallType,BallMass,ThrowScores");
            }
            Debug.Log($"CSV file created with headers");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating CSV file: {e.Message}");
        }
    }
    
    public void AddScore(int score)
    {
        currentTestScores.Add(score);
        Debug.Log($"Score added: {score}. Total throws: {currentTestScores.Count}");
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveTestDataToCSV();
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveTestDataToCSV();
        }
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
        if (currentTestScores.Count == 0)
        {
            Debug.Log("No scores to save");
            return;
        }
        
        try
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {
                string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                string throwScores = string.Join(",", currentTestScores);
                
                string csvLine = $"{dateTime},{targetDistance},{targetHeight},{gravityStrength},{ballType},{ballMass},{throwScores}";
                writer.WriteLine(csvLine);
                
                Debug.Log($"CSV saved: {csvLine}");
            }
            
            Debug.Log($"Test data saved to CSV. Throws recorded: {currentTestScores.Count}");
            currentTestScores.Clear();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error writing to CSV: {e.Message}");
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
        // Clear previous test scores when starting new game
        currentTestScores.Clear();
        Debug.Log("Starting new game - scores reset");
        
        SceneManager.LoadScene("SampleScene");
    }
    
    // Manual save method for testing
    [ContextMenu("Save CSV Now")]
    public void ManualSaveCSV()
    {
        Debug.Log("Manual CSV save triggered");
        SaveTestDataToCSV();
    }
    
    [ContextMenu("TEST: Simulate Throws + Save")]
    public void TestSimulateThrowsAndSave()
    {

        currentTestScores.Clear();

        // Simulirana bacanja
        currentTestScores.Add(10);
        currentTestScores.Add(8);
        currentTestScores.Add(6);
        currentTestScores.Add(9);
        currentTestScores.Add(7);


        SaveTestDataToCSV();

    }

}



public enum BallType
{
    Tennis,
    Spear,
    Bowling,
}
