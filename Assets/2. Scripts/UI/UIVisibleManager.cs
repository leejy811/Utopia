using UnityEngine;
using UnityEngine.UI;


public class UIVisibleManager : MonoBehaviour
{
    public GameObject panel; 

    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }
}

