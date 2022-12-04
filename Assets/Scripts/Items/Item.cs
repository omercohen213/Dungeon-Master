using UnityEngine;
using System;


[Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public string type;
    public int requiredLvl;
    public Sprite inverntoryIcon;
    public Sprite itemSprite;
    public Vector3 spriteSize;
    public int price;
    public float dropRate; // Percentage; In range 0-100
}
