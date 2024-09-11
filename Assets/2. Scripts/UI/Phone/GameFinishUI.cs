using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishUI : MonoBehaviour, IObserver
{
    public GameObject gameClear;
    public GameObject gameOver;
    public GameObject panel;

    public float gameoverTime;
    public CinemachineVirtualCamera virtualCamera;

    private void Init()
    {
        gameObject.SetActive(true);
        panel.SetActive(true);
        InputManager.SetCanInput(false);
        RoutineManager.instance.OnOffDailyLight(false);
    }

    IEnumerator GameOver()
    {
        InputManager.SetCanInput(false);
        RoutineManager.instance.OnOffDailyLight(false);
        foreach (Tile tile in Grid.instance.tiles)
        {
            if(tile.CheckPurchased()) continue;

            tile.smokeFXLoop.Play();

            if (tile.building != null)
                tile.building.transform.DOMoveY(-1f, gameoverTime);
        }

        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 5.0f;
        noise.m_FrequencyGain = 1.0f;

        yield return new WaitForSeconds(gameoverTime);

        foreach (Tile tile in Grid.instance.tiles)
        {
            tile.smokeFXLoop.Stop();
        }

        noise.m_AmplitudeGain = 0.0f;
        noise.m_FrequencyGain = 0.0f;

        Init();
        gameOver.gameObject.SetActive(true);
    }

    public void Notify(EventState state)
    {
        if (state == EventState.GameClear)
        {
            Init();
            gameClear.SetActive(true);
        }
        else if(state == EventState.GameOver)
        {
            GameManager.instance.StartCoroutine(GameOver());
        }
    }
}
