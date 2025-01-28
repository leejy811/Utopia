using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhoneUI : MonoBehaviour, IObserver
{
    public EndGameUI gameClear;
    public EndGameUI gameOver;
    public GameObject backGround;

    public float gameoverTime;
    public CinemachineVirtualCamera virtualCamera;

    private EventState curState;

    private void Init()
    {
        backGround.gameObject.SetActive(true);

        if (curState == EventState.GameClear)
            gameClear.gameObject.SetActive(true);
        else if (curState == EventState.GameOver)
            gameOver.gameObject.SetActive(true);
    }

    IEnumerator GameOver()
    {
        foreach (Tile tile in Grid.instance.tiles)
        {
            if (tile.CheckPurchased()) continue;

            tile.smokeFX.Play();

            if (tile.building != null)
                tile.building.transform.DOMoveY(-2.2f, gameoverTime);
        }

        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 5.0f;
        noise.m_FrequencyGain = 1.0f;

        yield return new WaitForSeconds(gameoverTime);

        noise.m_AmplitudeGain = 0.0f;
        noise.m_FrequencyGain = 0.0f;

        Init();
    }

    public void Notify(EventState state)
    {
        if (state == EventState.GameClear || state == EventState.GameOver)
        {
            gameObject.SetActive(true);
            InputManager.SetCanInput(false);
            RoutineManager.instance.OnOffDailyLight(false);
            curState = state;

            if (state == EventState.GameOver)
                StartCoroutine(GameOver());
            else
                Init();
        }
    }
}