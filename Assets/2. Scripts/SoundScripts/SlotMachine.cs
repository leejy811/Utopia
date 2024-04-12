using UnityEngine;
using System.Collections;

public class SlotMachine: MonoBehaviour
{
    public void Saemin()
    {
        StartCoroutine(RunEverySecondFor5Seconds());
    }

    IEnumerator RunEverySecondFor5Seconds()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 7f)
        {
            // 실행할 작업
            Debug.Log("매 초마다 실행됩니다.");
            AkSoundEngine .PostEvent("Play_Slot_rimshot", gameObject);
            // 1초 대기
            yield return new WaitForSeconds(1f);

            // 경과 시간 갱신
            elapsedTime += 1f;
        }
    }
}