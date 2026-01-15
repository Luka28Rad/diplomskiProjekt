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

    [Header("User Data")]
    public int userID;

    // CSV Data
    private List<int> currentTestScores = new List<int>();
    private string csvFilePath;
    private const string CSV_FILENAME = "TestData.csv";
    //public int userID;

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
        // Save CSV in project root folder
        csvFilePath = Path.Combine(Application.dataPath, "..", CSV_FILENAME);
        csvFilePath = Path.GetFullPath(csvFilePath);
        
        Debug.Log($"GameManager CSV initialized. CSV path: {csvFilePath}");
        
        if (!File.Exists(csvFilePath))
        {
            CreateCSVWithHeaders();
        }
        else
        {
            Debug.Log("CSV file already exists");
        }
    }
    
    private void CreateCSVWithHeaders()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, false))
            {
                // Create headers with userID as first column, then setup columns + scores as one field
                List<string> headers = new List<string>
                {
                    "UserID",
                    "DateTime",
                    "TargetDistance", 
                    "TargetHeight",
                    "GravityStrength",
                    "BallType",
                    "BallMass",
                    "Scores"
                };
                
                writer.WriteLine(string.Join(",", headers));
            }
            Debug.Log($"CSV file created at: {csvFilePath}");
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
    
    // Method to trigger CSV save on target reset
    public void SaveOnTargetReset()
    {
        if (currentTestScores.Count > 0)
        {
            Debug.Log("Target reset triggered - saving current session to CSV");
            SaveTestDataToCSV();
        }
        else
        {
            Debug.Log("Target reset triggered - no scores to save");
        }
    }
    
    private void SaveTestDataToCSV()
    {
        Debug.Log($"SaveTestDataToCSV called with {currentTestScores.Count} scores");
        
        // Only save if there are scores to save
        if (currentTestScores.Count == 0)
        {
            Debug.Log("No scores to save - skipping CSV write");
            return;
        }
        
        try
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {
                List<string> values = new List<string>();
                
                // Add userID as first column
                values.Add(userID.ToString());
                
                // Add setup data
                values.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                values.Add(targetDistance.ToString());
                values.Add(targetHeight.ToString());
                values.Add(gravityStrength.ToString(CultureInfo.InvariantCulture));
                values.Add(ballType.ToString());
                values.Add(ballMass.ToString(CultureInfo.InvariantCulture));

                // Convert scores to string and join with semicolons (or spaces/pipes)
                string scoresString = string.Join(";", currentTestScores);
                values.Add($"\"{scoresString}\""); // Wrap in quotes to handle potential commas
                
                string csvLine = string.Join(",", values);
                writer.WriteLine(csvLine);
                
                Debug.Log($"CSV line written: {csvLine}");
            }
            
            Debug.Log($"Test data saved to CSV. Throws recorded: {currentTestScores.Count}");
            Debug.Log($"CSV file location: {csvFilePath}");
            currentTestScores.Clear(); // Clear scores after saving
        }
        catch (Exception e)
        {
            Debug.LogError($"Error writing to CSV file: {e.Message}");
        }
    }
    
    public string GetCSVFilePath()
    {
        return csvFilePath;
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
    
    // Manual save method for testing
    [ContextMenu("Force Save CSV Test")]
    public void ForceSaveCSVTest()
    {
        Debug.Log("Force saving CSV with current data...");
        SaveTestDataToCSV();
    }
    
    [ContextMenu("TEST: Simulate Throws + Save")]
    public void TestSimulateThrowsAndSave()
    {
        currentTestScores.Clear();

        // Simulated throws including zeros
        currentTestScores.Add(10);
        currentTestScores.Add(0);  // Miss
        currentTestScores.Add(6);
        currentTestScores.Add(0);  // Miss
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
