using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PleaseMoneyUI : DebtUI
{
    [Header("Mail")]
    public Animator mailAnim;
    public float mailAnimSecond;
    public float uiMoveSecond;

    protected override void SetValue()
    {
        base.SetValue();
        StartCoroutine(PlayMailAnim());
    }

    protected override void CalculateValue()
    {
        base.CalculateValue();
        curDay = curDay.AddDays(8 - dayOfWeek);
    }

    IEnumerator PlayMailAnim()
    {
        InputManager.canInput = false;
        transform.localPosition = new Vector3(transform.localPosition.x, 500.0f, transform.localPosition.z);

        mailAnim.gameObject.SetActive(true);
        mailAnim.SetFloat("MailSpeed", 1.0f / mailAnimSecond);
        mailAnim.SetInteger("MailNum", 1);

        yield return new WaitForSeconds(mailAnimSecond);
        mailAnim.gameObject.SetActive(false);

        transform.DOLocalMoveY(0f, uiMoveSecond).OnComplete(() => { InputManager.canInput = true; });
    }
}
