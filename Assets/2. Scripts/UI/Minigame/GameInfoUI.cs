using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameInfoUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI betChipText;
    public TextMeshProUGUI betTimesText;

    private EnterBuilding building;

    public void SetValue(EnterBuilding enterBuilding)
    {
        building = enterBuilding;

        nameText.text = building.buildingName + building.count.ToString();
        betChipText.text = ((int)building.values[ValueType.betChip].cur).ToString();
        betTimesText.text = building.betTimes.ToString();

        gameObject.SetActive(true);
    }

    public void OnClickPlayGame()
    {
        UIManager.instance.minigames[(int)building.minigameType].InitGame(building);
        UIManager.instance.notifyObserver(EventState.Minigame);
    }
}
