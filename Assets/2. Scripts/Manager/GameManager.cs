using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MapType { Utopia, Totopia }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MapType curMapType;
    public bool isLoad;
    public bool skipTutorial;

    private PostProcessManager postProcess;
    private SoundManager soundManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        postProcess = PostProcessManager.instance;
        soundManager = SoundManager.instance;

        SceneManager.sceneLoaded += OnLoadedScene;
    }

    private void OnLoadedScene(Scene scene, LoadSceneMode mode)
    {
        postProcess.SetComponent();
        soundManager.SetBGM(scene.name);
    }

    public void LoadLobbyScene()
    {
        LoadingSceneController.Instance.LoadScene("LobbyScene");
    }

    public void LoadGameScene()
    {
        AkSoundEngine.PostEvent("Stop_ForestAmbienceInLobby", Camera.main.gameObject);
        AkSoundEngine.PostEvent("Stop_Lobby", Camera.main.gameObject);

        LoadingSceneController.Instance.LoadScene("InGameScene_" + (int)curMapType);
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
