using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static public UIManager instance;

    public GameObject optionPopUp;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SetOptionPopUp(bool active)
    {
        optionPopUp.SetActive(active);
    }

    public void SetBuildingPopUp(bool active, GameObject building = null)
    {
        //ToDo - 건물 클릭시 팝업 그룹 띄우고 그 안의 변수들 세팅
    }

    public void OnClickBuildingBuy(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuyBuilding, index);
    }

    public void OnClickBuildingSell()
    {
        ShopManager.instance.ChangeState(BuyState.SellBuilding);
    }

    public void OnClickTileBuy()
    {
        ShopManager.instance.ChangeState(BuyState.BuyTile);
    }

    public void OnClickTileBuild(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuildTile, index);
    }

    public void OnClickOptionBuy(int index)
    {
        ShopManager.instance.BuyOption((OptionType)index);
    }

    public void OnClickNextDay()
    {
        RoutineManager.instance.DailyUpdate();
    }
}
