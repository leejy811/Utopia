using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LobbyUIManager : MonoBehaviour, ISubject
{
    [Header("Observer")]
    public LobbyMenuUI main;
    public UIElement mapSelect;

    [Header("UIElement")]
    public Button continueButton;
    public GameObject optionPanel;

    [Header("Image")]
    public Image mainImage;
    public Image optionImage;
    public Sprite[] mainSprites;
    public Sprite[] optionSprites;

    private List<IObserver> observers = new List<IObserver>();

    #region EventFunc

    private void Start()
    {
        InitObserver();
        SetButton();
        GameManager.instance.skipTutorial = false;
    }

    private void SetButton()
    {
        if(File.Exists(Application.persistentDataPath + "/MapData_" + MapType.Utopia.ToString())
            || File.Exists(Application.persistentDataPath + "/MapData_" + MapType.Totopia.ToString()))
        {
            continueButton.interactable = true;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            continueButton.GetComponent<UIMouseOver>().interactable = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            notifyObserver(EventState.None);
    }

    public void SetMainColor(int idx)
    {
        mainImage.sprite = mainSprites[idx];
        optionImage.sprite = optionSprites[idx];
    }

    #endregion

    #region OnClick

    public void OnClickClose()
    {
        notifyObserver(EventState.None);
    }

    public void OnClickSetting()
    {
        optionPanel.SetActive(!optionPanel.activeSelf);
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

    public void OnClickSkipTutorial(bool isSkip)
    {
        GameManager.instance.skipTutorial = isSkip;
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
