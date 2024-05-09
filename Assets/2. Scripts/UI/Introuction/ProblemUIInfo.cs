using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemUIInfo : BuildingEventUIInfo
{
    public SolutionUIInfo[] solutionUIInfos;

    public override void SetEventUIInfo(Event curEvent)
    {
        base.SetEventUIInfo(curEvent);

        for (int j = 0; j < solutionUIInfos.Length; j++)
        {
            solutionUIInfos[j].nameText.text = curEvent.solutions[j].name.ToString();
            solutionUIInfos[j].costText.text = "(-" + curEvent.solutions[j].cost.ToString() + "��)";
            solutionUIInfos[j].probText.text = "�ذ�Ȯ�� " + curEvent.solutions[j].prob.ToString() + "%";
        }
    }
}
