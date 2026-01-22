using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject[] mainMenuUI;
    
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField userIdInput;
    [SerializeField] private TMP_InputField massInput;

    [Header("Dropdowns")]
    [SerializeField] private TMP_Dropdown thresholdDropdown;
    [SerializeField] private TMP_Dropdown ballTypeDropdown;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void RefreshUI()
    {
        var gm = GameManager.Instance;

        // Input fields
        userIdInput.SetTextWithoutNotify(gm.userID.ToString());
        massInput.SetTextWithoutNotify(gm.ballMass.ToString());

        // Dropdowns
        ballTypeDropdown.SetValueWithoutNotify((int)gm.ballType);
        thresholdDropdown.SetValueWithoutNotify(GetThresholdIndex(gm.threshold));
    }

    public void unhideUIelements()
    {
        for(int i = 0; i < mainMenuUI.Length; i++) mainMenuUI[i].SetActive(!mainMenuUI[i].activeInHierarchy);
    }

    public void StartGameUsingUI()
    {
        GameManager.Instance.StartGame();
    }

    public void SetThresholdUsingUI(int index)
    {
        GameManager.Instance.SetThreshold(index);
    }

    public void SetUserIDUsingUI(string value)
    {
        GameManager.Instance.SetUserID(value);        
    }

    public void SetMassUsingUI(string value)
    {
        GameManager.Instance.SetMass(value);
    }

    public void SetBallTypeUsingUI(int index)
    {
        GameManager.Instance.SetBallType(index);
    }

    public void quitGame()
    {
        Application.Quit();
    }
    
    private int GetThresholdIndex(float value)
    {
        var gm = GameManager.Instance;

        for (int i = 0; i <= 2; i++)
        {
            gm.SetThreshold(i);
            if (Mathf.Approximately(gm.threshold, value))
                return i;
        }

        return 0;
    }
    
}
