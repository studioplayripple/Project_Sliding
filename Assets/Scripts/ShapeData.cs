using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[Serializable]
public class Targetdata
{
    public ShapeCode goalShapeCode;
    public int totalCount;
}
[Serializable]
public class Leveldata
{
    public Targetdata[] TargetData;
    public int maxShapeCount;
    public int maxCoinSpawn;
    public int maxPoints = 30;
    public int obstacleIndex;
    public int ObstacleLevel;
}



[Serializable]
public enum ShapeCode
{
    None,
    Any,
    Circle,
    Square,
    Polygon,
    Shurican,
    Diamopnd,
    Triangle,
    Trapiz
}

[Serializable]
public class Shape
{
    public ShapeCode shapeCode = ShapeCode.None;
    public Gradient colorGradient;
    public ShapeMovement shapePrefab;
}

public class ShapeData : MonoBehaviour
{

    public static ShapeData Instance;
    public Shape[] allShapes;
    public Leveldata[] allLevelData;
    public Sprite[] allBgSprites;

    public static readonly string FirstPlayPref = "FirstPlayPref";
    public static readonly string TutorialCodePref = "TutorialCodePref";
    public static readonly string LevelNumberPref = "LevelNumberPref";
    public static readonly string CoinCountPref = "CoinCountPref";
    public static readonly string CameraSizePref = "CameraSizePref";
    public static readonly string AbilityTutorialPref = "AbilityTutorialPref";

    public static readonly string RepositionAbilityPref = "RepositionAbilityPref";
    public static readonly string MovementAbilityPref = "MovementAbilityPref";
    public static readonly string MoveHintAbilityPref = "MoveHintAbilityPref";




    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    public List<ShapeCode> AnyShapeCodes(int totalCount)
    {
        int count = allShapes.Length;
        int[] shapeCount = new int[count];
        int max = totalCount / 3;


        List<ShapeCode> shapeCodes = new List<ShapeCode>();
        int currentcount = 0;
        while (currentcount < totalCount)
        {
            int random = UnityEngine.Random.Range(0, count);
            ShapeCode shapecode = allShapes[random].shapeCode;
            if (totalCount < 6)
            {
                for (int i = 0; i < 50; i++)
                {
                    if(shapeCodes.Contains(shapecode))
                    {
                        random = UnityEngine.Random.Range(0, count);
                        shapecode = allShapes[random].shapeCode;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if(shapeCount.Length > random && shapeCount[random] >= max)
                {
                    continue;
                }
                else
                {
                    shapeCount[random]++;
                }
            }
            shapeCodes.Add(shapecode);
            currentcount++;
        }
        return shapeCodes;
    }
}
