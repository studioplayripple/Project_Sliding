using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class AbilityData
{
    public Button abilityButton;
    public int abilityCount;
    public int unlockLevel;
}

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    [SerializeField] private AbilityData[] abilityDataRef;

    private Spawner spawner;
    private int currentlevel;
    private int currentAbilityActive = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        abilityDataRef[0].abilityButton.onClick.AddListener(RepositionAbility);
        abilityDataRef[1].abilityButton.onClick.AddListener(MovementAbility);
        abilityDataRef[2].abilityButton.onClick.AddListener(MoveHintAbility);
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
            abilityDataRef[i].abilityCount = num;
        }
        AbilityStatus();
        currentlevel = level;
        spawner = sp;
        currentAbilityActive = -1;
        CheckForSelectedShape();
    }

    private void CheckForSelectedShape()
    {
        if (spawner == null)
            return;
        if(currentAbilityActive >= 0 && spawner.CurrentSelectedShape != null)
        {
            spawner.CurrentSelectedShape.AbilityCodeSetup(currentAbilityActive);
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
            abilityDataRef[i].abilityButton.interactable = true;
        }
    }
    private void RepositionAbility()
    {
        currentAbilityActive = 0;
        CheckForSelectedShape();
    }
    private void MovementAbility()
    {
        currentAbilityActive = 1;
        CheckForSelectedShape();
    }
    private void MoveHintAbility()
    {
        currentAbilityActive = 2;
        CheckForSelectedShape();
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
            abilityDataRef[currentAbilityActive].abilityCount += num;
        }
        ResetActiveAbility();
    }
    private void ResetActiveAbility()
    {
        currentAbilityActive = -1;
        if (spawner.CurrentSelectedShape != null)
        {
            spawner.CurrentSelectedShape.AbilityCodeSetup(currentAbilityActive);
        }
        AbilityStatus();
    }
}
