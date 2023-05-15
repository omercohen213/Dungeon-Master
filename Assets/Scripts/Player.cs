using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Fighter
{
    public static Player instance;
    private HUD hud;
    private GameManager gameManager;

    // Resources
    private int lvl;
    private int xp;
    private int gold;
    private int hp;
    private int maxHp;
    private int mp;
    private int maxMp;
    private int attackPower;
    private int abilityPower;
    private int defense;
    private int magicResist;
    private float critChance;
    private int abilityPoints;
    private int attributePoints;
    private string playerName;
    public int Lvl { get => lvl; set => lvl = value; }
    public int Xp { get => xp; set => xp = value; }
    public int Gold { get => gold; set => gold = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Mp { get => mp; set => mp = value; }
    public int MaxMp { get => maxMp; set => maxMp = value; }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public int AbilityPower { get => abilityPower; set => abilityPower = value; }
    public int Defense { get => defense; set => defense = value; }
    public int MagicResist { get => magicResist; set => magicResist = value; }
    public float CritChance { get => critChance; set => critChance = value; }
    public int AbilityPoints { get => abilityPoints; set => abilityPoints = value; }
    public int AttributePoints { get => attributePoints; set => attributePoints = value; }
    public string PlayerName { get => playerName; set => playerName = value; }

    // Hp and Mp regeneration
    private readonly int hpRegen = 10;
    private readonly int mpRegen = 5;
    private readonly float regenDelay = 3f;
    private readonly float inCombatDelay = 5f;
    private bool isInCombat = false;
    public bool IsInCombat { set => isInCombat = value; }
    private const float DAMAGE_DELAY_TIME = 0.1f;

    private Weapon weapon;
    private Armor armor;
    private Helmet helmet;
    public Weapon Weapon { get => weapon; set => weapon = value; }
    public Armor Armor { get => armor; set => armor = value; }
    public Helmet Helmet { get => helmet; set => helmet = value; }

    private List<Quest> activeQuests;
    public List<Quest> ActiveQuests { get => activeQuests; set => activeQuests = value; }

    // Player Name
    private Camera cam;
    private GameObject playerNameGo;
    private readonly float nameOffset = -0.12f;

    public GameObject deathScreenPrefab;
    public float deathTimer = 3.0f;

    private GameObject shadowGo;

    private float speed = 2;
    private string facingDirection;
    public string FacingDirection { get => facingDirection; set => facingDirection = value; }

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        cam = Camera.main;
        hud = HUD.instance;
        gameManager = GameManager.instance;
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerNameGo = transform.Find("PlayerNameCanvas/PlayerName").gameObject;
        playerNameGo.GetComponent<Text>().text = playerName;
        shadowGo = transform.Find("Shadow").gameObject;
        StartCoroutine(RegenerateCoroutine());
        damageDelay = DAMAGE_DELAY_TIME;
    }

    private void Update()
    {
        // arrow keys (returns 1/-1 on key down)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        UpdateFacingDirection(horizontalInput, verticalInput);
        UpdateMotor(new Vector3(horizontalInput, verticalInput, 0), speed);
    }
    public void Initialize()
    {
        activeQuests = new List<Quest>();
    }

    private IEnumerator RegenerateCoroutine()
    {
        while (true)
        {
            if (isInCombat)
            {
                yield return new WaitForSeconds(inCombatDelay);
                isInCombat = false; // set isInCombat back to false after inCombatDelay
            }
            else
            {
                bool shouldRegenerateHp = hp < maxHp;
                bool shouldRegenerateMp = mp < maxMp;

                if (shouldRegenerateHp)
                {
                    // Regenerate HP
                    hp += hpRegen;
                    if (hp > maxHp)
                    {
                        hp = maxHp;
                    }
                    hud.onHpChange();
                }

                if (shouldRegenerateMp)
                {
                    // Regenerate MP
                    mp += mpRegen;
                    if (mp > maxMp)
                    {
                        mp = maxMp;
                    }
                    hud.onMpChange();
                }

                // Wait for the next regeneration tick
                yield return new WaitForSeconds(regenDelay);
            }
        }

    }

    private void LateUpdate()
    {
        Vector3 namePos = cam.WorldToScreenPoint(transform.position + new Vector3(0, nameOffset));
        if (playerNameGo.transform.position != namePos)
            playerNameGo.transform.position = namePos;
    }
    private void UpdateFacingDirection(float horizontal, float vertical)
    {
        Vector2 inputDirection = new Vector2(horizontal, vertical).normalized;

        if (inputDirection.magnitude == 0)
        {
            return;
        }

        if (Mathf.Abs(inputDirection.x) >= Mathf.Abs(inputDirection.y))
        {
            if (inputDirection.x > 0)
                facingDirection = "Right";
            else
                facingDirection = "Left";
        }
        else
        {
            if (inputDirection.y > 0)
                facingDirection = "Up";
            else
                facingDirection = "Down";
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        cam = Camera.main;
    }

    public void GrantGold(int amount)
    {
        Gold += amount;
        string text = "+" + amount + " gold!";
        int fontSize = 20;
        float destroyTimer = 1.5f;
        FloatingTextManager.instance.ShowFloatingText(text, fontSize, Color.yellow, transform.position, "GetResource", destroyTimer);
        GameManager.instance.SaveGame();
        hud.onGoldChange();
    }

    public void GrantXp(int xpAmount)
    {
        Xp += xpAmount;
        if (Xp > gameManager.XpToLevelUp(Lvl))
        {
            Xp = 0;
            OnLevelUp();
        }
        string text = "+" + xpAmount + "xp";
        int fontSize = 20;
        float destroyTimer = 1.5f;
        FloatingTextManager.instance.ShowFloatingText(text, fontSize, Color.magenta, transform.position, "GetResource", destroyTimer);
        gameManager.SaveGame();
        hud.onXpChange();
    }

    public void OnLevelUp()
    {
        // To not instantly show lvl up text on load 
        if (Time.time > 1)
            FloatingTextManager.instance.ShowFloatingText("Level Up!", 30, Color.magenta, transform.position, "GetResource", 1.5f);
        Lvl++;
        Hp = MaxHp;
        AbilityPoints++;
        AttributePoints += 3;
        hud.onHpChange();
        hud.onLevelChange();
    }

    // Receive damage
    public override void ReceiveDamage(int damageAmount, float pushForce, Vector3 origin)
    {
        isInCombat = true;

        if (Time.time - lastDamage > damageDelay)
        {
            lastDamage = Time.time;
            if (damageAmount > 0)
            {
                Hp -= damageAmount;
                FloatingTextManager.instance.ShowFloatingText(damageAmount.ToString(), 30, Color.red, origin, "Hit", 2.0f);
            }
            else FloatingTextManager.instance.ShowFloatingText("0", 30, Color.red, origin, "Hit", 2.0f);
            pushDirection = (transform.position - origin).normalized * pushForce;

            if (Hp <= 0)
            {
                Hp = 0;
                Death();
            }
        }
        hud.onHpChange();
    }
    // Return the total amount of defense including items
    public int GetTotalDefense()
    {
        if (armor != null && helmet != null)
            return defense + armor.defense + helmet.defense;
        else if (armor == null && helmet != null)
            return defense + helmet.defense;
        else if (armor != null && helmet == null)
            return defense + armor.defense;
        else return defense;
    }
    // Return the total amount of attack power including items
    public int GetTotalAttackPower()
    {
        return attackPower + weapon.attackPower;
    }
    // Initialize needed variables on game start
    public override void Death()
    {
        // Show the death screen
        GameObject deathScreen = Instantiate(deathScreenPrefab);
        deathScreen.transform.SetParent(GameObject.Find("Canvas").transform, false);
        deathScreen.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        // Show the respawn text
        GameObject respawnText = new GameObject("RespawnText");
        respawnText.transform.SetParent(deathScreen.transform, false);
        Text textComponent = respawnText.AddComponent<Text>();
        textComponent.text = "Respawning in " + deathTimer.ToString("0") + "...";
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.color = Color.red;
        textComponent.alignment = TextAnchor.MiddleCenter;
        RectTransform textTransform = respawnText.GetComponent<RectTransform>();
        textTransform.sizeDelta = new Vector2(400.0f, 50.0f);
        textTransform.anchoredPosition = new Vector2(0.0f, 0.0f);

        // Start the respawn timer
        StartCoroutine(RespawnCoroutine(deathScreen, textComponent));
    }

    private IEnumerator RespawnCoroutine(GameObject deathScreen, Text respawnText)
    {
        float timer = 0.0f;
        while (timer < deathTimer)
        {
            timer += Time.deltaTime;
            respawnText.text = "Respawning in " + (deathTimer - timer).ToString("0") + "...";
            yield return null;
        }

        // Destroy the death screen and respawn the player
        Destroy(deathScreen);
        SpawnPlayer();
        hp = maxHp;
        hud.onHpChange();
    }

    public void SpawnPlayer()
    {
        // Spawn point
        RectTransform portalRectTransform = GameObject.Find("SpawnPoint").GetComponent<RectTransform>();
        Transform portal = GameObject.Find("SpawnPoint").transform;
        float portalWidth = portalRectTransform.rect.width * 0.16f;
        float portalHeight = portalRectTransform.rect.height * 0.16f;
        transform.position = portal.position + new Vector3(portalWidth, -portalHeight / 3, 0);
    }

    public override void GetKnockUp(float duration, float distance)
    {
        StartCoroutine(GetKnockUpCoroutine(duration, distance));
    }

    private IEnumerator GetKnockUpCoroutine(float duration, float distance)
    {
        float startTime = Time.time;
        float peakTime = startTime + (duration / 2f);
        float endTime = startTime + duration;

        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + (Vector3.up * distance);
        Vector3 endPosition = startPosition;

        Vector3 shadowPos = shadowGo.transform.position;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            if (Time.time <= peakTime)
            {
                transform.position = Vector3.Lerp(startPosition, peakPosition, t * 2f);
                float shadowScale = Mathf.Lerp(0.05f, 0.01f, t * 2f);
                shadowGo.transform.localScale = new Vector3(shadowScale, shadowScale / 5, 1f);
                shadowGo.transform.position = shadowPos;
            }
            else
            {
                transform.position = Vector3.Lerp(peakPosition, endPosition, (t - 0.5f) * 2f);
                float shadowScale = Mathf.Lerp(0.01f, 0.05f, (t - 0.5f) * 2f);
                shadowGo.transform.localScale = new Vector3(shadowScale, shadowScale / 5, 1f);
                shadowGo.transform.position = shadowPos;
            }
            yield return null;
        }
    }

    // Check if given damage is enough to kill player
    public override bool IsDamageToKill(float damage)
    {
        return damage > hp;
    }
}
