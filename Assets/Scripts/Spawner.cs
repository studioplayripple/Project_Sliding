using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private FloorDataRef[] floorDataRefPrefab;

    [SerializeField] private Key keyObjPrefab;

    [SerializeField] private AudioSource[] audioSfx;

    [Header("Spawn Settings")]
    private float spawnDiff = 2f;

    [Header("Collision")]
    public LayerMask obstacleLayer;

    private ShapeData shapeData;
    private FloorDataRef floorSpawnRef;
    private List<Vector2> currentSpawnPoints = new List<Vector2>();
    private List<Vector2> spawnPoints = new List<Vector2>();
    private Vector2 yInstaPos = Vector2.zero;
    private int currentCoinSpawn = 0;
    private int maxCoinSpawn = 0;

    public bool HasGameRunning = true;
    public ShapeMovement CurrentSelectedShape;
    public int drawPoints = 0;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<ShapeMovement> shapesRef = new List<ShapeMovement>();
    private bool isTutorial;
    private int tutorialCode = 0;
    private int gateCount = 0;

    

    public void SpawnShapes(List<ShapeCode> shapecodes, int maxcoin, int maxpoint, int obsIndex, int obsLevel)
    {
        if(shapeData == null)
            shapeData = ShapeData.Instance;

        gateCount = 0;
        yInstaPos = Vector2.zero;
        CheckForGate(obsIndex, obsLevel);
        StartCoroutine(SpawnShapeCO(shapecodes, maxcoin, maxpoint));
    }

    private void CheckForGate(int obsIndex, int obstaclelevel)
    {
        floorSpawnRef = Instantiate(floorDataRefPrefab[obstaclelevel], transform);
        spawnedObjects.Add(floorSpawnRef.gameObject);
        floorSpawnRef.transform.localPosition = Vector2.zero;
        yInstaPos = floorSpawnRef.SpawnYPosition(obsIndex);
        SpawnObject();

    }

    private IEnumerator SpawnShapeCO(List<ShapeCode> shapecodes, int maxcoin, int maxpoint)
    {
        yield return new WaitForSeconds(0.1f);
        drawPoints = 0;
        currentCoinSpawn = 0;
        maxCoinSpawn = maxcoin;
        for (int i = 0; i < shapecodes.Count; i++)
        {
            ShapeCode shapecode = shapecodes[i];
            if (shapecode != ShapeCode.None && shapecode != ShapeCode.Any && currentSpawnPoints.Count > 0)
            {
                int rand = UnityEngine.Random.Range(0, currentSpawnPoints.Count);
                Vector2 spawnPosition = currentSpawnPoints[rand];

                Shape shape = SpawnPrefab(shapecode);
                ShapeMovement spawnmove = Instantiate(shape.shapePrefab, transform);
                spawnedObjects.Add(spawnmove.gameObject);
                shapesRef.Add(spawnmove);
                spawnmove.SetUp(shapecode, maxpoint, shape.colorGradient, this);
                spawnmove.transform.position = spawnPosition;

                currentSpawnPoints.RemoveAt(rand);
            }
        }
        
        isTutorial = false;
        tutorialCode = PlayerPrefs.GetInt(ShapeData.TutorialCodePref, 0);
        if (tutorialCode < 2)
        {
            isTutorial = true;
        }

        currentSpawnPoints = spawnPoints;
        print(currentSpawnPoints.Count);
        StartCoroutine(SpawnRoutine());

    }

    private void SpawnObject()
    {
        spawnPoints.Clear();
        HasGameRunning = true;
        for (float j = yInstaPos.y; j > yInstaPos.x; j -= spawnDiff)
        {
            for (float k = -12; k < 13; k += spawnDiff)
            {
                Vector2 spawnPosition = new Vector2(k, j);
                if (!HasObstacleHit(spawnPosition, 1))
                {
                    spawnPoints.Add(spawnPosition);
                    spawnPoints.Add(spawnPosition);

                }
            }
        }

        for (int i = spawnPoints.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector2 temp = spawnPoints[i];
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }
        currentSpawnPoints = spawnPoints;
        print(currentSpawnPoints.Count);

    }
    public bool HasObstacleHit(Vector2 spawnPosition, float rad)
    {
        bool hasObstacleHit = false;
        Collider2D hit = Physics2D.OverlapCircle(
                    spawnPosition,
                    rad,
                    obstacleLayer
                );

        if (hit != null)
        {
            hasObstacleHit = true;
        }
        return hasObstacleHit;
    }
    private Shape SpawnPrefab(ShapeCode shapecode)
    {
        Shape obj = null;

        for (int i = 0; i < shapeData.allShapes.Length; i++)
        {
            if (shapeData.allShapes[i].shapeCode == shapecode)
            {
                obj = shapeData.allShapes[i]; 
                break;
            }
        }
        return obj;
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        List<GateType> gateTypes = floorSpawnRef.CurrentFloorGateTypes();
        int count = gateTypes.Count;
        int max = 50;
        if (count > 0)
        {
            for (int i = 0; i < gateTypes.Count; i++)
            {
                while (true)
                {
                    int rand = UnityEngine.Random.Range(0, currentSpawnPoints.Count);
                    Vector2 spawnPosition = currentSpawnPoints[rand];
                    if (max > 0)
                    {
                        max--;
                        Collider2D hit = Physics2D.OverlapCircle( spawnPosition, 1, obstacleLayer);
                        if (hit == null)
                        {
                            Key key = Instantiate(keyObjPrefab, transform);
                            key.transform.position = spawnPosition;
                            key.SetUp(i, gateTypes[i], this);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                gateCount++;
            }
        }

        yield return new WaitForSeconds(0.1f);
        int maxitter = 100;
        while (currentCoinSpawn < maxCoinSpawn)
        {
            if(maxitter > 0)
            {
                int rand = UnityEngine.Random.Range(0, currentSpawnPoints.Count);
                Vector2 spawnPosition = currentSpawnPoints[rand];

                // Check if another object exists here
                Collider2D hit = Physics2D.OverlapCircle(
                    spawnPosition,
                    1,
                    obstacleLayer
                );
                if (hit == null)
                {
                    // Spawn object
                    GameObject coin = Instantiate(coinPrefab, transform);
                    spawnedObjects.Add(coin.gameObject);
                    coin.transform.position = spawnPosition;
                    currentSpawnPoints.RemoveAt(rand);
                    currentCoinSpawn++;

                }
                else
                {
                    maxitter--;
                }
            }
            else
            {
                yield break;
            }
        }
        TutorialStart();

    }

    private void TutorialStart()
    {
        InvokeRepeating(nameof(CoinSpawner), 0.3f, 3f);
        if (isTutorial)
        {
            if(tutorialCode == 1)
            {
                GetSecondTutorialShapeTrans();
            }
            else
            {
                GetFirstTutorialShapeTrans();
            }
        }
    }

    private void CoinSpawner()
    {
        if(currentCoinSpawn < maxCoinSpawn && HasGameRunning)
        {
            int rand = UnityEngine.Random.Range(0, currentSpawnPoints.Count);
            Vector2 spawnPosition = currentSpawnPoints[rand];

            // Check if another object exists here
            Collider2D hit = Physics2D.OverlapCircle(
                spawnPosition,
                1,
                obstacleLayer
            );
            if (hit == null)
            {
                // Spawn object
                GameObject coin = Instantiate(coinPrefab, transform);
                spawnedObjects.Add(coin.gameObject);
                coin.transform.position = spawnPosition;
                currentSpawnPoints.RemoveAt(rand);
                currentCoinSpawn++;

            }
        }
        else
        {
            CancelInvoke(nameof(CoinSpawner));
        }
    }




    public void GameOver()
    {
        foreach (var obj in spawnedObjects) 
        { 
            if(obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        shapesRef.Clear();
    }




    public void UpdateGoalCount(ShapeCode code)
    {
        gameManager.UpdateGoal(code);
        audioSfx[1].Play();
        if (isTutorial)
        {
            TutorialCheck();
        }

    }
    public void UpdateLifeCount(int num)
    {
        gameManager.UpdateLife(num);
        audioSfx[2].Play();
        if (isTutorial)
        {
            TutorialCheck();
        }

    }
    private void TutorialCheck()
    {
        tutorialCode++;
        isTutorial = false;
        TutorialManager.Instance.ResetTutorial();
        PlayerPrefs.SetInt(ShapeData.TutorialCodePref, tutorialCode);
    }
    public void CoinCollected()
    {
        gameManager.CoinCollected(1);
        audioSfx[0].Play();
    }

    private void GetFirstTutorialShapeTrans()
    {
        Transform trans = null;
        ShapeCode code = ShapeCode.None;
        Vector2 origin = floorSpawnRef.HolePos();
        float mindistance = 100;
        foreach (var obj in shapesRef)
        {
            Vector3 direction = (Vector2)obj.transform.position - origin;
            float distance = direction.magnitude;
            float ydistance = Vector2.Distance(origin, obj.transform.position);

            if (!Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, obstacleLayer) && ydistance < mindistance)
            {
                mindistance = ydistance;
                trans = obj.transform;
                code = obj.ThisShapeCode;
            }
        }
        if (code != ShapeCode.None)
        {
            Gradient colGrad = SpawnPrefab(code).colorGradient;
            List<Vector2> points = new List<Vector2>();
            points.Add(trans.position);
            points.Add(origin);
            TutorialManager.Instance.StartTutorial(points, colGrad);
        }
    }

    private void GetSecondTutorialShapeTrans()
    {
        Transform trans = null;
        ShapeCode code = ShapeCode.None;
        List<Vector2> origins = floorSpawnRef.AllTutePoints();
        float mindistance = 100;
        foreach (var obj in shapesRef)
        {
            Vector3 direction = (Vector2)obj.transform.position - origins[2];
            float distance = direction.magnitude;
            float ydistance = Vector2.Distance(origins[0], obj.transform.position);

            if (!Physics.Raycast(origins[2], direction.normalized, out RaycastHit hit, distance, obstacleLayer) && ydistance < mindistance)
            {
                mindistance = ydistance;
                trans = obj.transform;
                code = obj.ThisShapeCode;
            }
        }
        if (code != ShapeCode.None)
        {
            Gradient colGrad = SpawnPrefab(code).colorGradient;
            List<Vector2> points = new List<Vector2>();
            points.Add(trans.position);
            points.Add(origins[0]);
            points.Add(origins[1]);
            points.Add(origins[2]);
            TutorialManager.Instance.StartTutorial(points, colGrad);
        }
    }

    private List<Vector2> TutorialPonts(Transform shape)
    {
        List<Vector2> points = new List<Vector2>();
        Vector2 pos = shape.position;
        points.Add(pos);

        Vector2 pos2 = floorSpawnRef.HolePos();
        points.Add(pos2);

        return points;
    }

    public void KeyCollected(int index)
    {
        floorSpawnRef.DisableGate(index, false);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    int segments = 32;
    //    for (float j = 18.8f; j > -2; j -= spawnDiff)
    //    {
    //        for (float k = -12; k < 13; k += spawnDiff)
    //        {
    //            Vector2 spawnPosition = new Vector2(k, j);

    //            // Check if another object exists here
    //            Collider2D hit = Physics2D.OverlapCircle(
    //                spawnPosition,
    //                spawnDiff,
    //                obstacleLayer
    //            );
    //            if (hit == null)
    //            {
    //                DrawCircle(spawnPosition, spawnDiff, segments);

    //            }
    //        }
    //    }
    //}
    //private void DrawCircle(Vector2 center, float radius, int segments)
    //{
    //    Vector3 previousPoint = center + Vector2.right * radius;

    //    for (int i = 1; i <= segments; i++)
    //    {
    //        float angle = i * Mathf.PI * 2f / segments;

    //        Vector3 currentPoint = center + new Vector2(
    //            Mathf.Cos(angle),
    //            Mathf.Sin(angle)
    //        ) * radius;

    //        Gizmos.DrawLine(previousPoint, currentPoint);

    //        previousPoint = currentPoint;
    //    }
    //}
}
