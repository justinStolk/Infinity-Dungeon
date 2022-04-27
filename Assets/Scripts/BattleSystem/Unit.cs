using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private new string name;
    [SerializeField] private int level;
    [SerializeField] private int currentExperience;
    [SerializeField] private int requiredExperience;
    [SerializeField] private int hitPoints;
    [SerializeField] private int strength;
    [SerializeField] private int defense;
    [SerializeField] private int magic;
    [SerializeField] private int resistance;
    [SerializeField] private int speed;
    [SerializeField] private int luck;

    public string Name { get { return name; } private set { name = value; } }
    public int Level { get { return level; } private set { level = value; } }
    public int Exp { get { return currentExperience; } private set { currentExperience = value; } }
    public int ExpNeeded { get { return requiredExperience; } private set { requiredExperience = value; } }
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
        Exp += experienceEarned;
        if(Exp >= requiredExperience)
        {
            Exp -= requiredExperience;
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
}
