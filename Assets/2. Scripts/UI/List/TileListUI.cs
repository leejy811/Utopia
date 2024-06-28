using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileListUI : ListUI
{
    public Sprite unLockSprite;
    public override void SetValue(int type)
    {
        for (int i = 0; i < Button.Length; i++)
        {
            bool checkGrade = CityLevelManager.instance.levelIdx == CityLevelManager.instance.level.Length - 1;
            ButtonImage[i].sprite = !checkGrade ? lockSprite : unLockSprite;
            Button[i].interactable = checkGrade;
        }

        for(int i = 0;i < costText.Length; i++)
        {
            costText[i].text = Grid.instance.tileCost[i + 1].ToString("C");
        }
    }
}
