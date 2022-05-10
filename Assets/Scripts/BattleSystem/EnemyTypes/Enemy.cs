using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IAttacker, IDamageable
{
    [Header("Stats")]
    [SerializeField] protected new string name;
    [SerializeField] protected int level;
    [SerializeField] protected int maxHitPoints;
    [SerializeField] protected int hitPoints;
    [SerializeField] protected int strength;
    [SerializeField] protected int defense;
    [SerializeField] protected int magic;
    [SerializeField] protected int resistance;
    [SerializeField] protected int speed;
    [SerializeField] protected int luck;
    [Min(1)]
    [SerializeField] protected Vector2Int floorSpawnRange = Vector2Int.one;
    public int MaxHitPoints { get { return maxHitPoints; } protected set { maxHitPoints = value; } }
    public int HitPoints { get { return hitPoints; } protected set { hitPoints = value; } }
    public int Strength { get { return strength; } protected set { strength = value; } }
    public int Defense { get { return defense; } protected set { defense = value; } }
    public int Magic { get { return magic; } protected set { magic = value; } }
    public int Resistance { get { return resistance; } protected set { resistance = value; } }
    public int Speed { get { return speed; } protected set { speed = value; } }
    public int Luck { get { return luck; } protected set { luck = value; } }

    public Vector2Int FloorSpawnRange { get { return floorSpawnRange; } }


    public void Attack(IDamageable target)
    {
        throw new System.NotImplementedException();
    }

    public void ChangeHealth(int amount)
    {
        HitPoints += amount;
        if (HitPoints > MaxHitPoints)
        {
            HitPoints = MaxHitPoints;
        }
        if (HitPoints < 0)
        {
            HitPoints = 0;
        }
    }

}
