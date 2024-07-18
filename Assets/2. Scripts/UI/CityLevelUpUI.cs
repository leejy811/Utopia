using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CityLevelUpUI : MonoBehaviour, IObserver
{
    public Animator mailAnim;
    public LevelMailUI[] levelMails;
    public float mailAnimSecond;

    IEnumerator PlayLevelUp()
    {
        int idx = CityLevelManager.instance.levelIdx;
        mailAnim.gameObject.SetActive(true);
        mailAnim.SetFloat("MailSpeed", 1.0f / mailAnimSecond);
        mailAnim.SetInteger("MailNum", levelMails[idx].mails.Length);

        yield return new WaitForSeconds(mailAnimSecond);
        mailAnim.gameObject.SetActive(false);

        levelMails[idx].gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        foreach(LevelMailUI mail in levelMails)
        {
            mail.gameObject.SetActive(false);
        }
    }

    public void Notify(EventState state)
    {
        if (state == EventState.CityLevelUp)
        {
            gameObject.SetActive(true);
            StartCoroutine(PlayLevelUp());
        }
        else
            gameObject.SetActive(false);
    }
}
