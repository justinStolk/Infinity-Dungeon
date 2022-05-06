using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Item : ScriptableObject
{

    [SerializeField] protected new string name;
    [SerializeField] protected Sprite icon;

    public abstract void OnItemUsed();

}
