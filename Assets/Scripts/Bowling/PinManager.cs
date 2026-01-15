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
        
        int score = GetFallenPins();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(score);
            Debug.Log($"Bowling score saved: {score} pins knocked down");
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null - cannot save bowling score");
        }
        
        ResetAllPins();
        
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