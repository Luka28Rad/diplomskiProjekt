using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PinManager : MonoBehaviour
{
    public static PinManager Instance;
    public List<BowlingPin> pins = new List<BowlingPin>();
    public float resetDelay = 4.0f;
    private bool isResetRoutineRunning = false;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void OnPinFallen()
    {
        if (!isResetRoutineRunning)
        {
            StartCoroutine(ResetGameRoutine());
        }
    }
    
    private IEnumerator ResetGameRoutine()
    {
        isResetRoutineRunning = true;
        Debug.Log("First pin fell - countdown started");

        yield return new WaitForSeconds(resetDelay);

        Debug.Log("Reset game bowling");
        
        // Get the score BEFORE resetting pins
        int score = GetFallenPins();
        
        // Save the bowling score to the current session
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(score);
            Debug.Log($"Bowling score added to session: {score} pins knocked down");
        }
        
        ResetAllPins();
        
        // Save current session data to CSV after each bowling throw
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveOnTargetReset();
            Debug.Log("Bowling session saved to CSV");
        }
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        isResetRoutineRunning = false;
    }

    public int GetFallenPins()
    {
        int count = 0;
        foreach (BowlingPin pin in pins)
        {
            if (pin.isFallen) count++;
        }
        return count;
    }

    public void ResetAllPins()
    {
        foreach (BowlingPin pin in pins)
        {
            pin.ResetPin();
        }
    }
}