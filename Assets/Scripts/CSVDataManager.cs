using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;
using System.Linq;

public class CSVDataManager : MonoBehaviour
{
    public static CSVDataManager Instance;
    
    private string csvFilePath;
    private const string CSV_FILENAME = "TestData.csv";
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Save CSV in project root folder
        csvFilePath = Path.Combine(Application.dataPath, "..", CSV_FILENAME);
        csvFilePath = Path.GetFullPath(csvFilePath);
        
        Debug.Log($"CSVDataManager initialized. CSV path: {csvFilePath}");
        
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
                // Create headers with setup columns + throw score columns (up to 20 throws)
                List<string> headers = new List<string>
                {
                    "DateTime",
                    "TargetDistance", 
                    "TargetHeight",
                    "GravityStrength",
                    "BallType",
                    "BallMass"
                };
                
                // Add throw score columns
                for (int i = 1; i <= 20; i++)
                {
                    headers.Add($"Throw{i}");
                }
                
                writer.WriteLine(string.Join(",", headers));
            }
            Debug.Log($"CSV file created at: {csvFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating CSV file: {e.Message}");
        }
    }
    
    public void SaveCurrentTest(List<int> scores)
    {
        Debug.Log($"SaveCurrentTest called with {scores.Count} scores");
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }
        
        // Removed the check that skipped empty tests - now always saves
        try
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {
                List<string> values = new List<string>();
                
                // Add setup data
                values.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                values.Add(GameManager.Instance.targetDistance.ToString());
                values.Add(GameManager.Instance.targetHeight.ToString());
                values.Add(GameManager.Instance.gravityStrength.ToString(CultureInfo.InvariantCulture));
                values.Add(GameManager.Instance.ballType.ToString());
                values.Add(GameManager.Instance.ballMass.ToString(CultureInfo.InvariantCulture));
                
                // Add throw scores (each in its own column)
                for (int i = 0; i < 20; i++) // Support up to 20 throws
                {
                    if (i < scores.Count)
                    {
                        values.Add(scores[i].ToString());
                    }
                    else
                    {
                        values.Add(""); // Empty cell for unused throw columns
                    }
                }
                
                string csvLine = string.Join(",", values);
                writer.WriteLine(csvLine);
                
                Debug.Log($"CSV line written: {csvLine}");
            }
            
            Debug.Log($"Test data saved to CSV. Throws recorded: {scores.Count}");
            Debug.Log($"CSV file location: {csvFilePath}");
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
}