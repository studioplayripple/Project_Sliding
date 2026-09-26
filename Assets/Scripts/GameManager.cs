using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Spawner shapeSpawner;


    private ShapeData shapeData;
    private UiManager uiManager;
    private AbilityManager abilityManager;

    private int levelNumber;
    private int totalCoin;

    public int GetCoinCount { get { return totalCoin; } }


    private int gameStatus = -1;
    private int[] totalGoal;
    private int[] currentGoal;
    private int currentLife;
    void Start()
    {
        PlayerPrefs.SetInt(ShapeData.LevelNumberPref,2);
        //PlayerPrefs.SetInt(ShapeData.TutorialCodePref, 1);

        levelNumber = PlayerPrefs.GetInt(ShapeData.LevelNumberPref, 0);
        totalCoin = PlayerPrefs.GetInt(ShapeData.CoinCountPref, 0);
        float camSize = PlayerPrefs.GetFloat(ShapeData.CameraSizePref, 0);
        Camera.main.orthographicSize = camSize;

        shapeData = ShapeData.Instance;
        uiManager = UiManager.Instance;
        abilityManager = AbilityManager.Instance;

        int ability1 = PlayerPrefs.GetInt(ShapeData.RepositionAbilityPref, 5);
        int ability2 = PlayerPrefs.GetInt(ShapeData.MovementAbilityPref, 4);
        int ability3 = PlayerPrefs.GetInt(ShapeData.MoveHintAbilityPref, 3);
        List<int> list = new List<int>();
        list.Add(ability1);
        list.Add(ability2);
        list.Add(ability3);
        abilityManager.SetUp(list, levelNumber, shapeSpawner);

        Leveldata leveldata = shapeData.allLevelData[levelNumber];
        LevelShapeCodes(leveldata);
    }

    private void LevelShapeCodes(Leveldata leveldata)
    {
        uiManager.SetUp(this);
        gameStatus = -1;
        int count = leveldata.TargetData.Length;

        totalGoal = new int[count];
        currentGoal = new int[count];

        for (int i = 0; i < count; i++)
        {
            int num = leveldata.TargetData[i].totalCount;
            totalGoal[i] = num;
        }

        int maxShape = leveldata.maxShapeCount;


        uiManager.GoalShow(currentGoal[0], totalGoal[0]);
        currentLife = 3;
        uiManager.LifeStatus(currentLife, true);

        uiManager.CoinShow(totalCoin);

        List<ShapeCode> shapeCodes = new List<ShapeCode>();
        if (leveldata.TargetData.Length > 0)
        {
            if (leveldata.TargetData[0].goalShapeCode == ShapeCode.Any)
            {
                shapeCodes = shapeData.AnyShapeCodes(maxShape);
            }
        }

        if(shapeCodes.Count > 0)
        {
            shapeSpawner.SpawnShapes(shapeCodes, leveldata.maxCoinSpawn, leveldata.maxPoints, leveldata.obstacleIndex, leveldata.ObstacleLevel);
        }
        uiManager.UppderHolderStatus(true, 0.5f);

    }

    public void UpdateGoal(ShapeCode code)
    {
        currentGoal[0] += 1;

        if (currentGoal[0] >= totalGoal[0])
        {
            gameStatus = 1;
            currentGoal[0] = totalGoal[0];
            print("Game Won");
            GameEnded();


        }
        uiManager.GoalShow(currentGoal[0], totalGoal[0]);
    }
    public void UpdateLife(int num)
    {
        if(currentLife > 0)
        {
            currentLife -= num;
        }
        uiManager.LifeStatus(currentLife, false);

        if(currentLife <= 0)
        {
            gameStatus = 0;
            GameEnded();

        }
    }

    public void GameEnded()
    {
        shapeSpawner.HasGameRunning = false;
        Invoke(nameof(GameOver), 0.5f);
    }

    private void GameOver()
    {
        int drawpoint = shapeSpawner.drawPoints;
        uiManager.GameOver(gameStatus, drawpoint);
        shapeSpawner.GameOver();
    }
    public void CoinCollected(int val)
    {
        totalCoin += val;
        PlayerPrefs.SetInt(ShapeData.CoinCountPref, totalCoin);
        uiManager.CoinShow(totalCoin);
    }


    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }



}
