using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

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

        csvFilePath = Path.Combine(Application.dataPath, "..", CSV_FILENAME);
        csvFilePath = Path.GetFullPath(csvFilePath);

        Debug.Log($"CSVDataManager initialized. CSV path: {csvFilePath}");

        if (!File.Exists(csvFilePath))
            CreateCSVWithHeaders();
    }

    private void CreateCSVWithHeaders()
    {
        using (StreamWriter writer = new StreamWriter(csvFilePath, false))
        {
            List<string> headers = new List<string>
            {
                "DateTime",
                "UserID",
                "Scenario",
                "ThresholdVariant",
                "ReleaseThreshold",
                "BallMass"
            };

            // max 5 bacanja po varijanti
            for (int i = 1; i <= 5; i++)
                headers.Add($"Throw{i}");

            writer.WriteLine(string.Join(",", headers));
        }
    }

    public void SaveCurrentTest(List<int> scores)
    {
        if (GameManager.Instance == null || scores.Count == 0)
            return;

        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            List<string> values = new List<string>
            {
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                GameManager.Instance.userID,
                GameManager.Instance.scenario.ToString(),
                GameManager.Instance.thresholdVariant.ToString(),
                GameManager.Instance.GetActiveThreshold().ToString(CultureInfo.InvariantCulture),
                GameManager.Instance.GetActiveMass().ToString(CultureInfo.InvariantCulture)
            };

            for (int i = 0; i < 5; i++)
                values.Add(i < scores.Count ? scores[i].ToString() : "");

            writer.WriteLine(string.Join(",", values));
        }
    }
}
