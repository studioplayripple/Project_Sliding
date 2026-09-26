using System;
using UnityEngine;


[Serializable]
public enum GateType
{
    None, Yellow, Green, Blue, Black
}

public class Gates : MonoBehaviour
{
    public GateType thisGateType = GateType.None;

}
