using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilitiesManager : MonoBehaviour
{
    public static AbilitiesManager instance;

    // The array of all abilities
    private Ability[] abilities = new Ability[3];

    // References
    [SerializeField] Player player;
    private Animator anim;

    // To avoid using two abilities at the same time
    private bool disableAll = false;
    private float disableAllTimer = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        anim = player.GetComponentInChildren<Animator>();
        abilities = GetComponentsInChildren<Ability>();

        // Initialize texts and images
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].abilityCdText.gameObject.SetActive(false);
            abilities[i].abilityCdImage.fillAmount = 0;
        }
    }

    void Update()
    {
        // Can use an interface(?)

        // Auto attack
        Ability AA = abilities[0];
        if (!AA.isCd && !disableAll) // Ability is not on cd and no ability animation is active
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger(AA.abilityName); // Animate
                DisableAbilityUse(AA);
            }
        }
        else if (AA.isCd) // Ability on cd
            ApplyCooldown(AA);


        // Ability 1 (Z)
        Ability ability1 = abilities[1];
        if (!ability1.isCd && !disableAll) // Ability is not on cd and no other ability animation is active
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                //Debug.Log("before " + disableAll);
                anim.SetTrigger(ability1.abilityName); // Animate
                DisableAbilityUse(ability1);
            }
        }
        else if (ability1.isCd) // Ability on cd
            ApplyCooldown(ability1);
         


        // Ability 2 (X)
        Ability ability2 = abilities[2];
        if (!ability2.isCd && !disableAll) // ability is not on cd and no ability animation is active
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                anim.SetTrigger(ability2.abilityName); // Animate
                DisableAbilityUse(ability2);
            }
        }
        else if (ability2.isCd)
         // Ability on cd
            ApplyCooldown(ability2);                  
        }

    // Apply cooldown for ability
    private void ApplyCooldown(Ability ability)
    {
        if (ability.isAnimationActive)
        {
            disableAllTimer -= Time.deltaTime;
            if (disableAllTimer < 0) // Animation has ended and we can use another ability
            {             
                ability.isAnimationActive = false;
                disableAll = false;
                disableAllTimer = 0;
            }
        }
        // Reduce cd till it reaches 0 so we can use it again
        ability.cdTimer -= Time.deltaTime;

        // Cd is over
        if (ability.cdTimer < 0)
        {
            ability.isCd = false;
            ability.cdTimer = 0;
            ability.abilityCdText.gameObject.SetActive(false);
            ability.abilityCdImage.fillAmount = 0.0f;
        }
        // Still on cd
        else
        {
            ability.abilityCdText.text = Mathf.RoundToInt(ability.cdTimer).ToString();
            ability.abilityCdImage.fillAmount = ability.cdTimer / ability.cd;
        }
    }

    // Disable this ability use for cd time, disable all abilties for animation time
    public void DisableAbilityUse(Ability ability)
    {
        // To avoid using 2 abilities at the same time     
        ability.isAnimationActive = true;
        disableAllTimer = ability.animationTime;
        disableAll = true;

        ability.isCd = true;
        ability.abilityCdText.gameObject.SetActive(true);
        ability.cdTimer = ability.cd;
    }

    public bool isAaActive()
    {
        return abilities[0].isAnimationActive;
    }
    public bool isAbility1Active()
    {
        return abilities[1].isAnimationActive;

    }
    public bool isAbility2Active()
    {
        return abilities[2].isAnimationActive;
    }
}







