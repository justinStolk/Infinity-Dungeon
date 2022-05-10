using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IAttacker, IDamageable, IExperience
{

    [Header("Stats")]
    [SerializeField] private new string name;
    [Min(1)]
    [SerializeField] private int level;
    [SerializeField] private int currentExperience;
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int strength;
    [SerializeField] private int defense;
    [SerializeField] private int magic;
    [SerializeField] private int resistance;
    [SerializeField] private int speed;
    [SerializeField] private int luck;

    [Header("Growth Modifiers")]
    [SerializeField] private int hitPointGrowthModifier;
    [SerializeField] private int strengthGrowthModifier;
    [SerializeField] private int defenseGrowthModifier;
    [SerializeField] private int magicGrowthModifier;
    [SerializeField] private int resistanceGrowthModifier;
    [SerializeField] private int speedGrowthModifier;
    [SerializeField] private int luckGrowthModifier;

    [SerializeField] private CharacterClass myClass;

    private int requiredExperience;

    public string Name { get { return name; } private set { name = value; } }
    public int Level { get { return level; } private set { level = value; } }
    public int CurrentExperience { get { return currentExperience; } private set { currentExperience = value; } }
    public int RequiredExperience { get { return requiredExperience; } private set { requiredExperience = value; } }
    public int MaxHitPoints { get { return maxHitPoints; } private set { maxHitPoints = value; } }
    public int HitPoints { get { return hitPoints; } private set { hitPoints = value; } }
    public int Strength { get { return strength; } private set { strength = value; } }
    public int Defense { get { return defense; } private set { defense = value; } }
    public int Magic { get { return magic; } private set { magic = value; } }
    public int Resistance { get { return resistance; } private set { resistance = value; } }
    public int Speed { get { return speed; } private set { speed = value; } }
    public int Luck { get { return luck; } private set { luck = value; } }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetExperience(int experienceEarned)
    {
        CurrentExperience += experienceEarned;
        if(CurrentExperience >= RequiredExperience)
        {
            CurrentExperience -= RequiredExperience;
            LevelUp();
        }
    
    }

    private void LevelUp()
    {
        Level += 1;
        IncreaseStats();
    }

    private void IncreaseStats()
    {
        
    }

    public void ChangeHealth(int amount)
    {
        HitPoints += amount;
        if(HitPoints > MaxHitPoints)
        {
            HitPoints = MaxHitPoints;
        }
        if(HitPoints < 0)
        {
            HitPoints = 0;
        }
    }

    public void Attack(IDamageable target)
    {
        target.ChangeHealth(0);
        Debug.LogWarning("Not implemented attack function! Review this at some point!");
    }
}
