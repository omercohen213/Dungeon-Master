using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesManager : MonoBehaviour
{
    public static AbilitiesManager instance;

    // The array of all abilities
    private List<Ability> abilities;

    // References
    [SerializeField] Player player;
    private Animator weaponAnim;

    [SerializeField] private Transform abilityObjects;
    [SerializeField] private GameObject swordSwingPrefab;
    [SerializeField] private GameObject aatroxQ1Prefab;
    [SerializeField] private GameObject aatroxQ2Prefab;
    [SerializeField] private GameObject aatroxQ3Prefab;
    [SerializeField] private Vector3 aatroxQOffset;
    GameObject aatroxQObj;
    private int aatroxQPart = 1;
    public ContactFilter2D bodyFilter;
    public ContactFilter2D edgeFilter;
    private Collider2D bodyCollider;
    private Collider2D edgeCollider;
    private Collider2D[] bodyHits = new Collider2D[10];
    private Collider2D[] edgeHits = new Collider2D[10];
    private bool hitBody;
    private bool hitEdge;
    private readonly float aatroxQHitVFXScale = 0.2f;
    private readonly float enemyKnockUpDuration = 0.25f;
    private readonly float enemyKnockUpDistance = 0.05f;
    private readonly float playeKnockUpDuration = 0.5f;
    private readonly float playerKnockUpDistance = 0.1f;

    // To avoid using two abilities at the same time
    private bool disableAll = false;
    private float disableAllTimer = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        weaponAnim = player.GetComponentInChildren<Animator>();
        abilities = GetComponentsInChildren<Ability>().ToList();
        // Initialize texts and images
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].abilityCdText.gameObject.SetActive(false);
            abilities[i].abilityCdImage.fillAmount = 0;
        }
        SetDefaultKeyBinds(); // (Can change keybinds through inspector)
    }

    void Update()
    {
        CheckAbilityUse();
    }

    // Set abilities keybinds
    private void SetDefaultKeyBinds()
    {
        abilities[0].keyCode = KeyCode.Space;
        abilities[1].keyCode = KeyCode.Z;
        abilities[2].keyCode = KeyCode.X;
        abilities[3].keyCode = KeyCode.C;
        abilities[4].keyCode = KeyCode.V;
    }

    private void CheckAbilityUse()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            Ability ability = abilities[i];
            if (!ability.isCd && !disableAll) // Ability is not on cd and no ability animation is active
            {
                if (Input.GetKeyDown(ability.keyCode))
                {
                    MethodInfo methodInfo = GetType().GetMethod(ability.abilityName);
                    if (methodInfo != null)
                        methodInfo.Invoke(this, new object[] { ability }); // Call ability method    
                    if (CheckTriggerExists(ability.abilityName))
                    {
                        weaponAnim.SetTrigger(ability.abilityName);
                    }
                    DisableAbilityUse(ability);
                }
            }
            else if (ability.isCd) // Ability on cd
                ApplyCooldown(ability);
        }
    }
    public void AutoAttack(Ability ability)
    {
        SwingWeapon(ability, Color.white);
    }

    public void SwingWeapon(Ability ability, Color color)
    {
        Vector3 pos = player.transform.Find("Weapon").position;
        GameObject swordSwing = Instantiate(swordSwingPrefab, pos, Quaternion.identity, abilityObjects);
        swordSwing.transform.localScale = player.transform.localScale;
        Image swordSwingImage = swordSwing.GetComponent<Image>();
        swordSwingImage.fillAmount = 0.0f;
        swordSwingImage.color = color;
        StartCoroutine(UpdateFillAmount(swordSwingImage, ability));
    }

    public void AatroxQ(Ability ability)
    {
        GameObject abilityObjectPrefab = null;
        switch (aatroxQPart)
        {
            case 1: abilityObjectPrefab = aatroxQ1Prefab; break;
            case 2: abilityObjectPrefab = aatroxQ2Prefab; break;
            case 3: abilityObjectPrefab = aatroxQ3Prefab; break;
        }

        Quaternion rotation = Quaternion.identity;
        switch (player.FacingDirection)
        {
            case "Right":
            default:
                aatroxQOffset = new Vector3(0.2f, -0.1f);
                break;
            case "Left":
                rotation = Quaternion.Euler(0, 0, 180);
                aatroxQOffset = new Vector3(-0.2f, 0f);
                break;
            case "Up":
                rotation = Quaternion.Euler(0, 0, 90);
                aatroxQOffset = new Vector3(0.03f, 0.2f);
                break;
            case "Down":
                rotation = Quaternion.Euler(0, 0, 270);
                aatroxQOffset = new Vector3(-0.03f, -0.2f);
                break;
        }
        SwingWeapon(ability, Color.red);
        Vector3 pos = player.transform.position + aatroxQOffset;

        player.GetKnockUp(playeKnockUpDuration, playerKnockUpDistance);
        StartCoroutine(WaitForHit(ability, abilityObjectPrefab, pos, rotation));

        if (aatroxQPart == 3)
        {
            aatroxQPart = 1;
            ability.cd = 7f;
        }
        else
        {
            aatroxQPart++;
            ability.cd = 2f;
        }
    }

    public IEnumerator WaitForHit(Ability ability, GameObject abilityObjectPrefab, Vector3 pos, Quaternion rotation)
    {
        yield return new WaitForSeconds(ability.animationTime / 3);
        aatroxQObj = Instantiate(abilityObjectPrefab, pos, rotation, abilityObjects);

        hitBody = false;
        hitEdge = false;
        bodyCollider = aatroxQObj.transform.Find("Body").GetComponent<Collider2D>();
        edgeCollider = aatroxQObj.transform.Find("Edge").GetComponent<Collider2D>();
        float rnd = UnityEngine.Random.Range(0.8f, 1);
        int bodyDamage = Mathf.RoundToInt(player.AbilityPower * rnd);
        int edgeDamage = Mathf.RoundToInt(player.AbilityPower * 3f * rnd);
        Vector3 bodyOrigin = aatroxQObj.transform.Find("Body").position;
        Vector3 edgeOrigin = aatroxQObj.transform.Find("Edge").position;
        float pushForce = 0f;

        edgeCollider.OverlapCollider(edgeFilter, edgeHits);
        for (int i = 0; i < edgeHits.Length; i++)
        {
            if (edgeHits[i] == null)
                continue;
            bool isEdge = true;
            OnHit(edgeHits[i], edgeDamage, pushForce, edgeOrigin, isEdge);

            // Array is not cleaned up so we do it by ourselves
            edgeHits[i] = null;
        }
        if (hitEdge)
        {
            yield return new WaitForSeconds(ability.timeBeforeDestroyed);
            Destroy(aatroxQObj);
            yield break;
        }

        bodyCollider.OverlapCollider(bodyFilter, bodyHits);
        for (int i = 0; i < bodyHits.Length; i++)
        {
            if (bodyHits[i] == null)
                continue;
            bool isEdge = false;
            OnHit(bodyHits[i], bodyDamage, pushForce, bodyOrigin, isEdge);

            // Array is not cleaned up so we do it by ourselves
            bodyHits[i] = null;
        }
        if (hitBody)
        {
            Debug.Log("hitBody");
        }
        yield return new WaitForSeconds(ability.timeBeforeDestroyed);
        Destroy(aatroxQObj);
    }

    private void OnHit(Collider2D coll, int damage, float pushForce, Vector3 origin, bool isEdge)
    {
        if (coll.CompareTag("Fighter"))
        {
            if (coll.name == "Player")
                return;
            
            //IDamageable damageable = coll.gameObject.GetComponent<IDamageable>();
            Fighter fighter = coll.GetComponent<Fighter>();
            fighter.ShowHitVFX(coll.transform.position, aatroxQHitVFXScale, aatroxQObj.transform);
            fighter.ReceiveDamage(damage, pushForce, origin);
            if (isEdge)
            {
                hitEdge = true;
                if (!fighter.IsDamageToKill(damage))
                {
                    fighter.GetKnockUp(enemyKnockUpDuration, enemyKnockUpDistance);
                }
            }
        }
    }

    private bool CheckTriggerExists(string triggerName)
    {
        foreach (AnimatorControllerParameter param in weaponAnim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && param.name == triggerName)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator UpdateFillAmount(Image image, Ability ability)
    {
        float timeElapsed = 0.0f;
        float duration = ability.animationTime / 2f;

        while (timeElapsed < duration)
        {
            float fillAmount = timeElapsed / duration;
            image.fillAmount = fillAmount;

            yield return null;

            timeElapsed += Time.deltaTime;
        }

        // Fill amount is now 1.0f, destroy the swordSwing object
        Destroy(image.gameObject);
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

    public bool IsAbilityActive(string abilityName)
    {
        Ability ability = abilities.Find(ability => abilityName == ability.abilityName);
        return ability.isAnimationActive;
    }

}





