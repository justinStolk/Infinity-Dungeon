using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Infinity Dungeon/Items/Weapon")]
public class Weapon : Equipment
{
    public enum WeaponType { Sword, Spear, Axe, Bow, Rod }
    public WeaponType weaponType;
}
