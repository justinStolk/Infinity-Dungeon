using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : Item
{

    [SerializeField] protected int hitPointBoost;
    [SerializeField] protected int strengthBoost;
    [SerializeField] protected int defenseBoost;
    [SerializeField] protected int magicBoost;
    [SerializeField] protected int resistanceBoost;
    [SerializeField] protected int speedBoost;
    [SerializeField] protected int luckBoost;

    public int HitPointBoost { get { return hitPointBoost; } }
    public int StrengthBoost { get { return strengthBoost; } }
    public int DefenseBoost { get { return defenseBoost; } }
    public int MagicBoost { get { return magicBoost; } }
    public int ResistanceBoost { get { return resistanceBoost; } }
    public int SpeedBoost { get { return speedBoost; } }
    public int LuckBoost { get { return luckBoost; } }

    public override void OnItemUsed()
    {
        Equip();
    }
    public abstract void Equip();

}
