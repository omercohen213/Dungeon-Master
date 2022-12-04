using UnityEngine;
using System;


[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/Weapon"), Serializable]
public class Weapon : Item
{
    public int minDmg;
    public int maxDmg;
    public int range;
    public int pushForce;
   
}