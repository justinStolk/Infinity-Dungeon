using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Infinity Dungeon/Items/Consumable")]
public class Consumable : Item
{
    [SerializeField] private ConsumableEffect[] consumableEffects;

    public void UseConsumable()
    {
        foreach(ConsumableEffect effect in consumableEffects)
        {
            effect.OnConsumableUse();
        }
    }
}
