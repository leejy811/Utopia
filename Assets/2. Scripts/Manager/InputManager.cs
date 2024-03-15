using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                switch(state)
                {
                    case BuyState.BuyBuilding:
                        ShopManager.instance.BuyBuilding(hit.transform);
                        break;
                    case BuyState.SellBuilding:
                        ShopManager.instance.SellBuilding(hit.transform);
                        break;
                    case BuyState.BuyTile:
                        ShopManager.instance.BuyTile(hit.transform);
                        break;
                    case BuyState.BuildTile:
                        break;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")))
                UIManager.instance.SetOptionPopUp(true);
            else
                ShopManager.instance.ChangeState(BuyState.None);
        }
        else
        {
            if(state == BuyState.SellBuilding && Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Building")
                    ShopManager.instance.SetSellBuilding(hit.transform.gameObject);
                else
                    ShopManager.instance.SetSellBuilding(null);
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                switch (state)
                {
                    case BuyState.BuyBuilding:
                        ShopManager.instance.CheckBuyBuilding(hit.transform);
                        break;
                    case BuyState.SellBuilding:
                        break;
                    case BuyState.BuyTile:
                        break;
                    case BuyState.BuildTile:
                        break;
                }
            }
        }
    }
}
