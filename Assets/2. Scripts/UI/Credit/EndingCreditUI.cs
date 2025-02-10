using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;


public class EndingCreditUI : MonoBehaviour
{
    public Vector2 distance;

    public Transform creditParent;
    public GameObject creditPrefab;

    public CreditDB creditDB;

    private void Start()
    {
        //InitCredit();
    }

    private void InitCredit()
    {
        creditDB.entities.Sort(ComparePriceLesser);

        for (int i = 0;i < creditDB.entities.Count; i++)
        {
            int col = (i % 3) - 1;
            int row = i / 3;

            Vector3 pos = creditParent.localPosition;
            pos += new Vector3(col * distance.x, row * distance.y, 0);

            GameObject credit = Instantiate(creditPrefab, transform);
            credit.transform.localPosition = pos;
            credit.GetComponent<CreditEntityUI>().SetValue(creditDB.entities[i]);
        }

        float moveDist = (creditParent.localPosition.y + ((creditDB.entities.Count / 3) + 3) * distance.y) * -1;
        transform.DOLocalMoveY(transform.localPosition.y + moveDist, 90.0f);
    }

    public void StartCredit()
    {
        InitCredit();
        StartCoroutine(CityLevelManager.instance.PlayEndingLapse());
    }

    private int ComparePriceLesser(CreditDBEntity a, CreditDBEntity b)
    {
        return a.price > b.price ? -1 : 1;
    }
}