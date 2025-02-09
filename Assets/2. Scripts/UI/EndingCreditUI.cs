using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreditBigEntity
{
    public string typeName;
    public List<CreditSmallEntity> entities;
}

[System.Serializable]
public class CreditSmallEntity
{
    public string typeName;
    public List<CreditNameEntity> names;
}

[System.Serializable]
public class CreditNameEntity
{
    public int spriteIdx;
    public string name;
}

public class EndingCreditUI : MonoBehaviour, IObserver
{
    public Transform creditParent;
    public GameObject creditTypePrefab;
    public GameObject creditSubTypePrefab;
    public GameObject creditNamePrefab;

    public CreditDB creditDB;
    public List<CreditBigEntity> creditEntities;

    private void Start()
    {
        LoadFundingList();
    }

    private void LoadFundingList()
    {
        CreditBigEntity bigEntity = new CreditBigEntity();
        CreditSmallEntity smallEntity = new CreditSmallEntity();
        bigEntity.entities = new List<CreditSmallEntity>();

        bigEntity.typeName = "ÅÒºí¹÷ ÈÄ¿ø";
        smallEntity.typeName = "ÈÄ¿øÀÚ ¸í´Ü";

        bigEntity.entities.Add(smallEntity);
        creditEntities.Add(bigEntity);

        creditDB.entities.Sort(ComparePriceLesser);

        creditEntities[creditEntities.Count - 1].entities[0].names = new List<CreditNameEntity>();
        for (int i = 0;i < creditDB.entities.Count; i++)
        {
            CreditNameEntity nameEntity = new CreditNameEntity();
            nameEntity.spriteIdx = creditDB.entities[i].spriteIdx;
            nameEntity.name = creditDB.entities[i].name;

            creditEntities[creditEntities.Count - 1].entities[0].names.Add(nameEntity);
        }
    }

    private int ComparePriceLesser(CreditDBEntity a, CreditDBEntity b)
    {
        return a.price > b.price ? -1 : 1;
    }

    private void InitCredit()
    {
        for (int i = 0;i < creditEntities.Count; i++)
        {
            Instantiate(creditTypePrefab, creditParent);

            for (int j = 0; j < creditEntities[i].entities.Count; j++)
            {
                Instantiate(creditSubTypePrefab, creditParent);
                
                for (int k = 0; k < creditEntities[i].entities[j].names.Count; k++)
                {
                    Instantiate(creditNamePrefab, creditParent);
                }
            }
        }
    }

    public void Notify(EventState state)
    {
        if (state == EventState.EndingCredit)
        {
            InitCredit();
            StartCoroutine(CityLevelManager.instance.PlayEndingLapse());
        }
    }
}