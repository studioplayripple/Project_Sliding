using UnityEngine;

public class FaceExpressionManager : MonoBehaviour
{
    public enum FaceState
    {
        Idle,
        Annoyed,
        Irritated,
        Happy,
        Shocked,
        Dizzy
    }

    [Header("Face Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] annoyedSprite;
    [SerializeField] private Sprite irritatedSprite;
    [SerializeField] private Sprite[] happySprite;
    [SerializeField] private Sprite[] shockedSprite;
    [SerializeField] private Sprite dizzySprite;

    [Header("Face Renderer")]
    [SerializeField] private SpriteRenderer faceRenderer;

    private FaceState currentState = FaceState.Idle;


    private void Start()
    {
        SetFace(FaceState.Idle);
    }

    // --------------------------------------------------
    // FACE
    // --------------------------------------------------
    public void SetFace(FaceState newState, int num = -1)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (newState)
        {
            case FaceState.Idle:
                faceRenderer.sprite = idleSprite;
                break;

            case FaceState.Annoyed:
                int rand1 = Random.Range(0, annoyedSprite.Length);
                if (num > 0)
                {
                    rand1 = num;
                }
                faceRenderer.sprite = annoyedSprite[rand1];
                break;

            case FaceState.Irritated:
                faceRenderer.sprite = irritatedSprite;
                break;

            case FaceState.Happy:
                int rand2 = Random.Range(0, happySprite.Length);
                faceRenderer.sprite = happySprite[rand2];
                break;

            case FaceState.Shocked:
                int rand3 = 0;
                if (num > 0)
                {
                    rand3 = num;
                }
                faceRenderer.sprite = shockedSprite[rand3];
                break;

            case FaceState.Dizzy:
                faceRenderer.sprite = dizzySprite;
                break;
        }
    }
}
