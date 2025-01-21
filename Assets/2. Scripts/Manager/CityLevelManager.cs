using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
public class FrameInfo
{
    public int index;
    public Vector2Int position;
    public Quaternion rotation;
    public bool isInsert;
}

public class CityLevelManager : MonoBehaviour
{
    public static CityLevelManager instance;

    public int levelIdx;
    public CityLevel[] level;
    public Queue<FrameInfo> prevFrames = new Queue<FrameInfo>();
    public Queue<FrameInfo> curFrames = new Queue<FrameInfo>();

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

        for (int i = 0; i <= levelIdx; i++)
        {
            week -= level[i].debtWeek;
        }

        if (week >= level[levelIdx + 1].debtWeek)
            StartCoroutine(PlayTimeLapse());
    }

    private void LevelUp()
    {
        levelIdx++;
        Grid.instance.PurchaseTile(level[levelIdx].tileSize, level[levelIdx - 1].tileSize, 3f);
        UIManager.instance.SetDebtInfo();

        if (levelIdx == level.Length - 1)
            UIManager.instance.notifyObserver(EventState.GameClear);
        else
            UIManager.instance.notifyObserver(EventState.CityLevelUp);
    }

    public void LoadLevel(int levelData)
    {
        levelIdx = levelData;
        Grid.instance.PurchaseTile(level[levelIdx].tileSize, level[0].tileSize, 0f);
    }

    public bool CheckBuildingLevel(Building building)
    {
        return building.grade <= levelIdx;
    }

    public int GetPrefixSumWeek()
    {
        int sum = 0;

        for(int i = 0;i <= levelIdx;i++)
        {
            sum += level[i].debtWeek;
        }

        return sum;
    }

    public void LoadFrame(FrameInfo[] prevFrames, FrameInfo[] curFrames)
    {
        foreach (FrameInfo frame in prevFrames)
        {
            this.prevFrames.Enqueue(frame);
            DequeueFrame(frame);
        }

        foreach (FrameInfo frame in curFrames)
        {
            this.curFrames.Enqueue(frame);
        }
    }

    public void DequeueFrame(FrameInfo frame)
    {
        if (frame.isInsert)
        {
            GameObject prefab = BuildingSpawner.instance.buildingPrefabs[frame.index];
            Vector3 startPos = new Vector3(Grid.instance.levelUPStartPoint.x, 0, Grid.instance.levelUPStartPoint.y);
            Vector3 spawnPos = new Vector3(frame.position.x, 0, frame.position.y);
            GameObject building = Instantiate(prefab, spawnPos + startPos, frame.rotation, transform);
            Tile tile = Grid.instance.levelUpTiles[frame.position.x, frame.position.y];
            tile.building = building;
            tile.smokeFX.Play(true);
        }
        else
        {
            Destroy(Grid.instance.levelUpTiles[frame.position.x, frame.position.y].building);
        }
    }

    IEnumerator PlayTimeLapse()
    {
        InputManager.SetCanInput(false);
        RoutineManager.instance.OnOffDailyLight(false);

        yield return new WaitForSeconds(2.0f);

        UIManager.instance.notifyObserver(EventState.None);
        AkSoundEngine.SetRTPCValue("TIMELAP", 2);
        AkSoundEngine.PostEvent("Play_TImeLapsBGM", gameObject);

        yield return new WaitForSeconds(0.8f);

        cameraController.StartCoroutine(cameraController.SetCameraPrioritiesWithDelay());
        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, true));
        StartCoroutine(PostProcessManager.instance.VignetteInOut(2f, 0.45f));
        UIManager.instance.MovePanelAnim(2f, true);


        yield return new WaitForSeconds(2.0f);

        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, false));

        yield return new WaitForSeconds(2.0f);

        UIManager.instance.skipTimeLapse.gameObject.SetActive(true);
        cameraController.rotateOnCoroutine = StartCoroutine(cameraController.RotateCameraOn());

        int timesOfRotate = 2;
        float timePerFrame = ((360 * timesOfRotate) / cameraController.rotationSpeed) / curFrames.Count;

        while (curFrames.Count > 0)
        {
            FrameInfo curFrame = curFrames.Dequeue();
            prevFrames.Enqueue(curFrame);

            DequeueFrame(curFrame);

            if (UIManager.instance.skipTimeLapse.isSkip)
                yield return null;
            else
                yield return new WaitForSeconds(timePerFrame);
        }

        UIManager.instance.skipTimeLapse.gameObject.SetActive(false);
        StopCoroutine(cameraController.rotateOnCoroutine);
        cameraController.RotateCameraOff();
        StartCoroutine(StopTimeLapse());
    }

    public IEnumerator StopTimeLapse()
    {
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, true));

        yield return new WaitForSeconds(2.0f);

        AkSoundEngine.SetRTPCValue("TIMELAP", 1);
        AkSoundEngine.PostEvent("Stop_TImeLapsBGM", gameObject);

        StartCoroutine(PostProcessManager.instance.FadeInOut(2f, false));
        StartCoroutine(PostProcessManager.instance.VignetteInOut(2f, 0.0f));
        UIManager.instance.MovePanelAnim(2f, false);

        yield return new WaitForSeconds(2.0f);

        RoutineManager.instance.OnOffDailyLight(true);
        LevelUp();
    }
}
