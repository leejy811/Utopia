using System.Collections;
using UnityEngine;

public class SlotMachineManager : MonoBehaviour
{
    public GameObject slotContainer1;
    public GameObject slotContainer2;
    public GameObject slotContainer3;

    // isFirstFunction 변수를 추가합니다.
    private bool isFirstFunction = true;

    private IEnumerator SpinSlot(GameObject slotContainer, float reachedDistance)
    {
        float movedDistance = 0;

        while (movedDistance < reachedDistance * 25f)
        {
            foreach (Transform image in slotContainer.transform)
            {
                image.localPosition += Vector3.up * 5f;
            }

            movedDistance += 5f;

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void OnButtonClick()
    {
        if (isFirstFunction)
        {
            StartSlot1();
            StartSlot2();
            StartSlot3();
        }
        else
        {
            // 괄호를 추가하여 메서드 호출을 올바르게 마무리합니다.
            SpinReturnPos(slotContainer1, slotContainer2, slotContainer3, 1f, 2f, 3f);
        }


        isFirstFunction = !isFirstFunction;
    }



    private void SpinReturnPos(GameObject slotContainer1, GameObject slotContainer2, GameObject slotContainer3, float reachedDistance1, float reachedDistance2, float reachedDistance3)
    {

        foreach (Transform image in slotContainer1.transform)
        {
            image.localPosition -= Vector3.up * 25f * reachedDistance1;
        }

        foreach (Transform image in slotContainer2.transform)
        {
            image.localPosition -= Vector3.up * 25f * reachedDistance2;
        }

        foreach (Transform image in slotContainer3.transform)
        {
            image.localPosition -= Vector3.up * 25f * reachedDistance3;
        }
    }


    // 각각의 슬롯 컨테이너를 독립적으로 시작하는 메서드
    public void StartSlot1()
    {
        StartCoroutine(SpinSlot(slotContainer1, 1f));
    }

    public void StartSlot2()
    {
        StartCoroutine(SpinSlot(slotContainer2, 2f));
    }

    public void StartSlot3()
    {
        StartCoroutine(SpinSlot(slotContainer3, 3f));
    }



}