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
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")) && 
                (state == BuyState.None || state == BuyState.SolveEvent || state == BuyState.BuyOption))
            {
                ShopManager.instance.ChangeState(BuyState.SolveEvent, 0, hit.transform.gameObject);
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
                switch(state)
                {
                    case BuyState.BuyBuilding:
                        ShopManager.instance.BuyBuilding(hit.transform);
                        break;
                    case BuyState.BuyTile:
                        ShopManager.instance.BuyTile();
                        break;
                    case BuyState.BuildTile:
                        break;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")) &&
                (state == BuyState.None || state == BuyState.SolveEvent || state == BuyState.BuyOption))
            {
                if (hit.transform.gameObject.GetComponent<ResidentialBuilding>() != null)
                    ShopManager.instance.ChangeState(BuyState.BuyOption, 0, hit.transform.gameObject);
            }
            else
            {
                if(state == BuyState.BuyTile)
                    ShopManager.instance.SetTargetObject(null, Color.green, Color.red);
                else if(state == BuyState.SellBuilding)
                    ShopManager.instance.SetTargetObject(null, Color.red, Color.white);

                ShopManager.instance.ChangeState(BuyState.None);
                UIManager.instance.OnClickBuildingBuyPopUp(-1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShopManager.instance.ChangeState(BuyState.None);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (state == BuyState.BuyBuilding)
                ShopManager.instance.RotatePickBuilding();
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
            {
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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")))
            {
                if (state == BuyState.SellBuilding)
                    ShopManager.instance.SetTargetObject(hit.transform.gameObject, Color.red, Color.white); ;
            }
            else if (state == BuyState.SellBuilding)
                ShopManager.instance.SetTargetObject(null, Color.red, Color.white);
        }
    }
}
