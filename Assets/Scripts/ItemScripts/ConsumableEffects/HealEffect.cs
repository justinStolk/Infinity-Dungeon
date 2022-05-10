using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Infinity Dungeon/Consumable Effects/Heal Effect")]
public class HealEffect : ConsumableEffect
{
    [SerializeField] private int healAmount;
    public override void OnConsumableUse(Unit targetUnit)
    {
        targetUnit.ChangeHealth(healAmount);
        //Heal by healAmount
    }
}
