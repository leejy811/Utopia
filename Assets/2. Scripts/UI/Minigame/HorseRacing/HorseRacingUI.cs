using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HorseRacingUI : MinigameUI
{
    [Header("Error")]
    public Image chipErrorImage;
    public Image timesErrorImage;

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
    public Image[] gradePickImages;

    [Header("Parameter")]
    public int bettingSecond;
    public float errorMsgSecond;
    public float rewardRatio;
    public Vector2 horseYRange;

    [Header("CountDown")]
    public Image countImage;
    public Sprite[] countSprites;

    [Header("Bar")]
    public Image[] horsePinImages;
    public Sprite pickHorse;
    public Sprite otherHorse;
    public Transform goalPoint;
    public float pickPinScale;
    public float otherPinScale;

    [Header("System")]
    public GameObject horsePanel;
    public HorseRacingCamera mainCamera;
    public List<HorseController> horses;

    private List<HorseInfo> resultInfo = new List<HorseInfo>();
    private HorseType curHorseType;
    private int curBetChip;
    private Coroutine timeCountCoroutine;
    private HorseController prevPickHorse;

    private void Start()
    {
        float speedAvg = 0.0f;
        foreach (HorseController horse in horses)
        {
            horse.getGrade += GetGrade;
            horse.applyResult += GoalInHorse;
            horse.setHorsePin += SetHorsePinPos;
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
    }

    public override void InitGame(EnterBuilding building)
    {
        base.InitGame(building);
    }

    protected override void SetGamePanel(bool active)
    {
        base.SetGamePanel(active);
        horsePanel.SetActive(active);

        string isPlay = active ? "Play" : "Stop";
        AkSoundEngine.PostEvent(isPlay + "_RACE_active", gameObject);
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
            float yPos = horse.transform.localPosition.y;
            float zPos = horse.transform.localPosition.z;
            if (resultInfo.Count != 0)
            {
                int grade = resultInfo.FindIndex(x => x.horseType == horse.horseInfo.horseType);
                int index = resultInfo.Count - grade - 1;
                yPos = horseYRange.x + (horseYRange.y - horseYRange.x) / (resultInfo.Count - 1) * index;
                zPos = index;
            }
            horse.transform.localPosition = new Vector3(0.0f, yPos, zPos);
            horse.crownEffect.gameObject.SetActive(false);
        }

        if(prevPickHorse != null)
            prevPickHorse.pickEffect.gameObject.SetActive(false);
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

        if (prevPickHorse != null)
            prevPickHorse.pickEffect.gameObject.SetActive(false);
        horse.pickEffect.gameObject.SetActive(true);
        prevPickHorse = horse;
    }

    private void SetHorseTab(int index)
    {
        for(int i = 0;i < tabText.Length;i++)
        {
            tabText[i].color = i == index ? Color.white : Color.gray;
        }
    }

    private void SetHorsePickImage(int index)
    {
        for (int i = 0; i < horsePinImages.Length; i++)
        {
            horsePinImages[i].sprite = i == index ? pickHorse : otherHorse;
            horsePinImages[i].transform.localScale = Vector3.one * (i == index ? pickPinScale : otherPinScale);
        }

        horsePinImages[index].transform.SetAsLastSibling();
    }

    private void SetBetChipUI()
    {
        curBetText.text = curBetChip.ToString();
        remainChipText.text = "º¸À¯ Ä¨ : " + (ChipManager.instance.curChip - curBetChip).ToString();
    }

    private void SetGradeName()
    {
        for (int i = 0;i < gradeNameTexts.Length;i++)
        {
            gradeNameTexts[i].text = resultInfo[i].horseName;
        }
    }

    private void SetPickGrade(int grade)
    {
        for (int i = 0; i < gradePickImages.Length; i++)
        {
            gradePickImages[i].enabled = i == grade - 1;
        }
    }

    private void GetReward()
    {
        ChipManager.instance.curChip += (int)Mathf.Round(curBetChip * rewardRatio);
    }

    IEnumerator OnError(Image errorImage)
    {
        errorImage.enabled = true;
        yield return new WaitForSeconds(errorMsgSecond);
        errorImage.enabled = false;
    }

    IEnumerator CountDown()
    {
        countImage.gameObject.SetActive(true);
        for (int i = 0;i < countSprites.Length;i++)
        {
            countImage.sprite = countSprites[i];
            AkSoundEngine.PostEvent("Play_RACE_countdown", gameObject);
            yield return new WaitForSeconds(1);
        }
        countImage.gameObject.SetActive(false);

        foreach (HorseController horse in horses)
        {
            horse.StartRace();
        }
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
        StartCoroutine(CountDown());
    }

    public void OnClickPickHorse(int index)
    {
        HorseType type = resultInfo.Count == 0 ? (HorseType)index : resultInfo[index].horseType;
        HorseController horse = FindHorse(type);
        curHorseType = type;
        mainCamera.targetHorse = horse.transform;
        SetHorseInfo(horse);
        SetHorseTab(index);
        SetHorsePickImage((int)type);
    }
    
    public void OnClickChangeState(int state)
    {
        SetState((MinigameState)state);
    }

    public void OnClickStartBetting()
    {
        if (ChipManager.instance.curChip < curGameBuilding.betChip)
            StartCoroutine(OnError(chipErrorImage));
        else if (curGameBuilding.betTimes == 0)
            StartCoroutine(OnError(timesErrorImage));
        else
            SetState(MinigameState.Betting);
    }

    public void OnClickBetChip(int amount)
    {
        if (curBetChip + amount < curGameBuilding.betChip)
        {
            return;
        }
        else if (curBetChip + amount > ChipManager.instance.curChip)
        {
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
        else if (resultInfo.Count == 1)
        {
            FindHorse(horseType).crownEffect.gameObject.SetActive(true);
            if (horseType == curHorseType)
                GetReward();
        }

        if (horseType == curHorseType)
            SetPickGrade(resultInfo.Count);
    }

    private void SetHorsePinPos(int type, float xPos)
    {
        float ratio = xPos / (mainCamera.mapWidth * 2.0f);
        float xRectPos = goalPoint.localPosition.x * ratio;

        Vector3 prevPos = horsePinImages[type].transform.localPosition;
        horsePinImages[type].transform.localPosition = new Vector3(xRectPos, prevPos.y, prevPos.z);
    }

    private int GetGrade(HorseType horseType)
    {
        horses.Sort(CompareGrade);
        return horses.FindIndex(x => x.horseInfo.horseType == horseType);
    }

    private int CompareGrade(HorseController a, HorseController b)
    {
        return a.transform.position.x > b.transform.position.x ? -1 : 1;
    }

    private HorseController FindHorse(HorseType horseType)
    {
        return horses.Find(x => x.horseInfo.horseType == horseType);
    }
}
