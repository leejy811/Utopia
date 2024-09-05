using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MCSS soundBnk;

    private PostProcessManager postProcess;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        postProcess = PostProcessManager.instance;
    }

    public void LoadLobbyScene()
    {
        soundBnk.StopInGameBGM();

        StartCoroutine(PlayLoadScene("LobbyScene"));
    }

    public void LoadGameScene()
    {
        AkSoundEngine.PostEvent("Stop_ForestAmbienceInLobby", Camera.main.gameObject);
        AkSoundEngine.PostEvent("Stop_Lobby", Camera.main.gameObject);

        StartCoroutine(PlayLoadScene("InGameScene"));
    }

    IEnumerator PlayLoadScene(string SceneName)
    {
        StartCoroutine(postProcess.FadeInOut(1.5f, true));

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); // 어플리케이션 종료
        #endif
    }
}
