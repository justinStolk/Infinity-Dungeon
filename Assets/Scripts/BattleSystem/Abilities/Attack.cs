using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : MonoBehaviour
{
    public enum AttackType { PHYSICAL, MAGIC }
    public AttackType attackType;

    public void AttackTarget(IStats attacker, IDamageable defender)
    {
        switch (attackType)
        {
            case AttackType.PHYSICAL:
                int totalPower = attacker.Strength;
                //foreach(Equipment e in attacker.)

                break;
            case AttackType.MAGIC:

                break;


        }
    }
}
