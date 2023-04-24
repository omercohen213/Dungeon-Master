using UnityEngine;
using System;


[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/Weapon"), Serializable]
public class Weapon : Item
{
    public int attackPower;
    public int range;
    public int pushForce;
   
}