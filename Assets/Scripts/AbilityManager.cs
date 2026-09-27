using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[Serializable]
public class AbilityData
{
    public Sprite abilitySprite;
    public Button abilityButton;
    public Text abilityCountText;
    public int abilityCount;
    public int unlockLevel;
    public string abilityName;
    public string abilityDetails;
}

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    [SerializeField] private AbilityData[] abilityDataRef;
    [SerializeField] private VideoClip[] abilityVideoClip;


    [SerializeField] private Image abilityTuteBG;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Text abilityNameText;
    [SerializeField] private VideoPlayer abilityVideoPlayer;
    [SerializeField] private Text abilityDetailText;
    [SerializeField] private Button abilityOkButton;


    private Spawner spawner;
    private int currentlevel;
    private int currentAbilityActive = -1;
    private int tutorialNum;

    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        abilityTuteBG.DOFade(0, 0);
        abilityTuteBG.gameObject.SetActive(false);
        abilityVideoPlayer.gameObject.SetActive(false);
        abilityDataRef[0].abilityButton.onClick.AddListener(RepositionAbility);
        abilityDataRef[1].abilityButton.onClick.AddListener(MovementAbility);
        abilityDataRef[2].abilityButton.onClick.AddListener(MoveHintAbility);

        abilityOkButton.onClick.AddListener(DisableAbilityTutorial);
        ShapeMovement.ShapeSelectedEvent += CheckForSelectedShape;
    }
    private void OnDisable()
    {
        ShapeMovement.ShapeSelectedEvent -= CheckForSelectedShape;
    }

    public void SetUp(List<int> abilitycounts, int level, Spawner sp)
    {
        for (int i = 0; i < abilityDataRef.Length; i++)
        {
            int num = 0;
            if(i < abilitycounts.Count)
            {
                num = abilitycounts[i];
            }
            SetAbilityCount(i, num);
        }
        currentlevel = level;
        spawner = sp;
        currentAbilityActive = -1;
        AbilityStatus();
        CheckForSelectedShape();

        tutorialNum = PlayerPrefs.GetInt(ShapeData.AbilityTutorialPref, 0);
        for (int i = 0; i < abilityDataRef.Length; i++)
        {
            if(currentlevel == abilityDataRef[i].unlockLevel && tutorialNum == i)
            {
                AbilityTutorial(tutorialNum);
                tutorialNum++;
                break;
            }
        }

    }

    private void AbilityTutorial(int index)
    {
        spawner.HasGameRunning = false;
        abilityTuteBG.gameObject.SetActive(true);
        abilityVideoPlayer.gameObject.SetActive(false);
        abilityIcon.sprite = abilityDataRef[index].abilitySprite;
        abilityNameText.text = abilityDataRef[index].abilityName;
        abilityDetailText.text = abilityDataRef[index].abilityDetails;
        abilityVideoPlayer.clip = abilityVideoClip[index];
        abilityTuteBG.DOFade(0.95f, 0.5f).OnComplete(() =>
        {
            abilityVideoPlayer.gameObject.SetActive(true);
            abilityVideoPlayer.Play();

        });
    }



    private void SetAbilityCount(int index, int num)
    {
        if(index >= 0 && index < abilityDataRef.Length)
        {
            abilityDataRef[index].abilityCount = num;
            abilityDataRef[index].abilityCountText.text = $"x{num}";
        }
    }

    private void CheckForSelectedShape()
    {
        if (spawner == null)
            return;
        if(currentAbilityActive >= 0 && spawner.CurrentSelectedShape != null)
        {
            spawner.CurrentSelectedShape.AbilityCodeSetup(currentAbilityActive, true);
        }
    }
    private void AbilityStatus()
    {
        for (int i = 0; i < abilityDataRef.Length; i++)
        {
            bool isActive = false;
            if (abilityDataRef[i].unlockLevel <= currentlevel && abilityDataRef[i].abilityCount > 0)
            {
                isActive = true;
            }
            abilityDataRef[i].abilityButton.interactable = isActive;
        }
    }
    private void RepositionAbility()
    {
        currentAbilityActive = 0;
        CheckForSelectedShape();
        SetButtonsDisable();
    }
    private void MovementAbility()
    {
        currentAbilityActive = 1;
        CheckForSelectedShape();
        SetButtonsDisable();
    }
    private void MoveHintAbility()
    {
        currentAbilityActive = 2;
        CheckForSelectedShape();
        SetButtonsDisable();
    }

    private void SetButtonsDisable()
    {
        abilityDataRef[currentAbilityActive].abilityButton.interactable = false;
    }

    public void AbilityUsed(bool used)
    {
        int num = 0;
        if(used)
        {
            num = -1;
        }
        if (abilityDataRef[currentAbilityActive].abilityCount > 0)
        {
            int val = abilityDataRef[currentAbilityActive].abilityCount + num;
            SetAbilityCount(currentAbilityActive, val);
        }
        ResetActiveAbility();
    }
    private void ResetActiveAbility()
    {
        currentAbilityActive = -1;
        if (spawner.CurrentSelectedShape != null)
        {
            spawner.CurrentSelectedShape.AbilityCodeSetup(currentAbilityActive, false);
        }
        AbilityStatus();
    }
    private void DisableAbilityTutorial()
    {
        spawner.HasGameRunning = true;
        abilityTuteBG.gameObject.SetActive(false);
        abilityVideoPlayer.gameObject.SetActive(false);
        PlayerPrefs.SetInt(ShapeData.AbilityTutorialPref, tutorialNum);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(ShapeData.RepositionAbilityPref, abilityDataRef[0].abilityCount);
        PlayerPrefs.SetInt(ShapeData.MovementAbilityPref, abilityDataRef[1].abilityCount);
        PlayerPrefs.SetInt(ShapeData.MoveHintAbilityPref, abilityDataRef[2].abilityCount);
    }
}
