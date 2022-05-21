using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatStrategy : MonoBehaviour
{
    public abstract Unit GetTargetThroughPolicy(List<Unit> potentialTargets);

}
