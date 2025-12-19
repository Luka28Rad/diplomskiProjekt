using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] public TMP_InputField gravityInput;
    public TMP_InputField massInput;
    public TMP_Dropdown ballTypeDropdown;

    private Button currentSelected;

    public void SelectButton(Button button)
    {
        if (currentSelected != null)
            currentSelected.GetComponent<Image>().color = normalColor;

        currentSelected = button;
        currentSelected.GetComponent<Image>().color = selectedColor;
    }
    
}
