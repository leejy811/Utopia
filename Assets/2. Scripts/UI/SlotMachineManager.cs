using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SlotMachineManager : MonoBehaviour
{
    public GameObject slotContainer1;
    public GameObject slotContainer2;
    public GameObject slotContainer3;

    public float spinSpeed = 25f;
    public float distanceMultiplier = 70f;
    public float returnPositionMultiplier = 70f;
    public float slot1Distance;
    public float slot2Distance;
    public float slot3Distance;

    private bool isFirstFunction = true;

    private List<Event> ranEvents;

    private IEnumerator SpinSlot(GameObject slotContainer, float reachedDistance)
    {
        float initialYPosition = slotContainer.transform.localPosition.y;

        float movedDistance = 0;
        while (movedDistance < reachedDistance * distanceMultiplier)
        {
            foreach (Transform image in slotContainer.transform)
            {
                image.localPosition += Vector3.up * spinSpeed * Time.fixedDeltaTime;
            }

            movedDistance += spinSpeed * Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Vector3 pos = slotContainer.transform.localPosition;
        pos.y = initialYPosition;
        slotContainer.transform.localPosition = pos;
    }

    public void OnButtonClick()
    {
        ProcessButtonClick(ranEvents);
    }

    public void SetEvent(List<Event> events)
    {
        ranEvents = events;
    }

    private void ProcessButtonClick(List<Event> events)
    {
        slot1Distance = events[0].eventIndex;
        slot2Distance = events[1].eventIndex;
        slot3Distance = events[2].eventIndex;

        SetInitialPosition(slotContainer1, slot1Distance);
        SetInitialPosition(slotContainer2, slot2Distance);
        SetInitialPosition(slotContainer3, slot3Distance);

        if (isFirstFunction)
        {
            StartSlot1();
            StartSlot2();
            StartSlot3();
        }
        else
        {
            SpinReturnPos(slotContainer1, slotContainer2, slotContainer3, slot1Distance, slot2Distance, slot3Distance);
        }

        isFirstFunction = !isFirstFunction;
    }


    private void SetInitialPosition(GameObject slotContainer, float distance)
    {
        float positionShift = 0;
        if (distance >= 11 && distance <= 20)
        {
            positionShift = 700f;
        }
        else if (distance >= 21 && distance <= 30)
        {
            positionShift = 1400f;
        }

        Vector3 newPosition = slotContainer.transform.localPosition;
        newPosition.y += positionShift;
        slotContainer.transform.localPosition = newPosition;
    }

    private void SpinReturnPos(GameObject slotContainer1, GameObject slotContainer2, GameObject slotContainer3, float reachedDistance1, float reachedDistance2, float reachedDistance3)
    {
        foreach (Transform image in slotContainer1.transform)
        {
            image.localPosition -= Vector3.up * returnPositionMultiplier * reachedDistance1;
        }

        foreach (Transform image in slotContainer2.transform)
        {
            image.localPosition -= Vector3.up * returnPositionMultiplier * reachedDistance2;
        }

        foreach (Transform image in slotContainer3.transform)
        {
            image.localPosition -= Vector3.up * returnPositionMultiplier * reachedDistance3;
        }
    }

    public void StartSlot1()
    {
        StartCoroutine(SpinSlot(slotContainer1, slot1Distance));
    }

    public void StartSlot2()
    {
        StartCoroutine(SpinSlot(slotContainer2, slot2Distance));
    }

    public void StartSlot3()
    {
        StartCoroutine(SpinSlot(slotContainer3, slot3Distance));
    }
}
