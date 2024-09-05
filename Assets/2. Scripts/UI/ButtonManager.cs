using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;

    private Button lastDisabledButton = null;

    void Start()
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

    private void OnEnable()
    {
        lastDisabledButton = buttons[0];
        buttons[0].onClick.Invoke();

        if (!UIManager.instance.lists[0].gameObject.activeSelf)
        {
            buttons[0].onClick.Invoke();
        }

        foreach (Button btn in buttons)
        {
            btn.interactable = true;
        }

        buttons[0].interactable = false;
        lastDisabledButton = buttons[0];
    }
}
