using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ConsumableEffect : ScriptableObject
{
    public abstract void OnConsumableUse(Unit target);

}
