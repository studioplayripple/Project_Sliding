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
        List<ShapeCode> shapeCodes = new List<ShapeCode>();
        int currentcount = 0;
        while (currentcount < totalCount)
        {
            int maxRange = Enum.GetValues(typeof(ShapeCode)).Length;
            int random = UnityEngine.Random.Range(2, maxRange);
            ShapeCode shapecode = (ShapeCode)random;
            if (totalCount < 6)
            {
                for (int i = 0; i < 50; i++)
                {
                    if(shapeCodes.Contains(shapecode))
                    {
                        random = UnityEngine.Random.Range(2, maxRange);
                        shapecode = (ShapeCode)random;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            shapeCodes.Add(shapecode);
            currentcount++;
        }
        return shapeCodes;
    }
}
