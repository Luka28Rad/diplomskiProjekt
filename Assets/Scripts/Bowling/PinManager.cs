using UnityEngine;
using System.Collections.Generic;

public class PinManager : MonoBehaviour
{
    public List<BowlingPin> pins = new List<BowlingPin>();

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