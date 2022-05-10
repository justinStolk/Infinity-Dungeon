using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{

    public void OnSelected();
}

public interface IDamageable
{
    public int MaxHitPoints { get;  }
    public int HitPoints { get; }
    public int Defense { get; }
    public int Resistance { get; }
    public int Luck { get; }
    public int Speed { get; }
    public void ChangeHealth(int amount);

}

public interface IAttacker
{
    public int Strength { get; }
    public int Magic { get; }
    public int Luck { get; }
    public void Attack(IDamageable target);

}

public interface IExperience
{
    public int CurrentExperience { get; }
    public int RequiredExperience { get; }
    public void GetExperience(int experienceEarned);
}


