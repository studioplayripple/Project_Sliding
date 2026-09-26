using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private Transform handCursor;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Animator handAnimator;

    private List<Vector2> pointList = new List<Vector2>();
    private Sequence sequence;
    private float duration = 2f;
    private Vector3 prevPos;
    private float minDistance = 0.1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void StartTutorial(List<Vector2> points, Gradient gradient)
    {
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
        float opacity = 0.8f; // 0 = transparent, 1 = original
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha *= opacity;
        }

        gradient.alphaKeys = alphaKeys;
        lineRenderer.colorGradient = gradient;
        if(points.Count > 0)
        {
            pointList = points;
            if (points.Count <= 2)
            {
                StartFirstTutorial();
            }else if(points.Count <= 4)
            {
                StartSecondTutorial();
            }
            else
            {
                ResetTutorial();
            }
        }
        else
        {
            ResetTutorial();
        }
    }

    public void ResetTutorial()
    {
        print("aadfafd tute");
        sequence?.Kill();
        handAnimator.StopPlayback();
        lineRenderer.gameObject.SetActive(false);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, handCursor.position);
        handCursor.gameObject.SetActive(false);
    }

    private void StartFirstTutorial()
    {
        handCursor.position = pointList[0];
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, handCursor.position);
        prevPos = handCursor.position;
        sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            handCursor.position = pointList[0];
            handCursor.gameObject.SetActive(true);
            handAnimator.Play("Cursordown");

            // Reset line
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, handCursor.position);
        });

        sequence.AppendInterval(1f);

        sequence.Append(
            handCursor.DOMove(pointList[1], duration)
                .OnUpdate(() =>
                {
                    if (Vector3.Distance(handCursor.position, prevPos) > minDistance)
                    {
                        // Add current cursor position
                        int index = lineRenderer.positionCount;
                        prevPos = handCursor.position;
                        lineRenderer.positionCount = index + 1;
                        lineRenderer.SetPosition(index, handCursor.position);
                    }
                })
        );

        sequence.AppendInterval(1f);

        sequence.AppendCallback(() =>
        {
            handCursor.gameObject.SetActive(false);
        });

        sequence.Append(handCursor.DOMove(pointList[0], 1f));

        sequence.AppendInterval(1f);

        sequence.SetLoops(-1, LoopType.Restart);
    }

    private void StartSecondTutorial()
    {
        handCursor.position = pointList[0];

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, handCursor.position);

        sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            handCursor.position = pointList[0];
            handCursor.gameObject.SetActive(true);
            handAnimator.Play("Cursordown");

            // Reset line
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, handCursor.position);
        });

        sequence.AppendInterval(1f);

        // Bezier movement: P0 -> P1 -> P2 -> P3
        float bezierT = 0f;

        sequence.Append(
            DOTween.To(
                () => bezierT,
                value =>
                {
                    bezierT = value;

                    handCursor.position = GetBezierPoint(
                        bezierT,
                        pointList[0],
                        pointList[1],
                        pointList[2],
                        pointList[3]
                    );

                    // Add current cursor position to LineRenderer
                    int index = lineRenderer.positionCount;

                    lineRenderer.positionCount = index + 1;
                    lineRenderer.SetPosition(index, handCursor.position);
                },
                1f,
                duration
            )
            .SetEase(Ease.Linear)
        );

        sequence.AppendInterval(1f);

        sequence.AppendCallback(() =>
        {
            handCursor.gameObject.SetActive(false);
        });

        sequence.Append(
            handCursor.DOMove(pointList[0], 1f)
        );

        sequence.AppendInterval(1f);

        sequence.SetLoops(-1, LoopType.Restart);
    }

    private Vector3 GetBezierPoint(
    float t,
    Vector2 p0,
    Vector2 p1,
    Vector2 p2,
    Vector2 p3)
    {
        float u = 1f - t;

        return
            u * u * u * p0 +
            3f * u * u * t * p1 +
            3f * u * t * t * p2 +
            t * t * t * p3;
    }
}
