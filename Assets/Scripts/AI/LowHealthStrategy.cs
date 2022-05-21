using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowHealthStrategy : CombatStrategy
{
    public override Unit GetTargetThroughPolicy(List<Unit> potentialTargets)
    {
        Unit targetUnit = potentialTargets[0];
        int lowestHealth = targetUnit.HitPoints;
        foreach(Unit u in potentialTargets)
        {
            if (u.HitPoints < lowestHealth)
            {
                targetUnit = u;
                lowestHealth = u.HitPoints;
            }
        }
        return targetUnit;
    
    }

}
