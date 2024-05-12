using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    static public UIManager instance;

    #region UIComponent

    public Canvas canvas;

    [Header("Info")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI happinessText;
    public TextMeshProUGUI cityResidentText;
    public TextMeshProUGUI NewsMessage;
    public TextMeshProUGUI NewsMessage2;

    [Header("Building")]
    public BuildingIntroUI[] buildingIntros;
    public InfoUI[] infos;
    public ListUI[] lists;

    [Header("Tile")]
    public TileInfluenceUI tileInfluenceInfo;

    [Header("Statistic")]
    public StatisticUI statistic;

    [Header("CityLevel")]
    public GameObject cityLevelPanel;
    public CityLevelUI[] cityLevels;

    [Header("CityLevelUp")]
    public GameObject cityLevelUpPanel;
    public GameObject[] cityLevelUps;

    [Header("Roulette")]
    public EventRouletteUI eventRoulette;

    [Header("Message")]
    public GameObject errorMessagePrefab;
    public GameObject happinessMessagePrefab;

    [Header("Cost")]
    public CostUI costInfo;

    [Header("Event Notify")]
    public EventNotifyUI eventNotify;

    [Header("Buttons")]
    public Button[] buttons;
    public bool isButtonLock;

    [Header("Func")]
    public UIElement construct;
    public UIElement etcFunc;

    #endregion


    private Building targetBuilding;

    public int NewsHappiness;
    private int previousHappiness;
    private bool UpdateNews = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public RectTransform imageRectTransform;
    public RectTransform imageRectTransform2;
    public TextMeshProUGUI textComponent;


    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        UpdateDailyInfo();
        NewsHappiness = (int)RoutineManager.instance.cityHappiness;
        previousHappiness = NewsHappiness;

    }

    #region NewsMessage

    void Update()
    {
        //NewsHappiness = (int)RoutineManager.instance.cityHappiness;

        //if (NewsHappiness != 0 && UpdateNews == false)
        //{
        //    previousHappiness = NewsHappiness;
        //    UpdateNews = true;
        //}

        //if (previousHappiness != NewsHappiness && UpdateNews == true)
        //{
        //    StartCoroutine(HappinessBasedNews());
        //}
    }


    IEnumerator HappinessBasedNews()
    {
        if (previousHappiness > 20 && NewsHappiness <= 20)
        {
            NewsMessage.text = "�� ���ð� �ູ�ϴٴ� ������ �˱��?";
            yield return new WaitForSeconds(2);
            NewsMessage.text = "�޽��� ���� �� �޽���";
            previousHappiness = NewsHappiness;
        }
        else if (previousHappiness < 20 && NewsHappiness >= 20)
        {
            NewsMessage.text = "������ ��Ȱ����";
            yield return new WaitForSeconds(2);
            NewsMessage.text = "�� ���� �ٸ� ���̶� �ٸ��� ������...��� �һ�.";
            previousHappiness = NewsHappiness;
        }
        else if (previousHappiness > 40 && NewsHappiness <= 40)
        {

            NewsMessage.text = "�� ���� ���̽��ϴ�...";
            yield return new WaitForSeconds(2);

            float animationDuration = 3f;
            NewsMessage2.text = "�޽��� ���� ��sssssssssssssssssssssssssssssssss �޽���";
            yield return new WaitForSeconds(1);
            TweeningObject tweeningobject = GetComponent<TweeningObject>();
            tweeningobject.MatchSizes();
            yield return new WaitForSeconds(animationDuration);
            NewsMessage.text = "�޽��� ���� ��sssssssssssssssssssssssssssssssss �޽���";
            previousHappiness = NewsHappiness;
        }
        else if (previousHappiness < 40 && NewsHappiness >= 40)
        {
            NewsMessage.text = "���� �������� ���� ������ ���� �ʾҴ�.";
            yield return new WaitForSeconds(2);
            NewsMessage.text = "����� ���ö�Ʈ �ɾ�帱�Կ�";
            previousHappiness = NewsHappiness;
        }
        else if (previousHappiness > 60 && NewsHappiness <= 60)
        {
            NewsMessage.text = "����� �ù��� ���ΰŴ�?";
            yield return new WaitForSeconds(2);
            NewsMessage.text = "������ �ߵǴ� ��ȸ�� �������";
            previousHappiness = NewsHappiness;
        }
        else if (previousHappiness < 60 && NewsHappiness >= 60)
        {
            NewsMessage.text = "�̰� ���� �츮 ����� �̱�� �ߴ�.";
            yield return new WaitForSeconds(2);
            NewsMessage.text = "���ݺ��� ����԰� ���� �� ������ �����Ѵ�";
            previousHappiness = NewsHappiness;
        }
        else if (previousHappiness > 80 && NewsHappiness <= 80)
        {
            NewsMessage.text = "�̰��� �����Ǿƶ�°� ������ �м�";
            yield return new WaitForSeconds(2);
            NewsMessage.text = "�� ���� �ƽ������~";
            previousHappiness = NewsHappiness;
        }
        else if (previousHappiness < 80 && NewsHappiness >= 80)
        {
            NewsMessage.text = "���� ����� �ְ�...GOAT";
            yield return new WaitForSeconds(2);
            NewsMessage.text = "�̰��� �����Ǿƶ�°� �а��� ����";
            previousHappiness = NewsHappiness;
        }
    }

    Vector2 CalculateTextSize(string text)
    {
        // �ӽ÷� �ؽ�Ʈ�� ����
        textComponent.text = text;
        // ������ ũ�⸦ ��ȯ (�� ���ÿ����� �����ϰ� ���� ũ�⸸ ����)
        return new Vector2(textComponent.preferredWidth + 20, imageRectTransform.sizeDelta.y);
    }

    #endregion



    #region SetValue

    public void LockButtons(bool active)
    {
        isButtonLock = active;

        foreach (Button button in buttons)
        {
            button.interactable = active;
        }

        if (active)
        {
            SetCityLevelPopUp(true);
            ShopManager.instance.ChangeState(BuyState.None);
        }
    }

    private string GetCommaText(int data)
    {
        if (data == 0)
            return data.ToString();
        else
            return string.Format("{0:#,###}", data);
    }

    public void UpdateDailyInfo()
    {
        DateTime curDay = RoutineManager.instance.day;
        dayText.text = curDay.ToString("yy") + "/" + curDay.ToString("MM") + "/" + curDay.ToString("dd");
        SetHappiness();
        Setmoney();
        SetCityResident();
    }

    public void SetHappiness()
    {
        happinessText.text = ((int)RoutineManager.instance.cityHappiness).ToString() + "%";
    }

    public void Setmoney()
    {
        moneyText.text = GetCommaText(ShopManager.instance.Money);
    }

    public void SetCityResident()
    {
        cityResidentText.text = GetCommaText(ResidentialBuilding.cityResident);
    }

    public void SetBuildingIntroValue()
    {
        int idx = GetBuildingIndex();
        buildingIntros[idx].SetValue(targetBuilding);
    }

    public void SetEventNotifyValue(Building building)
    {
        eventNotify.SetValue(building);
    }

    public void SetBuildingListValue(int index)
    {
        lists[index].SetValue(index);
    }

    public void SetTileInfluenceValue(Tile tile)
    {
        tileInfluenceInfo.SetValue(tile);
    }

    #endregion

    #region PopUp

    public void SetRoulettePopUp(bool active, List<Event> ranEvents = null)
    {
        eventRoulette.gameObject.SetActive(active);

        if (active)
            eventRoulette.SetValue(ranEvents);
    }

    public void SetInfoPopUp(int typeIndex, int index)
    {
        infos[typeIndex].SetValue(index);
    }

    public void SetBuildingIntroPopUp(Building building = null)
    {
        //if(eventNotify.gameObject.activeSelf)
        //    SetEventNotifyValue(building);

        if (building == null)
        {
            foreach (BuildingIntroUI introUI in buildingIntros)
                introUI.gameObject.SetActive(false);
        }
        else
        {
            targetBuilding = building;
            int idx = GetBuildingIndex();

            for (int i = 0; i < buildingIntros.Length; i++)
            {
                if (i == idx)
                {
                    buildingIntros[i].gameObject.SetActive(true);
                    buildingIntros[i].SetValue(building);

                }
                else
                    buildingIntros[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetEventNotifyPopUp(bool active)
    {
        eventNotify.gameObject.SetActive(active);

        if (active)
            eventNotify.Init();
    }

    public void SetTileInfluencePopUp(Tile tile = null)
    {
        if (tile == null)
            tileInfluenceInfo.gameObject.SetActive(false);
        else
        {
            tileInfluenceInfo.gameObject.SetActive(true);
            tileInfluenceInfo.SetValue(tile);
        }
    }

    public void SetStatisticPopUp(bool active)
    {
        statistic.gameObject.SetActive(active);
        statistic.SetValue();
    }

    public void SetErrorPopUp(string massage, Vector3 position)
    {
        TemporayUI message = Instantiate(errorMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(massage, position);
    }

    public void SetHappinessPopUp(int amount, Vector3 position)
    {
        string massage = amount < 0 ? "<sprite=1> <sprite=5>" : "<sprite=3> <sprite=6>";

        TemporayUI message = Instantiate(happinessMessagePrefab, canvas.transform).GetComponent<TemporayUI>();
        message.SetUI(massage, position);
    }

    public void SetCostPopUp(int cost, Vector3 position)
    {
        //costInfo.OnUI(cost, position);
    }

    public void SetCityLevelPopUp(bool active)
    {
        cityLevelPanel.SetActive(active);

        if (active)
        {
            foreach(CityLevelUI cityLevel in cityLevels)
            {
                cityLevel.SetValue();
            }
        }
    }

    public void SetCityLevelUpPopUp(bool active, int index = 0)
    {
        cityLevelUpPanel.SetActive(active);

        if (active)
        {
            for (int i = 0;i < cityLevelUps.Length; i++)
            {
                if(i == index)
                    cityLevelUps[i].SetActive(true);
                else
                    cityLevelUps[i].SetActive(false);
            }
        }
    }

    public void SetAllPopUp()
    {
        if (ShopManager.instance.buyState == BuyState.BuyTile)
            ShopManager.instance.SetTargetObject(null, Color.green, Color.red);
        else if (ShopManager.instance.buyState == BuyState.SellBuilding)
            ShopManager.instance.SetTargetObject(null, Color.red, Color.white);

        ShopManager.instance.ChangeState(BuyState.None);
        //SetCityLevelPopUp(false);

        //if (Grid.instance.isInfluenceMode)
        //    Grid.instance.SetTileInfluenceMode();

        //if (Grid.instance.isColorMode)
        //    Grid.instance.SetTileColorMode();

        //if (BuildingSpawner.instance.isHighlightMode)
        //    BuildingSpawner.instance.EventBuildingsHighlight();
    }

    #endregion

    #region OnClick

    public void OnClickConstructButton()
    {
        construct.gameObject.SetActive(!construct.gameObject.activeSelf);
    }

    public void OnClickEtcFuncButton()
    {
        etcFunc.gameObject.SetActive(!etcFunc.gameObject.activeSelf);
    }

    public void OnClickBuyPopUp(int index)
    {
        for (int i = 0; i < lists.Length; i++)
        {
            if (i == index)
            {
                lists[i].gameObject.SetActive(!lists[i].gameObject.activeSelf);
                lists[i].SetValue(i);
            }
            else
                lists[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < infos.Length; i++)
            infos[i].gameObject.SetActive(false);
    }

    public void OnClickTileBuild(int index)
    {
        ShopManager.instance.ChangeState(BuyState.BuildTile, index);
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

    public void OnClickOptionBuy(int index)
    {
        if (ShopManager.instance.BuyOption((OptionType)index))
        {
            if (ShopManager.instance.buyState == BuyState.SolveBuilding)
            {
                int idx = GetBuildingIndex();
                buildingIntros[idx].SetValue(targetBuilding);
            }
            else
                eventNotify.SetValue(EventManager.instance.eventBuildings[eventNotify.curIndex]);
        }
    }

    public void OnClickNextDay()
    {
        if (eventRoulette.gameObject.activeSelf) return;

        SetAllPopUp();
        RoutineManager.instance.DailyUpdate();
        SetStatisticPopUp(true);
    }

    public void OnClickCloseStatistic()
    {
        SetStatisticPopUp(false);
        RoutineManager.instance.UpdateAfterStat();
        UpdateDailyInfo();
    }

    public void OnClickCloseEventRoulette()
    {
        SetRoulettePopUp(false);
    }

    public void OnClickSolveEvent(int index)
    {
        ShopManager.instance.SolveEvent(index);
    }

    public void OnClickTileColorMode()
    {
        Grid.instance.SetTileColorMode();
    }

    public void OnClickTileInfluenceMode()
    {
        Grid.instance.SetTileInfluenceMode();
    }

    public void OnClickCityLevelMode()
    {
        SetCityLevelPopUp(!cityLevelPanel.activeSelf);
    }

    public void OnClickEventHighLight()
    {
        BuildingSpawner.instance.EventBuildingsHighlight();
    }

    public void OnClickEventNotify()
    {
        if (BuildingSpawner.instance.GetEventBuildingCount() <= 0) return;

        ShopManager.instance.ChangeState(BuyState.EventCheck);
    }

    public void OnClickEventNotifyNext(bool isRight)
    {
        if (EventManager.instance.eventBuildings.Count == 0)
            return;

        eventNotify.NextBuilding(isRight);
    }

    #endregion

    private int GetBuildingIndex()
    {
        int idx = targetBuilding.type == BuildingType.Residential ? 0 : 1;
        return idx;
    }
}