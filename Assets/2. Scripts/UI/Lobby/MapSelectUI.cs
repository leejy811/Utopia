using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MapSelectUI : MonoBehaviour
{
    public RectTransform targetRect;
    public VideoPlayer videoPlayer;
    public Image[] inActiveImage;
    public VideoClip[] mapVideos;
    public LobbyUIManager uiManager;

    public float slideSecond;

    private int curIdx;

    public void OnClickNextMap(int idx)
    {
        curIdx = (curIdx + idx + inActiveImage.Length) % inActiveImage.Length;

        targetRect.DOLocalMoveX(curIdx * -1920f, slideSecond);

        for (int i = 0;i < inActiveImage.Length;i++)
        {
            inActiveImage[i].enabled = i != curIdx;
        }

        videoPlayer.clip = mapVideos[curIdx];
        uiManager.SetMainColor(curIdx);
    }
}
