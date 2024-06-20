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
    public Button[] alpha3Buttons;

    bool onAlpha1 = false;
    bool onAlpha3 = false;

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
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
        else if (Input.GetMouseButton(0))
        {
            if (IsPointerOverUIObject(Input.mousePosition)) return;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                if (state == BuyState.BuyTile)
                {
                    ShopManager.instance.AddTile(hit.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (state == BuyState.BuyTile)
            {
                ShopManager.instance.BuyTile();
            }
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.notifyObserver(EventState.None);
            ShopManager.instance.ChangeState(BuyState.None);
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
            UIManager.instance.OnClickSpaceBar();
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
                    case BuyState.BuyTile:
                        ShopManager.instance.SetTargetObject(hit.transform.gameObject, Color.green, Color.red);
                        break;
                }
            }
            else if (state == BuyState.BuyTile)
                ShopManager.instance.SetTargetObject(null, Color.green, Color.red);

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
            else if (onAlpha3)
                InvokeButton(alpha3Buttons[idx]);
            else
                InvokeButton(buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha2);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha3)
                InvokeButton(alpha3Buttons[idx]);
            else
                InvokeButton(buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha3);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha3)
                InvokeButton(alpha3Buttons[idx]);
            else
                InvokeButton(buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha4);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha3)
                InvokeButton(alpha3Buttons[idx]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            int idx = NumKeyCodeToInt(KeyCode.Alpha5);
            if (onAlpha1)
                InvokeButton(alpha1Buttons[idx]);
            else if (onAlpha3)
                InvokeButton(alpha3Buttons[idx]);
        }
    }

    private bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    private int NumKeyCodeToInt(KeyCode code)
    {
        return (int)code - (int)KeyCode.Alpha1;
    }

    private void InvokeButton(Button button)
    {
        if (button.interactable)
        {
            button.Select();
            button.onClick.Invoke();
        }
    }

    public void Notify(EventState state)
    {
        if (state == EventState.Construct && !onAlpha1)
            onAlpha1 = true;
        else
            onAlpha1 = false;

        if (state == EventState.EtcFunc && !onAlpha3)
            onAlpha3 = true;
        else
            onAlpha3 = false;
    }
}
