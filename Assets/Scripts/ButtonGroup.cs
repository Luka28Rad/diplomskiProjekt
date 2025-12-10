using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color normalColor = Color.white;

    private Button currentSelected;

    public void SelectButton(Button button)
    {
        if (currentSelected != null)
            currentSelected.GetComponent<Image>().color = normalColor;

        currentSelected = button;
        currentSelected.GetComponent<Image>().color = selectedColor;
    }
}
