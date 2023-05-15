using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability: MonoBehaviour
{
    public string abilityName;
    public float cd;
    public float cdTimer= 0;
    public bool isCd = false;
    public float animationTime;
    public bool isAnimationActive = false;
    public float timeBeforeDestroyed;
    public KeyCode keyCode;
    public Image abilityCdImage;
    public Text abilityCdText;

}
