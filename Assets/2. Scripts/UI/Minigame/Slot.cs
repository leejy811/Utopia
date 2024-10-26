using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Transform[] slots;
    public float slotDistance;
    public int curIdx;

    public IEnumerator RollSlot(int idx, float second)
    {
        do
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].DOLocalMoveY(slots[i].localPosition.y - slotDistance, second).SetEase(Ease.Linear);
            }
            curIdx--;
            if (curIdx < 0) curIdx = slots.Length - 1;
            yield return new WaitForSeconds(second);

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].localPosition.y < slotDistance * -3)
                    slots[i].localPosition = new Vector3(slots[i].localPosition.x, slotDistance, slots[i].localPosition.z);
            }
        } while (curIdx != idx);
    }
}
