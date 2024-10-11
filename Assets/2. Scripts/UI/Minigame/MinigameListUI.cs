using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinigameListUI : MonoBehaviour
{
    public Transform listParent;
    public GameObject gameInfoPrefab;

    private List<GameInfoUI> gameInfos = new List<GameInfoUI>();
    private List<EnterBuilding> sortBuilding = new List<EnterBuilding>();
    private bool nameGreater;
    private bool chipGreater;
    private bool timesGreater;

    private void OnEnable()
    {
        nameGreater = true;
        gameObject.GetComponentInChildren<Scrollbar>().value = 1.0f;
        SetInfo();
        OnClickNameSort();
    }

    private void SortBuilding(Comparison<EnterBuilding> comparison)
    {
        List<EnterBuilding> buildings = BuildingSpawner.instance.gameBuildings;
        buildings.Sort(comparison);
        sortBuilding = buildings;

        SetInfo();
    }

    private void SetInfo()
    {
        for(int i = 0;i < sortBuilding.Count; i++)
        {
            if (i >= gameInfos.Count)
                AddInfo(sortBuilding[i]);
            else
                gameInfos[i].SetValue(sortBuilding[i]);
        }

        if (gameInfos.Count > sortBuilding.Count)
        {
            for (int i = 0; i < gameInfos.Count; i++)
            {
                if(i >= sortBuilding.Count)
                    gameInfos[i].gameObject.SetActive(false);
            }
        }
    }

    private void AddInfo(EnterBuilding building)
    {
        GameInfoUI gameInfo = Instantiate(gameInfoPrefab, listParent).GetComponent<GameInfoUI>();
        gameInfo.SetValue(building);
        gameInfos.Add(gameInfo);
    }

    #region Compare
    private int CompareBetChipGreater(EnterBuilding a, EnterBuilding b)
    {
        return a.betChip < b.betChip ? -1 : 1;
    }

    private int CompareBetChipLesser(EnterBuilding a, EnterBuilding b)
    {
        return a.betChip > b.betChip ? -1 : 1;
    }

    private int CompareBetTimesGreater(EnterBuilding a, EnterBuilding b)
    {
        return a.betTimes < b.betTimes ? -1 : 1;
    }

    private int CompareBetTimesLesser(EnterBuilding a, EnterBuilding b)
    {
        return a.betTimes > b.betTimes ? -1 : 1;
    }

    private int CompareNameGreater(EnterBuilding a, EnterBuilding b)
    {
        if (a.grade < b.grade) return -1;
        else if (a.grade > b.grade) return 1;
        else return a.count < b.count ? -1 : 1;
    }

    private int CompareNameLesser(EnterBuilding a, EnterBuilding b)
    {
        if (a.grade < b.grade) return 1;
        else if (a.grade > b.grade) return -1;
        else return a.count > b.count ? -1 : 1;
    }
    #endregion

    #region OnClick
    public void OnClickNameSort()
    {
        SortBuilding(nameGreater ? CompareNameGreater : CompareNameLesser);
        nameGreater = !nameGreater;
    }

    public void OnClickChipSort()
    {
        SortBuilding(chipGreater ? CompareBetChipGreater : CompareBetChipLesser);
        chipGreater = !chipGreater;
    }

    public void OnClickTimesSort()
    {
        SortBuilding(timesGreater ? CompareBetTimesGreater : CompareBetTimesLesser);
        timesGreater = !timesGreater;
    }
    #endregion
}