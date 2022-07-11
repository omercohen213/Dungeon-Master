using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability: MonoBehaviour
{
    public string abilityName;
    public float cd;
    public float cdTimer= 0;
    public bool isAbilityCd = false;
    public float castTime;
    public float castTimer= 0;
    public bool isAbilityOnCast= false;

    public Image abilityCdImage;
    public Text abilityCdText;  
}
