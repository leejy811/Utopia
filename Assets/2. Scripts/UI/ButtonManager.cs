using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;

    private Button lastDisabledButton = null;

    void Update()
    {
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => ButtonClicked(btn));
        }
    }

    void ButtonClicked(Button clickedButton)
    {
        if (lastDisabledButton != null)
        {
            lastDisabledButton.interactable = true;
        }

        clickedButton.interactable = false;
        lastDisabledButton = clickedButton;
    }
}
