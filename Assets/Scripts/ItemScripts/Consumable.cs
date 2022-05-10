using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Infinity Dungeon/Items/Consumable")]
public class Consumable : Item
{
    public Unit TargetUnit;
    [SerializeField] private ConsumableEffect[] consumableEffects;
    

    public override void OnItemUsed()
    {
        UseConsumable();
    }

    private void UseConsumable()
    {
        foreach(ConsumableEffect effect in consumableEffects)
        {
            effect.OnConsumableUse(TargetUnit);
        }
    }
}
