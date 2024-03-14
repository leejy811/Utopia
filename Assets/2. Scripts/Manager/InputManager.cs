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
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ShopManager.instance.ChangeState(BuyState.None);
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile"))) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.transform.CompareTag("Tile"))
            {
                BuyState state = ShopManager.instance.buyState;
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
        else
        {
            if (hit.transform.CompareTag("Tile"))
            {
                BuyState state = ShopManager.instance.buyState;
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
