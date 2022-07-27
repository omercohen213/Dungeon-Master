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
    private Animator anim;

    // To avoid using two abilities at the same time
    private bool isAnimationActive = false;
    private float disableTimer = 0;


    void Start()
    {
        anim = player.GetComponentInChildren<Animator>();
        abilities = GetComponents<Ability>();

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
        if (!abilities[0].isCd && !isAnimationActive) // Ability is not on cd and no ability animation is active
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger(abilities[0].abilityName); // Animate
                DisableAbility(abilities[0]);              
            }
        }
        else if (abilities[0].isCd) // Ability on cd
            ApplyCooldown(abilities[0]);


        // Ability 1 (Z)
        if (!abilities[1].isCd && !isAnimationActive) // Ability is not on cd and no ability animation is active
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                anim.SetTrigger(abilities[1].abilityName); // Animate
                DisableAbility(abilities[1]);              
            }
        }
        else if (abilities[1].isCd) // Ability on cd
            ApplyCooldown(abilities[1]);


        // Ability 2 (X)
        if (!abilities[2].isCd && !isAnimationActive) // ability is not on cd and no ability animation is active
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                anim.SetTrigger(abilities[2].abilityName); // Animate
                DisableAbility(abilities[2]);               
            }
        }
        else if (abilities[2].isCd) // Ability on cd
            ApplyCooldown(abilities[2]);
    }

    // Apply cooldown for ability
    private void ApplyCooldown(Ability ability)
    {
        if (isAnimationActive)
        {
            disableTimer -= Time.deltaTime;
            if (disableTimer < 0)
            {
                isAnimationActive = false;
                disableTimer = 0;
            }
        }
            // Reduce cd till it reaches 0 so we can use it again
            ability.cdTimer -= Time.deltaTime;

        if (ability.cdTimer < 0)
        {
            
            ability.isCd = false;
            ability.cdTimer = 0;
            ability.abilityCdText.gameObject.SetActive(false);
            ability.abilityCdImage.fillAmount = 0.0f;
        }
        else
        {
            ability.abilityCdText.text = Mathf.RoundToInt(ability.cdTimer).ToString();
            ability.abilityCdImage.fillAmount = ability.cdTimer / ability.cd;
        }
    }

    // Disable this ability for cd time, disable all abilties for animation time
    public void DisableAbility(Ability ability)
    {
        // To avoid using 2 abilities at the same time
        isAnimationActive = true;
        disableTimer = ability.animationTime;

        ability.isCd = true;
        ability.abilityCdText.gameObject.SetActive(true);
        ability.cdTimer = ability.cd;
    }

}







