using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class HorseRacingUI : MinigameUI
{
    [Header("Betting")]
    public TextMeshProUGUI[] tabText;
    public TextMeshProUGUI secondText;
    public TextMeshProUGUI horseNameText;
    public TextMeshProUGUI horseSpeedText;
    public TextMeshProUGUI horseTypeText;
    public TextMeshProUGUI horseSkillText;
    public TextMeshProUGUI curBetText;
    public TextMeshProUGUI remainChipText;

    [Header("Result")]
    public TextMeshProUGUI[] gradeNameTexts;

    [Header("Parameter")]
    public int bettingSecond;
    public float errorMsgSecond;
    public float rewardRatio;

    [Header("System")]
    public GameObject gamePanel;
    public HorseRacingCamera mainCamera;
    public List<HorseController> horses;

    private List<HorseInfo> resultInfo = new List<HorseInfo>();
    private HorseType curHorseType;
    private int curBetChip;
    private Coroutine timeCountCoroutine;

    private void Start()
    {
        float speedAvg = 0.0f;
        foreach (HorseController horse in horses)
        {
            horse.getGrade += GetGrade;
            horse.applyResult += GoalInHorse;
            speedAvg += (horse.speedRange.x + horse.speedRange.y) / 2.0f;
        }
        speedAvg /= horses.Count;

        foreach (HorseController horse in horses)
        {
            horse.animationSpeedRatio = speedAvg;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        gamePanel.SetActive(false);
    }

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
        gamePanel.SetActive(true);
    }

    protected override void SetValue()
    {
        base.SetValue();
    }

    protected override void SetUI(MinigameState state)
    {
        switch (state)
        {
            case MinigameState.Lobby:
                base.SetValue();
                InitHorses();
                break;
            case MinigameState.Betting:
                curBetChip = curGameBuilding.betChip;
                SetHorseSpeed();
                OnClickPickHorse(0);
                SetBetChipUI();
                timeCountCoroutine = StartCoroutine(CountTime());
                break;
            case MinigameState.Result:
                SetGradeName();
                break;
        }
    }

    private void InitHorses()
    {
        mainCamera.targetHorse = null;
        foreach (HorseController horse in horses)
        {
            horse.transform.localPosition = new Vector3(0.0f, horse.transform.localPosition.y, horse.transform.localPosition.z);
        }
    }

    private void SetHorseSpeed()
    {
        foreach (HorseController horse in horses)
        {
            horse.DecideSpeed();
        }
    }

    private void SetHorseInfo(HorseController horse)
    {
        horseNameText.text = horse.horseInfo.horseName;
        horseSpeedText.text = horse.curSpeed.ToString("F1");
        horseTypeText.text = horse.horseInfo.typeName;
        horseSkillText.text = horse.horseInfo.skillDescription;
    }

    private void SetHorseTab(int index)
    {
        for(int i = 0;i < tabText.Length;i++)
        {
            tabText[i].color = i == index ? Color.white : Color.gray;
        }
    }

    private void SetBetChipUI()
    {
        curBetText.text = curBetChip.ToString();
        remainChipText.text = "보유 칩 : " + (ChipManager.instance.curChip - curBetChip).ToString();
    }

    private void SetGradeName()
    {
        for (int i = 0;i < gradeNameTexts.Length;i++)
        {
            gradeNameTexts[i].text = resultInfo[i].horseName;
        }
    }

    private void GetReward()
    {
        ChipManager.instance.curChip += (int)Mathf.Round(curBetChip * rewardRatio);
    }

    IEnumerator CountTime()
    {
        int count = bettingSecond;
        while (count >= 0)
        {
            secondText.text = count.ToString();
            count--;
            yield return new WaitForSeconds(1f);
        }

        OnClickStartRace();
        secondText.text = bettingSecond.ToString();
    }

    public void OnClickStartRace()
    {
        if (timeCountCoroutine != null)
            StopCoroutine(timeCountCoroutine);

        ChipManager.instance.PayChip(curBetChip);
        resultInfo.Clear();
        SetState(MinigameState.Play);
        foreach (HorseController horse in horses)
        {
            horse.StartRace();
        }
    }

    public void OnClickPickHorse(int type)
    {
        HorseController horse = FindHorse((HorseType)type);
        curHorseType = (HorseType)type;
        mainCamera.targetHorse = horse.transform;
        SetHorseInfo(horse);
        SetHorseTab(type);
    }
    
    public void OnClickChangeState(int state)
    {
        SetState((MinigameState)state);
    }

    public void OnClickBetChip(int amount)
    {
        if (curBetChip + amount < curGameBuilding.betChip)
        {
            Debug.Log("기본 배팅 금액보다 적음");
            //Error Msg
            return;
        }
        else if (curBetChip + amount > ChipManager.instance.curChip)
        {
            Debug.Log("보유 금액 초과");
            //Error Msg
            return;
        }

        curBetChip += amount;
        SetBetChipUI();
    }

    private void GoalInHorse(HorseType horseType)
    {
        resultInfo.Add(FindHorse(horseType).horseInfo);

        if (resultInfo.Count == horses.Count)
            SetState(MinigameState.Result);
        else if (resultInfo.Count == 1 && horseType == curHorseType)
            GetReward();
    }

    private int GetGrade(HorseType horseType)
    {
        horses.Sort(CompareGrade);
        return horses.FindIndex(x => x.horseType == horseType);
    }

    private int CompareGrade(HorseController a, HorseController b)
    {
        return a.transform.position.x > b.transform.position.x ? -1 : 1;
    }

    private HorseController FindHorse(HorseType horseType)
    {
        return horses.Find(x => x.horseType == horseType);
    }
}
