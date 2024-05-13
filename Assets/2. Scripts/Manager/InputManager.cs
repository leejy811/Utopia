using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
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
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                if (Grid.instance.isInfluenceMode)
                {
                    Grid.instance.NotifyTileInfluence(hit.transform);
                }
                switch (state)
                {
                    case BuyState.BuyBuilding:
                        ShopManager.instance.CheckBuyBuilding(hit.transform);
                        break;
                    case BuyState.BuyTile:
                        ShopManager.instance.SetTargetObject(hit.transform.gameObject, Color.green, Color.red);
                        break;
                    case BuyState.BuildTile:
                        break;
                }
            }
            else if (state == BuyState.BuyTile)
                ShopManager.instance.SetTargetObject(null, Color.green, Color.red);
            else if (Grid.instance.isInfluenceMode)
                UIManager.instance.SetTileInfluencePopUp(null);

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
            UIManager.instance.OnClickBuyPopUp(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UIManager.instance.OnClickBuyPopUp(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UIManager.instance.OnClickBuyPopUp(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UIManager.instance.OnClickBuyPopUp(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UIManager.instance.OnClickBuildingSell();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UIManager.instance.OnClickTileBuy();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            UIManager.instance.OnClickEventHighLight();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            UIManager.instance.OnClickEventNotify();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            UIManager.instance.OnClickTileColorMode();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            UIManager.instance.OnClickTileInfluenceMode();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            UIManager.instance.OnClickCityLevelMode();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (UIManager.instance.statistic.gameObject.activeSelf)
                UIManager.instance.OnClickCloseStatistic();
            else
                UIManager.instance.OnClickNextDay();
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
}
