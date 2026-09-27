using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;



[Serializable]
public class TweenPanel
{
    public RectTransform panelRect;
    public float posInitial;
    public float posFinal;
}

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;


    [SerializeField] private Image bgImage;

    [SerializeField] private TweenPanel upperHolder;
    [SerializeField] private TweenPanel abilityHolder;


    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private Image headingImage;
    [SerializeField] private Sprite[] headingSprites;
    [SerializeField] private TweenPanel gameOverHeading;
    [SerializeField] private TweenPanel gameOverButtonRect;
    [SerializeField] private Button[] gameOverButtons;

    [SerializeField] private GameObject confettiEffect;

    [SerializeField] private GameObject pointDrawnObj;
    [SerializeField] private Text pointDrawnText;
    [SerializeField] private Image[] pointDrawnBGEffects;



    [SerializeField] private Text targetText;
    [SerializeField] private Text levelText;
    [SerializeField] private Image targetFill;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Text starCount;
    [SerializeField] private Button crossButton;


    [SerializeField] private AudioSource[] audioSfx;


    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        gameOverButtons[0].onClick.AddListener(RetryLevel);
        gameOverButtons[1].onClick.AddListener(NextLevel);
        crossButton.onClick.AddListener(ExitGame);
    }
    public void SetUp(GameManager gmmanager, int levelnum, Sprite bgimage)
    {
        bgImage.sprite = bgimage;
        levelText.text = $"Level: {levelnum + 1}";
        gameManager = gmmanager;
        TweenPanelStatus(upperHolder, false, 0);
        TweenPanelStatus(abilityHolder, false, 0);
        gameOverObject.SetActive(false);
        TweenPanelStatus(gameOverHeading, false, 0);
        TweenPanelStatus(gameOverButtonRect, false, 0);
        confettiEffect.SetActive(false);
        pointDrawnObj.SetActive(false);
        foreach (Image im in pointDrawnBGEffects) 
        {
            im.fillAmount = 0;
        }


    }

    private void TweenPanelStatus(TweenPanel tweenpanel,bool isFinal, float time)
    {
        float pos = tweenpanel.posInitial;
        if (isFinal)
        {
            pos = tweenpanel.posFinal;
        }
        tweenpanel.panelRect.DOAnchorPosY(pos, time);
    }

    public void UppderHolderStatus(bool isFinal, float time)
    {
        TweenPanelStatus(upperHolder, isFinal, time);
        TweenPanelStatus(abilityHolder, isFinal, time);
    }
    

    public void GoalShow(int goal, int total)
    {
        if(targetText != null)
        {
            targetText.text = $"{goal}/{total}";
        }
        if(targetFill != null)
        {
            float val = (float)goal / (float)total;
            targetFill.fillAmount = val;
        }
    }

    public void LifeStatus(int index, bool isSetup)
    {
        
        if (isSetup)
        {
            for (int i = 0; i < index; i++)
            {
                if (hearts.Length > index)
                {
                    hearts[index].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (hearts.Length > index && index >= 0)
            {
                hearts[index].gameObject.SetActive(false);
            }
        }
        
    }

    public void CoinShow(int num)
    {
        starCount.text = num.ToString();
    }

    public void GameOver(int gamestatus, int drawpointCount)
    {
        audioSfx[gamestatus].Play();
        headingImage.sprite = headingSprites[0];
        bool iswin = false;
        if (gamestatus == 1)
        {
            headingImage.sprite = headingSprites[1];
            iswin = true;
        }
        pointDrawnText.text = $"+{drawpointCount}";

        gameOverButtons[0].gameObject.SetActive(!iswin);
        gameOverButtons[1].gameObject.SetActive(iswin);

        TweenPanelStatus(upperHolder, false, 0.5f);
        TweenPanelStatus(abilityHolder, false, 0.5f);
        gameOverObject.SetActive(true);
        TweenPanelStatus(gameOverHeading, true, 0.5f);
        StartCoroutine(ShowGameOverButtons(gamestatus));
    }

    private IEnumerator ShowGameOverButtons(int gamestatus)
    {
        yield return new WaitForSeconds(0.5f);
        if (gamestatus == 1)
        {
            audioSfx[2].Play();
            confettiEffect.SetActive(true);
        }
        pointDrawnObj.SetActive(true);
        pointDrawnBGEffects[0].DOFillAmount(1, 0.5f);
        pointDrawnBGEffects[1].DOFillAmount(1, 0.5f);
        yield return new WaitForSeconds(1f);
        TweenPanelStatus(gameOverButtonRect, true, 0.5f);
    }

    private void RetryLevel()
    {
        gameManager.LoadScene();
    }

    private void NextLevel()
    {
        int levelNumber = PlayerPrefs.GetInt(ShapeData.LevelNumberPref, 0);
        levelNumber++;
        PlayerPrefs.SetInt(ShapeData.LevelNumberPref, levelNumber);
        gameManager.LoadScene();
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
