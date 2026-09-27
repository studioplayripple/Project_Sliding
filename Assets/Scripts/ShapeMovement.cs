using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShapeMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color disableColor;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask holeLayer;
    [SerializeField] private Transform moveHIntTrans;

    public ShapeCode ThisShapeCode { get; private set; }
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

    public static Action ShapeSelectedEvent = delegate { };

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
        if (spawner.CurrentSelectedShape && !hasSelected)
        {
            col = disableColor;
        }
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

    private void OnDisable()
    {
        ShapeSelectedEvent -= ChangeColor;

    }
}
