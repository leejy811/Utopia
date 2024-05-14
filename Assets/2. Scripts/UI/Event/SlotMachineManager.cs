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
    public float slot1Rand;
    public float slot2Rand;
    public float slot3Rand;

    private bool isFirstFunction = true;
    private System.Random random = new System.Random();


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

            bool shouldReset = false;
            foreach (Transform image in slotContainer.transform)
            {
                if (image.localPosition.y >= 2799)
                {
                    shouldReset = true;
                    break;
                }
            }

            if (shouldReset)
            {
                foreach (Transform image in slotContainer.transform)
                {
                    Vector3 pos = image.localPosition;
                    pos.y -= 2800;
                    image.localPosition = pos;
                }
            }

            movedDistance += spinSpeed * Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        Vector3 resetPosition = slotContainer.transform.localPosition;
        resetPosition.y = initialYPosition;
        slotContainer.transform.localPosition = resetPosition;
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

        slot1Rand = RandomRange(3, 5);
        slot2Rand = RandomRange(5, 7);
        slot3Rand = RandomRange(7, 9);

        foreach (Transform image in slotContainer1.transform)
        {
            image.localPosition += Vector3.up * returnPositionMultiplier *
                ((slot1Distance - slot1Rand) < 0 ? 40 - Mathf.Abs(slot1Distance - slot1Rand) : (slot1Distance - slot1Rand));
        }

        foreach (Transform image in slotContainer2.transform)
        {
            image.localPosition += Vector3.up * returnPositionMultiplier *
                ((slot2Distance - slot2Rand) < 0 ? 40 - Mathf.Abs(slot2Distance - slot2Rand) : (slot2Distance - slot2Rand));
        }

        foreach (Transform image in slotContainer3.transform)
        {
            image.localPosition += Vector3.up * returnPositionMultiplier *
                ((slot3Distance - slot3Rand) < 0 ? 40 - Mathf.Abs(slot3Distance - slot3Rand) : (slot3Distance - slot3Rand));
        }

        StartSlot1();
        StartSlot2();
        StartSlot3();
    }

    private float RandomRange(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
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
        StartCoroutine(SpinSlot(slotContainer1, slot1Rand));
    }

    public void StartSlot2()
    {
        StartCoroutine(SpinSlot(slotContainer2, slot2Rand));
    }

    public void StartSlot3()
    {
        StartCoroutine(SpinSlot(slotContainer3, slot3Rand));
    }
}
