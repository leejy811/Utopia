using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyTextUI : MonoBehaviour
{
    private TextMeshProUGUI moneyText;

    private void Start()
    {
        moneyText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator CalculateMoney(int prevMoney, int nextMoney, float second)
    {
        float elapsedTime = 0;
        int currentNumber = prevMoney;
        int direction = (nextMoney > prevMoney) ? 1 : -1;
        int totalSteps = Mathf.Abs(nextMoney - prevMoney);
        moneyText.text = string.Format("{0:#,###}", currentNumber) + "¿ø";
        moneyText.color = Color.yellow;

        AkSoundEngine.PostEvent("Play_Paythepayment_01", gameObject);
        while (elapsedTime < second)
        {
            float t = elapsedTime / second;
            currentNumber = prevMoney + Mathf.RoundToInt(t * totalSteps) * direction;
            moneyText.text = string.Format("{0:#,###}", currentNumber) + "¿ø";

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        moneyText.text = string.Format("{0:#,###}", nextMoney) + "¿ø";
        moneyText.color = Color.white;
    }
}
