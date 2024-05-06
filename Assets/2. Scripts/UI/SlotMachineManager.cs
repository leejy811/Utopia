using System.Collections;
using UnityEngine;

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

    private IEnumerator SpinSlot(GameObject slotContainer, float reachedDistance)
    {
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
    }

    public void OnButtonClick()
    {
        slot1Distance = Random.Range(1, 8); 
        slot2Distance = Random.Range(1, 8);
        slot3Distance = Random.Range(1, 8);

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