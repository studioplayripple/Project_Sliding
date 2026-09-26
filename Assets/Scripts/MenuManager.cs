using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public MneuGameNameEffect mneuGameNameEffect;
    public TweenPanel playButtontween;
    public Button playButton;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        //PlayerPrefs.SetInt(ShapeData.LevelNumberPref, 0);
        //PlayerPrefs.SetInt(ShapeData.TutorialCodePref, 0);
        TweenPanelStatus(playButtontween, false, 0);
        playButton.onClick.AddListener(StartGame);

    }
    public void GameOpens()
    {
        mneuGameNameEffect.SetUp();
        TweenPanelStatus(playButtontween, true, 0.3f);
    }

    private void TweenPanelStatus(TweenPanel tweenpanel, bool isFinal, float time)
    {
        float pos = tweenpanel.posInitial;
        if (isFinal)
        {
            pos = tweenpanel.posFinal;
        }
        tweenpanel.panelRect.DOAnchorPosY(pos, time);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
