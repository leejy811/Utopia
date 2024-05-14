using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileListUI : ListUI
{
    public Sprite unLockSprite;
    public override void SetValue(int type)
    {
        for(int i = 0;i < Button.Length;i++)
        {
            ButtonImage[i].sprite = CityLevelManager.instance.levelIdx == -1 ? lockSprite : unLockSprite;
            Button[i].interactable = CityLevelManager.instance.levelIdx != -1;
        }

        //ToDo Tile 건설 구현 후 작성
    }
}
