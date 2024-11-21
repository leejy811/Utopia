using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIManager : MonoBehaviour, ISubject
{
    [Header("Observer")]
    public LobbyMenuUI main;
    public UIElement mapSelect;

    [Header("UIElement")]
    public Button continueButton;

    private List<IObserver> observers = new List<IObserver>();

    #region EventFunc

    private void Start()
    {
        InitObserver();
        SetButton();
    }

    private void SetButton()
    {
        if(File.Exists(Application.persistentDataPath + "/MapData_" + MapType.Utopia.ToString())
            || File.Exists(Application.persistentDataPath + "/MapData_" + MapType.Totopia.ToString())
            || File.Exists(Application.persistentDataPath + "/MapData_" + MapType.SnowRabbit.ToString()))
        {
            continueButton.interactable = true;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            notifyObserver(EventState.None);
    }

    #endregion

    #region OnClick

    public void OnClickClose()
    {
        notifyObserver(EventState.None);
    }

    public void OnClickGameStart(bool isLoad)
    {
        GameManager.instance.isLoad = isLoad;
        notifyObserver(EventState.MapSelect);
    }

    public void OnClickMap(int type)
    {
        GameManager.instance.curMapType = (MapType)type;
        GameManager.instance.LoadGameScene();
    }

    public void OnClickQuitGame()
    {
        GameManager.instance.QuitGame();
    }

    public void OnClickButtonSound(string name)
    {
        SoundManager.instance.PostEvent(name);
    }

    #endregion

    #region Observer
    private void InitObserver()
    {
        addObserver(main);
        addObserver(mapSelect);
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
