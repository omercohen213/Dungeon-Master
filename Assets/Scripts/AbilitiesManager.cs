using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilitiesManager : MonoBehaviour
{
    // The array of all abilities
    private Ability[] abilities = new Ability[3];

    // References
    [SerializeField] Player player;
    [SerializeField] private Text aaCdText;
    [SerializeField] private Text ability1CdText;
    [SerializeField] private Text ability2CdText;
    [SerializeField] private Image aaCdImage;
    [SerializeField] private Image abilityCd1Image;
    [SerializeField] private Image abilityCd2Image;

    private Animator anim;

    // To avoid using two abilities at the same time
    public bool isAbilityInUse = false;
    

    void Start()
    {
        // Add the components we refer to
        anim = player.GetComponentInChildren<Animator>();
        for (int i = 0; i < abilities.Length; i++)
            abilities[i] = gameObject.AddComponent<Ability>();

        // Auto attack
        abilities[0].abilityName = "AA";
        abilities[0].cd = 0.5f;
        
        abilities[0].abilityCdText = aaCdText;
        abilities[0].abilityCdImage = aaCdImage;

        // Ability 1
        abilities[1].abilityName = "Ability1";
        abilities[1].cd = 5.0f;
        abilities[1].castTime = 2;
        abilities[1].abilityCdText = ability1CdText;
        abilities[1].abilityCdImage = abilityCd1Image;

        // Ability 2
        abilities[2].abilityName = "Ability2";
        abilities[2].cd = 10.0f;
        abilities[2].abilityCdText = ability2CdText;
        abilities[2].abilityCdImage = abilityCd2Image;

        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].abilityCdText.gameObject.SetActive(false);
            abilities[i].abilityCdImage.fillAmount = 0;
        }
    }

    void Update()
    {
        // Auto attack
        if (!isAbilityInUse && !abilities[0].isAbilityCd)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AbilityCast(abilities[0]);
                anim.SetTrigger(abilities[0].abilityName);
            }
        }
        else if (isAbilityInUse) {
            ApplyCooldown(abilities[0]);
            ApplyAbilityCastTime(abilities[0]);
        }
        else ApplyCooldown(abilities[0]);

            // Ability 1 (Z)
            if (!isAbilityInUse && !abilities[1].isAbilityCd)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    AbilityCast(abilities[1]);
                    anim.SetTrigger(abilities[1].abilityName);
                }

            }
            else ApplyCooldown(abilities[1]);

            // Ability 2 (X)
            if (!isAbilityInUse && !abilities[2].isAbilityCd)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    AbilityCast(abilities[2]);
                    anim.SetTrigger(abilities[2].abilityName);
                }
            }
            else ApplyCooldown(abilities[2]);
        

        /*else
        {
            for (int i = 0; i < abilities.Length; i++)
                if (abilities[i].isAbilityOnCast)
                    ApplyCooldown(abilities[i]);
        }*/
    }

    private void ApplyAbilityCastTime(Ability ability)
    {
        ability.castTimer -= Time.deltaTime;
        if (ability.castTimer < 0.0f)
        {
            isAbilityInUse = false;
            ability.isAbilityOnCast = false;
        }

        if (!ability.isAbilityOnCast)
        {
            ability.cdTimer = ability.cd;
            ability.castTimer = ability.castTime;
        }
    }

    // Apply cooldown for ability
    private void ApplyCooldown(Ability ability)
    {
        ability.cdTimer -= Time.deltaTime;
        

        if (ability.cdTimer < 0.0f)
        {
            ability.isAbilityCd = false;
            ability.abilityCdText.gameObject.SetActive(false);
            ability.abilityCdImage.fillAmount = 0.0f;

        }
        else
        {
            ability.abilityCdText.text = Mathf.RoundToInt(ability.cdTimer).ToString();
            ability.abilityCdImage.fillAmount = ability.cdTimer / ability.cd;
        }

        
    }

    // Attack animation and change boxCollider position on ability cast 
    public void AbilityCast(Ability ability)
    {
        if (ability.isAbilityCd)
        {
            // clicked during cd
        }
        else
        {
            isAbilityInUse = true;
            ability.isAbilityCd = true;
            ability.abilityCdText.gameObject.SetActive(true);
            
        }

        if (!ability.isAbilityOnCast)
        {
            ability.cdTimer = ability.cd;
            ability.castTimer = ability.castTime;
        }
    }
}







