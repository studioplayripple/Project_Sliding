using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static FaceExpressionManager;

public class ShapeMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform spriteRendererShadow;
    [SerializeField] private Color disableColor;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask holeLayer;
    [SerializeField] private Transform moveHIntTrans;
    [SerializeField] private FaceExpressionManager faceExpressionManager;

    public ShapeCode ThisShapeCode { get; private set; }
    private Vector2 startPos;
    private Vector3 prevPos;
    private float minDistance = 1f;
    private float speed = 15f;
    private int maxPoints = 100;

    private Vector3[] linePositions;
    private int moveIndex = 0;
    public bool startMovement{get; private set;}
    private float currentSpeed = 0;
    private Spawner spawner;
    private bool hasSelected = false;

    private AbilityManager abilityManager;
    private List<int> abilityCodes = new List<int>();
    private bool isAbilityHit = false;

    [Header("Nearby Character Detection")]
    private float nearbyRadius = 4f;
    private int[] irritatedCount = new int[3] {1, 3, 6 };
    [SerializeField] private LayerMask characterLayer;

    [Header("Obstacle Detection")]
    private float[] obstacleDistance = new float[2] {1.5f, 2.5f};
    [SerializeField] private LayerMask obstacleLayer;
    private bool isDizzy;
    private float dizzyTimer;

    public static Action ShapeSelectedEvent = delegate { };
    private void Start()
    {
        spriteRendererShadow.position = transform.position + Vector3.down * 0.2f;
    }
    public void RestartShape()
    {
        moveIndex = 0;
        currentSpeed = 0;
        startMovement = false;
        isDizzy = false;
        isAbilityHit = false;
        transform.position = startPos;
        hasSelected = false;
        minDistance = 1f;
        abilityCodes.Clear();
        linePositions = new Vector3[0];
    }

    public void SetUp(ShapeCode code, int maxpoint, Gradient colgrad, Spawner sp)
    {
        abilityManager = AbilityManager.Instance;
        moveHIntTrans.gameObject.SetActive(false);

        minDistance = 1f;
        lineRenderer.colorGradient = colgrad;
        maxPoints = maxpoint;
        ThisShapeCode = code;
        ShapeSelectedEvent += ChangeColor;
        prevPos = transform.position;
        startPos = prevPos;
        spawner = sp;
    }
    private void OnMouseDown()
    {
        if (spawner.HasGameRunning && (spawner.CurrentSelectedShape == null || hasSelected))
        {
            gameObject.layer = 0;
            hasSelected = true;
            spawner.CurrentSelectedShape = this;
            ShapeSelectedEvent?.Invoke();
            StartLine(transform.position);
            linePositions = new Vector3[0];

            if (abilityCodes.Contains(2))
            {
                moveHIntTrans.gameObject.SetActive(true);
            }
        }
    }

    private void OnMouseDrag()
    {
        if (spawner.HasGameRunning && (spawner.CurrentSelectedShape == null || hasSelected))
            UpdateLine();
    }

    private void OnMouseUp()
    {
        if (spawner.HasGameRunning && (spawner.CurrentSelectedShape == null || hasSelected))
        {
            currentSpeed = speed;
            bool status = HasHole();
            if(status || abilityCodes.Contains(0))
            {
                status = true;
            }
            startMovement = status;
            if (startMovement)
                spawner.drawPoints += linePositions.Length;
            else
            {
                StartLine(transform.position);
            }

        }
        moveHIntTrans.gameObject.SetActive(false);
        moveHIntTrans.position = transform.position;

    }

    private bool HasHole()
    {
        bool hashole = false;
        if (linePositions.Length > 1)
        {
            int lasindex = linePositions.Length - 1;
            Vector2 currentpos = linePositions[lasindex];
            Collider2D hit = Physics2D.OverlapCircle(
                        currentpos,
                        2,
                        holeLayer
                    );
            if (hit != null)
            {
                hashole = true;
                linePositions[lasindex] = hit.transform.position;
            }
            
        }
        return hashole;
    }


    private void Update()
    {
        if (startMovement && spawner.HasGameRunning && linePositions.Length > 0)
        {
            Vector2 currentpos = linePositions[moveIndex];
            Vector2 newPos = Vector2.MoveTowards(transform.position, currentpos, currentSpeed * Time.deltaTime);
            transform.position = newPos;
            float distance = Vector2.Distance(currentpos, transform.position);
            if (distance <= 0.05f)
            {
                if (moveIndex < linePositions.Length - 1)
                    moveIndex++;
                else
                {
                    StartLine(transform.position);
                }
            }
        }
        if (faceExpressionManager == null)
            return;

        // Dizzy has the highest priority.
        if (isDizzy)
        {
            faceExpressionManager.SetFace(FaceState.Dizzy);

            dizzyTimer -= Time.deltaTime;

            if (dizzyTimer <= 0)
            {
                isDizzy = false;
            }

            return;
        }

        // Happy has priority when selected.
        if (hasSelected)
        {
            faceExpressionManager.SetFace(FaceState.Happy);
            return;
        }

        // Check obstacle while moving.
        if (startMovement)
        {
            int num = -1;
            if (IsObstacleClose(obstacleDistance[0]))
            {
                num = 2;
            }else if (IsObstacleClose(obstacleDistance[1]))
            {
                num = 1;
            }
            faceExpressionManager.SetFace(FaceState.Shocked, num);
            return;
        }

        // Check nearby characters.
        int nearbyCharacters = GetNearbyCharacterCount(characterLayer);
        int nearbyObstacle = GetNearbyCharacterCount(obstacleLayer);

        if (nearbyCharacters >= irritatedCount[2])
        {
            faceExpressionManager.SetFace(FaceState.Annoyed, 2);
        }
        else if (nearbyCharacters >= irritatedCount[1])
        {
            faceExpressionManager.SetFace(FaceState.Annoyed, 1);
        }
        else
        {
            if (nearbyObstacle >= 2)
            {
                faceExpressionManager.SetFace(FaceState.Irritated);
            }
            else if (nearbyCharacters >= irritatedCount[0])
            {
                faceExpressionManager.SetFace(FaceState.Annoyed, 0);
            }
            else
            {
                faceExpressionManager.SetFace(FaceState.Idle);
            }
        }
        
    }



    private void StartLine(Vector2 position)
    {
        isAbilityHit = false;
        minDistance = 1f;
        startMovement = false;
        moveIndex = 0;
        linePositions = new Vector3[0];
        prevPos = transform.position;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, position);
    }


    void UpdateLine()
    {
        if (Input.GetMouseButton(0) && lineRenderer.positionCount < maxPoints && spawner.HasGameRunning)
        {
            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(currentPos, prevPos) > minDistance)
            {
                CheckForAbilityHint(currentPos);
                if (!isAbilityHit) 
                {
                    minDistance = 0.2f;
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);
                    prevPos = currentPos;
                    linePositions = new Vector3[lineRenderer.positionCount];
                    lineRenderer.GetPositions(linePositions);
                    if (abilityCodes.Contains(2))
                    {
                        moveHIntTrans.position = currentPos;
                    }
                }
            }
            
        }
    }

    private void CheckForAbilityHint(Vector2 point)
    {
        if (abilityCodes.Contains(1) && !isAbilityHit)
        {
            if(spawner.HasObstacleHit(point, 1.5f))
            {
                isAbilityHit = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (startMovement) 
        {
            if (collision.gameObject.CompareTag("Objects") || collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Gate"))
            {
                dizzyTimer = 2f;
                isDizzy = true;
                ResetEvent();
                StartLine(transform.position);
                if (spawner)
                {
                    spawner.UpdateLifeCount(1);
                }
            }
            else if (collision.gameObject.CompareTag("Hole"))
            {
                ResetEvent();
                StartLine(transform.position);
                gameObject.SetActive(false);
                if (spawner)
                {
                    spawner.UpdateGoalCount(ThisShapeCode);
                }
            } 
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (startMovement)
        {
            if (collision.gameObject.CompareTag("Coin"))
            {
                spawner.CoinCollected();
                Destroy(collision.gameObject);
            }
        }
    }
    private void ResetEvent()
    {
        if (abilityCodes.Count > 0)
        {
            abilityManager.AbilityUsed(true);
        }
        hasSelected = false;
        spawner.CurrentSelectedShape = null;
        ShapeSelectedEvent?.Invoke();
    }

    private void ChangeColor()
    {
        Color col = Color.white;
        bool status = true;
        if (spawner.CurrentSelectedShape && !hasSelected)
        {
            col = disableColor;
            status = false;
        }
        if (faceExpressionManager != null)
            faceExpressionManager.gameObject.SetActive(status);
        spriteRenderer.color = col;
    }

    public void AbilityCodeSetup(int index, bool status)
    {
        if (status && index >= 0)
        {
            if(!abilityCodes.Contains(index))
                abilityCodes.Add(index);
        }
        else
        {
            abilityCodes.Clear();
        }
    }

    // --------------------------------------------------
    // NEARBY CHARACTERS
    // --------------------------------------------------

    private int GetNearbyCharacterCount(LayerMask characterlayer)
    {
        Collider2D[] characters = Physics2D.OverlapCircleAll(
            transform.position,
            nearbyRadius,
            characterlayer
        );

        int count = 0;

        foreach (Collider2D character in characters)
        {
            if (character.gameObject != gameObject)
            {
                count++;
            }
        }

        return count;
    }

    // --------------------------------------------------
    // OBSTACLE DETECTION
    // --------------------------------------------------

    private bool IsObstacleClose(float distance)
    {
        Vector2 direction = transform.up;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            distance,
            obstacleLayer
        );

        return hit.collider != null;
    }






    private void OnDisable()
    {
        ShapeSelectedEvent -= ChangeColor;

    }
}
