using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField userIdInput;
    public TMP_InputField thresholdInput;
    public TMP_InputField gravityInput;
    public TMP_InputField targetDistanceInput;
    public TMP_InputField targetHeightInput;

    [Header("Dropdowns")]
    public TMP_Dropdown scenarioDropdown;
    public TMP_Dropdown variantDropdown;


    public void OnScenarioChanged(int value)
    {
        StudyScenario s = (StudyScenario)value;

        switch (s)
        {
            case StudyScenario.TennisOverhand:
            case StudyScenario.TennisUnderhand:
                thresholdInput.text = "0.3";
                break;

            case StudyScenario.BowlingUnderhand:
                thresholdInput.text = "0.35";
                break;

            case StudyScenario.SpearOverhand:
                thresholdInput.text = "0.25";
                break;
        }
    }

    public void StartStudy()
    {
        GameManager.Instance.userID = userIdInput.text;
        GameManager.Instance.baseReleaseThreshold = float.Parse(thresholdInput.text);
        GameManager.Instance.gravityStrength = float.Parse(gravityInput.text);
        GameManager.Instance.targetDistance = int.Parse(targetDistanceInput.text);
        GameManager.Instance.targetHeight = int.Parse(targetHeightInput.text);

        GameManager.Instance.scenario =
            (StudyScenario)scenarioDropdown.value;

        GameManager.Instance.thresholdVariant =
            (ThresholdVariant)variantDropdown.value;


        GameManager.Instance.ApplyScenarioPreset();

        GameManager.Instance.StartScenario();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
