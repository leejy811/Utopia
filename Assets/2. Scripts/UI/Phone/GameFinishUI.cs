using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishUI : MonoBehaviour, IObserver
{
    public GameObject gameClear;
    public GameObject gameOver;
    public GameObject panel;

    public void Notify(EventState state)
    {
        if (state == EventState.GameClear || state == EventState.GameOver)
        {
            gameObject.SetActive(true);
            panel.SetActive(true);

            InputManager.SetCanInput(false);
            RoutineManager.instance.OnOffDailyLight(false);

            if (state == EventState.GameClear)
            {
                gameClear.SetActive(true);
            }
            else
            {
                gameOver.SetActive(true);
            }
        }
    }
}
