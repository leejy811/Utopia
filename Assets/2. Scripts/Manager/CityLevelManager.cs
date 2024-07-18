using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum CityType { Village = 0, Town, City, Utopia }

[System.Serializable]
public struct CityLevel
{
    public CityType type;
    public int debtWeek;
    public Vector2Int tileSize;
}

[System.Serializable]
public struct FrameInfo
{
    public int index;
    public Vector2Int position;
    public Quaternion rotation;
    public bool isInsert;

    public FrameInfo(int idx, Vector2Int pos, Quaternion rot, bool isIn)
    {
        index = idx;
        position = pos;
        rotation = rot;
        isInsert = isIn;
    }
}

public class CityLevelManager : MonoBehaviour
{
    public static CityLevelManager instance;

    public int levelIdx;
    public CityLevel[] level;
    public Queue<FrameInfo> frames = new Queue<FrameInfo>();

    public TimeLapseCamera cameraController;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void UpdateCityType(int week)
    {
        if(levelIdx == level.Length - 1)
            return;

        for (int i = 0;i <= levelIdx;i++)
        {
            week -= level[i].debtWeek;
        }

        if (week >= level[levelIdx + 1].debtWeek)
            StartCoroutine(PlayTimeLapse());
    }

    private void LevelUp()
    {
        levelIdx++;
        Grid.instance.PurchaseTile(level[levelIdx].tileSize);
        UIManager.instance.notifyObserver(EventState.CityLevelUp);
    }

    public bool CheckBuildingLevel(Building building)
    {
        return building.grade <= levelIdx;
    }

    IEnumerator PlayTimeLapse()
    {
        UIManager.instance.notifyObserver(EventState.None);
        InputManager.canInput = false;
        RoutineManager.instance.OnOffDailyLight(false);
        AkSoundEngine.SetRTPCValue("TIMELAP", 2);
        AkSoundEngine.PostEvent("Play_TImeLapsBGM", gameObject);

        cameraController.StartCoroutine(cameraController.SetCameraPrioritiesWithDelay());
        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, true));
        StartCoroutine(PostProcessManager.instance.VignetteInOut(2f, 0.45f));
        UIManager.instance.MovePanelAnim(2f, true);

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, false));

        yield return new WaitForSeconds(2.0f);

        cameraController.rotateOnCoroutine = StartCoroutine(cameraController.RotateCameraOn());

        int timesOfRotate = 2;
        float timePerFrame = ((360 * timesOfRotate) / cameraController.rotationSpeed) / frames.Count;

        while (frames.Count > 0)
        {
            FrameInfo curFrame = frames.Dequeue();

            if (curFrame.isInsert)
            {
                GameObject prefab = BuildingSpawner.instance.buildingPrefabs[curFrame.index];
                Vector3 startPos = new Vector3(Grid.instance.levelUPStartPoint.x, 0, Grid.instance.levelUPStartPoint.y);
                Vector3 spawnPos = new Vector3(curFrame.position.x, 0, curFrame.position.y);
                GameObject building = Instantiate(prefab, spawnPos + startPos, curFrame.rotation, transform);
                Grid.instance.levelUpTiles[curFrame.position.x, curFrame.position.y].building = building;
            }
            else
            {
                Destroy(Grid.instance.levelUpTiles[curFrame.position.x, curFrame.position.y].building);
            }

            yield return new WaitForSeconds(timePerFrame);
        }

        StopCoroutine(cameraController.rotateOnCoroutine);
        cameraController.RotateCameraOff();
        StartCoroutine(StopTimeLapse());
    }

    public IEnumerator StopTimeLapse()
    {
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, true));

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, false));
        StartCoroutine(PostProcessManager.instance.VignetteInOut(2f, 0.0f));
        UIManager.instance.MovePanelAnim(2f, false);

        yield return new WaitForSeconds(2.0f);

        RoutineManager.instance.OnOffDailyLight(true);
        LevelUp();
        AkSoundEngine.SetRTPCValue("TIMELAP", 1);
        AkSoundEngine.PostEvent("Stop_TImeLapsBGM", gameObject);
    }
}
