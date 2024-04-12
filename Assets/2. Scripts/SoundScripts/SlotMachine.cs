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
            // ������ �۾�
            Debug.Log("�� �ʸ��� ����˴ϴ�.");
            AkSoundEngine .PostEvent("Play_Slot_rimshot", gameObject);
            // 1�� ���
            yield return new WaitForSeconds(1f);

            // ��� �ð� ����
            elapsedTime += 1f;
        }
    }
}