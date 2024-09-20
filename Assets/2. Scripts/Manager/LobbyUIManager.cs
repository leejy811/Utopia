using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour, ISubject
{
    public UIElement setting;

    private List<IObserver> observers = new List<IObserver>();

    private void Start()
    {
        InitObserver();
    }

    #region OnClick

    public void OnClickSetting()
    {
        notifyObserver(EventState.Setting);
    }

    public void OnClickGameStart()
    {
        GameManager.instance.LoadGameScene();
    }

    public void OnClickQuitGame()
    {
        GameManager.instance.QuitGame();
    }

    #endregion

    #region Observer
    private void InitObserver()
    {
        addObserver(setting);
    }

    public void addObserver(IObserver observer)
    {
        if (observer != null)
            observers.Add(observer);
        else
            Debug.LogError("Null Observer : " + observer.ToString());
    }

    public void removeObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void notifyObserver(EventState state)
    {
        foreach (IObserver observer in observers)
        {
            observer.Notify(state);
        }
    }
    #endregion
}
