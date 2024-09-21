using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour, ISubject
{
    public UIElement main;
    public UIElement mapSelect;
    public UIElement setting;

    private List<IObserver> observers = new List<IObserver>();

    #region EventFunc

    private void Start()
    {
        InitObserver();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            notifyObserver(EventState.None);
    }

    #endregion

    #region OnClick

    public void OnClickSetting()
    {
        notifyObserver(EventState.Setting);
    }

    public void OnClickGameStart()
    {
        notifyObserver(EventState.MapSelect);
    }

    public void OnClickMap()
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
        addObserver(main);
        addObserver(mapSelect);
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
