using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePhoneUI : MonoBehaviour, IObserver
{
    public EndGameUI gameClear;
    public EndGameUI gameOver;

    public float moveTime;
    public float waitTime;

    private EventState curState;

    private void OnEnable()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -450.0f, transform.localPosition.z);
        AkSoundEngine.PostEvent("Play_CellPhone_01", gameObject);

        StartCoroutine(InitPhone());
    }

    IEnumerator InitPhone()
    {
        transform.DOLocalMoveY(0.0f, moveTime).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(moveTime + waitTime);

        if (curState == EventState.GameClear)
            gameClear.gameObject.SetActive(true);
        else if (curState == EventState.GameOver)
            gameOver.gameObject.SetActive(true);
    }

    public void Notify(EventState state)
    {
        if(state == EventState.GameClear || state == EventState.GameOver)
        {
            curState = state;
        }
    }
}