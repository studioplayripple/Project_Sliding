using DG.Tweening;
using UnityEngine;

public class MneuGameNameEffect : MonoBehaviour
{
    public RectTransform[] points;
    public Transform handCursor;
    public SpriteRenderer handSpriteren;
    public LineRenderer lineRenderer;
    

    private Vector2 prevPos;
    private float minDistance = 0.1f;
    public float speed = 1f;
    public float curveHeight = 2f;

    Vector2 point1 = Vector2.zero;
    Vector2 point2 = Vector2.zero;


    private float t;
    private bool isSetUp;
    private void Start()
    {
        handCursor.gameObject.SetActive(false);
    }
    public void SetUp()
    {
        handCursor.gameObject.SetActive(true);
        isSetUp = true;
        point1 = points[0].TransformPoint(points[0].rect.center);
        point2 = points[1].TransformPoint(points[1].rect.center);
        prevPos = points[0].position;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, prevPos);
        Invoke(nameof(Disablehand), 2f);
        handSpriteren.DOFade(0, 2f);
    }

    void Update()
    {
        if (isSetUp)
        {
            t += speed * Time.deltaTime;
            t = Mathf.Clamp01(t);

            Vector3 pos = Vector3.Lerp(
                point1,
                point2,
                t
            );

            pos.y += Mathf.Sin(t * Mathf.PI) * curveHeight;

            handCursor.position = pos;

            if (Vector3.Distance(handCursor.position, prevPos) > minDistance)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, handCursor.position);
                prevPos = handCursor.position;
            }
        }
    }
    private void Disablehand()
    {
        handCursor.gameObject.SetActive(false);
    }
}
