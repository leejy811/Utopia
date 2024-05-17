using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileListUI : ListUI
{
    public Sprite unLockSprite;
    public override void SetValue(int type)
    {
        if (CityLevelManager.instance.levelIdx == -1)
        {
            for (int i = 0; i < Button.Length; i++)
            {
                ButtonImage[i].sprite = CityLevelManager.instance.levelIdx == -1 ? lockSprite : unLockSprite;
                Button[i].interactable = CityLevelManager.instance.levelIdx != -1;
            }
        }

        for(int i = 0;i < costText.Length; i++)
        {
            costText[i].text = Grid.instance.tileCost[i + 1].ToString("C");
        }
    }
}
