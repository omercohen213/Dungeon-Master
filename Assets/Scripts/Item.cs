using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "ScriptableObjects", menuName ="ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string type;
    public int requiredLvl;
    public Sprite inverntorySprite;   
}
