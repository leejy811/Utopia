using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IObserver
{
    public static bool canInput = false;

    public Button[] buttons;

    public Button[] alpha1Buttons;
    public Button[] alpha2Buttons;

    bool onAlpha1 = false;
    bool onAlpha2 = false;

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (UIManager.instance.eventRoulette.gameObject.activeSelf)
            GetSlotMachineInput();

        if (UIManager.instance.menu.gameObject.activeSelf || UIManager.instance.setting.gameObject.activeSelf)
            GetMenuInput();

        if (!canInput) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        BuyState state = ShopManager.instance.buyState;

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIObject(Input.mousePosition)) return;

            if (state == BuyState.SellBuilding)
            {
                ShopManager.instance.SellBuilding();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")) &&
                (state == BuyState.None || state == BuyState.SolveBuilding))
            {
                if (hit.transform.gameObject.GetComponent<Building>().viewState == ViewStateType.Transparent) return;
                ShopManager.instance.ChangeState(BuyState.SolveBuilding, 0, hit.transform.gameObject);
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                switch (state)
                {
                    case BuyState.BuyBuilding:
                        ShopManager.instance.BuyBuilding(hit.transform);
                        break;
                    case BuyState.BuildTile:
                        ShopManager.instance.BuildTile(hit.transform);
                        break;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            UIManager.instance.notifyObserver(EventState.None);
            ShopManager.instance.ChangeState(BuyState.None);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.notifyObserver(EventState.Menu);
            ShopManager.instance.ChangeState(BuyState.None);
            canInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (state == BuyState.BuyBuilding)
                ShopManager.instance.RotatePickBuilding();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (state == BuyState.BuyBuilding)
                Grid.instance.SetTileColorMode();
            else
                UIManager.instance.notifyObserver(EventState.TileColor);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.instance.OnClickNextDay();
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                switch (state)
                {
                    case BuyState.BuildTile:
                    case BuyState.BuyBuilding:
                        ShopManager.instance.CheckBuyBuilding(hit.transform);
                        break;
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")))
            {
                if (state == BuyState.SellBuilding)
                    ShopManager.instance.SetTargetObject(hit.transform.gameObject, Color.red, Color.white);
            }
            else if (state == BuyState.SellBuilding)
                ShopManager.instance.SetTargetObject(null, Color.red, Color.white);
        }

        InputButtonShortKey();
    }

    private void InputButtonShortKey()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha1);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha2)
                InvokeButton(alpha2Buttons[idx]);
            else
                InvokeButton(buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha2);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha2)
                InvokeButton(alpha2Buttons[idx]);
            else
                InvokeButton(buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha3);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha2)
                InvokeButton(alpha2Buttons[idx]);
            else
                InvokeButton(buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha4);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha2)
                InvokeButton(alpha2Buttons[idx]);
            else
                InvokeButton(buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha5);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha2)
                InvokeButton(alpha2Buttons[idx]);
        }
    }

    private void GetSlotMachineInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            UIManager.instance.OnClickEventRoulette();
        }
        else if (UIManager.instance.eventRoulette.state != RouletteState.While && UIManager.instance.eventRoulette.state != RouletteState.Before)
        {
            if (Input.GetMouseButtonDown(1))
                UIManager.instance.notifyObserver(EventState.None);
            else if (Input.GetKeyDown(KeyCode.Escape))
                UIManager.instance.notifyObserver(EventState.Menu);
        }
    }

    private void GetMenuInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UIManager.instance.notifyObserver(EventState.None);
            canInput = true;
        }
    }

    private bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        int count = results.Count;
        foreach (RaycastResult res in results)
        {
            if (res.gameObject.tag == "CanOverUI")
                count--;
        }

        return count > 0;
    }

    private int NumKeyCodeToInt(KeyCode code)
    {
        return (int)code - (int)KeyCode.Alpha1;
    }

    private void InvokeButton(Button button)
    {
        if (button.interactable)
        {
            button.onClick.Invoke();
        }
    }

    public void Notify(EventState state)
    {
        if (state == EventState.Construct && !onAlpha1)
            onAlpha1 = true;
        else
            onAlpha1 = false;

        if (state == EventState.EtcFunc && !onAlpha2)
            onAlpha2 = true;
        else
            onAlpha2 = false;
    }

    public void SetCanInput(bool canInput)
    {
        InputManager.canInput = canInput;
    }
}
