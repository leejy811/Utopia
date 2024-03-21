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
            if(state == BuyState.SellBuilding)
            {
                ShopManager.instance.SellBuilding();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")) && state == BuyState.None)
            {
                UIManager.instance.SetBuildingPopUp(true, hit.transform.gameObject);
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                switch(state)
                {
                    case BuyState.BuyBuilding:
                        ShopManager.instance.BuyBuilding(hit.transform);
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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")) && state == BuyState.None)
            {
                if (hit.transform.gameObject.GetComponent<ResidentialBuilding>() != null)
                    ShopManager.instance.ChangeState(BuyState.BuyOption, 0, hit.transform.gameObject);
            }
            else
                ShopManager.instance.ChangeState(BuyState.None);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.SetBuildingPopUp(false);
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
