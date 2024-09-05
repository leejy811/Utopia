using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDayUI : MonoBehaviour
{
    public Image panel;
    public Image newImage;
    public TextMeshProUGUI title;
    public TextMeshProUGUI data;

    private void OnEnable()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -85f, transform.localPosition.z);
        transform.DOLocalMoveY(-115f, 1f);

        panel.color -= new Color(0, 0, 0, 1);
        title.color -= new Color(0, 0, 0, 1);
        data.color -= new Color(0, 0, 0, 1);

        panel.DOFade(1.0f, 1f).OnComplete(() => { panel.DOFade(0.0f, 1f); });
        newImage.DOFade(1.0f, 1f).OnComplete(() => { newImage.DOFade(0.0f, 1f); });
        title.DOFade(1.0f, 1f).OnComplete(() => { title.DOFade(0.0f, 1f); });
        data.DOFade(1.0f, 1f).OnComplete(() => { data.DOFade(0.0f, 1f).OnComplete(() => { gameObject.SetActive(false); }); });

        AkSoundEngine.PostEvent("Play_D_Day_popup_001", gameObject);
    }
}
