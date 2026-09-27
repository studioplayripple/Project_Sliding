using System.Collections;
using UnityEngine;

public class CameraSizeSetter : MonoBehaviour
{

    public Camera cameraObject;
    public RectTransform[] screenBounds;
    public LayerMask targetLayer;


    private MenuManager menuManager;

    void Start()
    {
        menuManager = MenuManager.Instance;
        //PlayerPrefs.SetFloat(ShapeData.CameraSizePref, 0f);
        float num = PlayerPrefs.GetFloat(ShapeData.CameraSizePref, 0);
        if (num <= 0)
        {
            StartCoroutine(SetCameraSize());
        }
        else
        {
            cameraObject.orthographicSize = num;
            Invoke(nameof(GameStart), 0.3f);
        }
    }

    private IEnumerator SetCameraSize()
    {
        float cameraSize = 40;
        for (int i = 0; i < 200; i++)
        {
            Vector2 pos1 = screenBounds[0].TransformPoint(screenBounds[0].rect.center);
            Vector2 pos2 = screenBounds[1].TransformPoint(screenBounds[1].rect.center);
            cameraObject.orthographicSize = cameraSize;
            yield return new WaitForSeconds(0.01f);
            RaycastHit2D hit = Physics2D.CircleCast(
            pos1,
            1.7f,
            transform.right,
            0,
            targetLayer
        );
            RaycastHit2D hit2 = Physics2D.CircleCast(
            pos2,
            2,
            transform.right,
            0,
            targetLayer
        );

            if (hit.collider != null && hit2.collider != null)
            {
                print("detects");
                PlayerPrefs.SetFloat(ShapeData.CameraSizePref, cameraSize);
                Invoke(nameof(GameStart), 0.3f);
                break;
            }
            else
            {
                float val = cameraSize - 0.2f;
                cameraSize = Mathf.Round(val * 10f) / 10f;
            }
        }
    }

    private void GameStart()
    {
        menuManager.GameOpens();

    }
}
