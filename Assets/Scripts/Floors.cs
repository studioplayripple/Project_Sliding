using System.Collections.Generic;
using UnityEngine;

public class Floors : MonoBehaviour
{
    [SerializeField] private Gates[] allCurrentGates;
    [SerializeField] private GameObject[] ObstacleObjects;


    public List<GateType> ActivegateTypes()
    {
        List<GateType> currentGateTypes = new List<GateType>();
        foreach (Gates gate in allCurrentGates)
        {
            GateType type = gate.thisGateType;
            currentGateTypes.Add(type);
        }
        return currentGateTypes;
    }

    public void DisableGateObject(int index, bool status)
    {
        if(index >= 0 && index < allCurrentGates.Length)
        {
            allCurrentGates[index].gameObject.SetActive(status);
        }
    }
}
