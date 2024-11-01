using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingMapGenerator : MonoBehaviour
{
    public BoxCollider2D[] backGrounds;
    public HorseRacingCamera racingCamera;
    public int mapCount;

    private void Start()
    {
        float xPos = 0.0f;
        Instantiate(backGrounds[0].gameObject, Vector3.right * xPos, Quaternion.identity, transform);
        xPos += backGrounds[0].size.x / 2.0f;
        xPos += backGrounds[1].size.x / 2.0f;

        for (int i = 0;i < mapCount; i++)
        {
            Instantiate(backGrounds[1].gameObject, Vector3.right * xPos, Quaternion.identity, transform);
            xPos += backGrounds[1].size.x;
        }

        xPos -= backGrounds[1].size.x / 2.0f;
        xPos += backGrounds[2].size.x / 2.0f;
        Instantiate(backGrounds[2].gameObject, Vector3.right * xPos, Quaternion.identity, transform);

        xPos += backGrounds[2].size.x / 2.0f;
        xPos += backGrounds[0].size.x / 2.0f;

        racingCamera.centerX = (xPos / 2.0f) - backGrounds[0].size.x / 2.0f;
        racingCamera.mapWidth = xPos / 2.0f;
    }
}
