using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyQuit : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); // ���ø����̼� ����
        #endif
    }
}
