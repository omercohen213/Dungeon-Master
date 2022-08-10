using UnityEngine;
using System;


[Serializable]
[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/Armor")]
public class Armor : Item
{
    public int defense;
}
