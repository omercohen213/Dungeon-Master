using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/Weapon")]
public class Weapon : Item
{
    public int minDmg;
    public int maxDmg;
    public int range;
    public int pushForce;
    
}
