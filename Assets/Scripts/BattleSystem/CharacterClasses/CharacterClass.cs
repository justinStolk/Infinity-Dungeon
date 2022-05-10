using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Infinity Dungeon/Class")]
public class CharacterClass : ScriptableObject
{
    [SerializeField] private string className;

    [Header("Base Stats")]
    [SerializeField] private int baseMaxHitPoints;
    [SerializeField] private int baseStrength;
    [SerializeField] private int baseDefense;
    [SerializeField] private int baseMagic;
    [SerializeField] private int baseResistance;
    [SerializeField] private int baseSpeed;
    [SerializeField] private int baseLuck;
    [SerializeField] private int movementRange;

    [Header("Growth Modifiers")]
    [SerializeField] private int hitPointGrowthModifier;
    [SerializeField] private int strengthGrowthModifier;
    [SerializeField] private int defenseGrowthModifier;
    [SerializeField] private int magicGrowthModifier;
    [SerializeField] private int resistanceGrowthModifier;
    [SerializeField] private int speedGrowthModifier;
    [SerializeField] private int luckGrowthModifier;

    public int HitPointGrowthModifier { get { return hitPointGrowthModifier; } }
    public int StrengthGrowthModifier { get { return strengthGrowthModifier; } }
    public int DefenseGrowthModifier { get { return defenseGrowthModifier; } }
    public int MagicGrowthModifier { get { return magicGrowthModifier; } }
    public int ResistanceGrowthModifier { get { return resistanceGrowthModifier; } }
    public int SpeedGrowthModifier { get { return speedGrowthModifier; } }
    public int LuckGrowthModifier { get { return luckGrowthModifier; } }
    public int MovementRange { get { return movementRange; } }

}
