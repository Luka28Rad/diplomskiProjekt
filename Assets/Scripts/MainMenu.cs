using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject[] mainMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void unhideUIelements()
    {
        for(int i = 0; i < mainMenuUI.Length; i++) mainMenuUI[i].SetActive(!mainMenuUI[i].activeInHierarchy);
    }

    public void quitGame()
    {
        Application.Quit();
    }
    
}
