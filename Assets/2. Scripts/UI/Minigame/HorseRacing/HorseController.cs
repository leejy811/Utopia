using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct HorseInfo
{
    public HorseType horseType;
    public string horseName;
    public string typeName;
    public string skillDescription;
}

public enum HorseType { Solo, Chase, Sense, Sincere, Health }

public class HorseController : MonoBehaviour
{
    public HorseInfo horseInfo;
    public Vector2 speedRange;
    public float animationSpeedRatio;
    public int curGrade;
    public float curSpeed;

    [Header("Skill")]
    public float skillCooltime;
    public float skillDuration;
    public float skillParam;
    public int skillCondition;

    [Header("Injury")]
    [Range(0.0f, 1.0f)]public float injuryProb;
    public float injuryCooltime;
    public float injuryDuration;
    public float injuryParam;

    [Header("Effect")]
    public SpriteRenderer crownEffect;
    public SpriteRenderer skillEffect;
    public SpriteRenderer injuryEffect;
    public SpriteRenderer pickEffect;
    public float effectSecond;

    public Func<HorseType, int> getGrade;
    public Action<HorseType> applyResult;

    private bool isEnd;
    private Func<bool> checkSkill;
    private Func<bool> checkInjury;
    private Animator horseAnim;

    private void Start()
    {
        checkSkill += CheckSkillCondition;
        checkInjury += CheckInjuryCondition;
        horseAnim = gameObject.GetComponent<Animator>();
    }

    public void StartRace()
    {
        isEnd = false;
        StartCoroutine(CountEvent(skillCooltime, skillDuration, skillParam, checkSkill, skillEffect));
        StartCoroutine(CountEvent(injuryCooltime, injuryDuration, injuryParam, checkInjury, injuryEffect));
        StartCoroutine(MoveHorse());
    }

    public void DecideSpeed()
    {
        float ranSpeed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
        curSpeed = Mathf.Round(ranSpeed * 10) * 0.1f;
    }

    IEnumerator MoveHorse()
    {
        horseAnim.SetBool("IsMoving", true);
        while (!isEnd)
        {
            horseAnim.SetFloat("Speed", curSpeed / animationSpeedRatio);
            transform.position += Vector3.right * Time.fixedDeltaTime * curSpeed;
            yield return new WaitForFixedUpdate();
        }
        horseAnim.SetBool("IsMoving", false);
    }

    IEnumerator CountEvent(float coolTime, float duration, float param, Func<bool> checkCondition, SpriteRenderer effect)
    {
        yield return new WaitForSeconds(coolTime);
        while (!isEnd)
        {
            if (checkCondition.Invoke())
                StartCoroutine(ApplyEvent(duration, param, effect));
            yield return new WaitForSeconds(coolTime);
        }
    }

    IEnumerator ApplyEvent(float duration, float amount, SpriteRenderer effect)
    {
        curSpeed += amount;
        StartCoroutine(PlayEffect(effect));
        yield return new WaitForSeconds(duration);
        if (!isEnd)
            curSpeed -= amount;
    }

    IEnumerator PlayEffect(SpriteRenderer effect)
    {
        effect.gameObject.SetActive(true);
        yield return new WaitForSeconds(effectSecond);
        effect.gameObject.SetActive(false);
    }

    private bool CheckInjuryCondition()
    {
        float ranNum = UnityEngine.Random.Range(0.0f, 1.0f);
        if (ranNum < injuryProb) return true;
        else return false;
    }

    private bool CheckSkillCondition()
    {
        curGrade = getGrade.Invoke(horseInfo.horseType);
        if (curGrade == skillCondition || skillCondition == -1) return true;
        else return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Goal")
        {
            isEnd = true;
            applyResult.Invoke(horseInfo.horseType);
        }
    }
}
