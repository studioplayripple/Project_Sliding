using System;
using System.Collections.Generic;
using UnityEngine;



public class FloorDataRef : MonoBehaviour
{

    [SerializeField] private Floors[] obstacleObjects;
    [SerializeField] private GameObject[] holeMaskObjects;
    [SerializeField] private Transform[] spawnYTrans;
    [SerializeField] private Transform[] tutorialTrans;

    private Floors currentSelectedFloor = null;


    public void DisableMaskObjects()
    {
        foreach (var obj in holeMaskObjects)
        {
            obj.SetActive(false);
        }
        currentSelectedFloor.DisableMasks();
    }


    public Vector2 SpawnYPosition(int obsIndex)
    {
        SetObstacleHolder(obsIndex);
        Vector2 position = new Vector2(spawnYTrans[0].position.y, spawnYTrans[1].position.y);
        return position;
    }

    private void SetObstacleHolder(int obsIndex)
    {
        foreach (var obstacle in obstacleObjects) 
        {
            obstacle.gameObject.SetActive(false);
        }
        if (obsIndex >= 0 && obsIndex < obstacleObjects.Length)
        {
            obstacleObjects[obsIndex].gameObject.SetActive(true);
            currentSelectedFloor = obstacleObjects[obsIndex];
        }
    }

    public List<Vector2> TutorialPoints(bool isLeft)
    {
        List<Vector2> points = new List<Vector2>();
        if(isLeft)
        {
            Vector2 pos= spawnYTrans[0].position;
            Vector2 pos1= spawnYTrans[2].position;
            points.Add(pos);
            points.Add(pos1);
        }
        else
        {
            Vector2 pos = spawnYTrans[1].position;
            Vector2 pos1 = spawnYTrans[3].position;
            points.Add(pos);
            points.Add(pos1);
        }
        points.Add(spawnYTrans[4].position);
        return points;
    }

    public Vector2 HolePos()
    {
        return tutorialTrans[2].position;
    }
    public List<Vector2> AllTutePoints()
    {
        List<Vector2> points = new List<Vector2>();
        foreach (Transform t in tutorialTrans)
        {
            points.Add(t.position);
        }
        return points;
    }

    public List<GateType> CurrentFloorGateTypes()
    {
        List<GateType> gateTypes = new List<GateType>();
        if (currentSelectedFloor)
        {
            gateTypes = currentSelectedFloor.ActivegateTypes();
        }
        return gateTypes;
    }

    public void DisableGate(int index, bool status)
    {
        if(currentSelectedFloor != null)
        {
            currentSelectedFloor.DisableGateObject(index, status);
        }
    }

}
