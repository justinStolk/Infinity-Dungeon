using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{

    public void OnSelected();
}

public interface IDamageable : IHealth, IDefense, IResistance, ILuck, ISpeed
{
    public void ChangeHealth(int amount);

}

public interface IAttacker : IStrength, IMagic, ILuck
{
    public void Attack(IDamageable target);

}

public interface IExperience
{
    public int CurrentExperience { get; }
    public int RequiredExperience { get; }
    public void GetExperience(int experienceEarned);
}

public interface IStats : IHealth, IStrength, IDefense, IMagic, IResistance, ISpeed, ILuck
{
    public int[] GetAllStats();
}
public interface IHealth
{
    public int MaxHitPoints { get; }
    public int HitPoints { get; }
}
public interface IStrength
{
    public int Strength { get; }
}
public interface IDefense
{
    public int Defense { get; }
}
public interface IMagic
{
    public int Magic { get; }
}
public interface IResistance
{
    public int Resistance { get; }
}
public interface ILuck
{
    public int Luck { get; }
}
public interface ISpeed
{
    public int Speed { get; }
}



