using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    // Logic
    private Player player;
    [SerializeField] private List<NPC> npcs;
    [SerializeField] private GameObject quests;
    private bool showFloatingText = true; // Quest Completion floating text

    // QuestView objects
    [SerializeField] private GameObject QuestsExitButton;
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Transform content;

    // QuestInfo objects
    [SerializeField] private GameObject questInfo;
    [SerializeField] private Text QuestInfoNameText;
    [SerializeField] private Text QuestInfoText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    [SerializeField] private Button giveUpButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button turnInButton;

    // ActiveQuests objects
    [SerializeField] private Transform activeQuestFloatingText;
    [SerializeField] private GameObject activeQuestPrefab;
    [SerializeField] private GameObject activeQuests;
    [SerializeField] private Text activeQuestName;
    [SerializeField] private Text activeQuestProgress;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = Player.instance;
        if (player.ActiveQuests.Count == 0)
        {
            foreach (NPC npc in npcs) { 
                npc.SetNpcFloatingText("!","NoQuest");
            }
        }
        activeQuests.SetActive(false);
        quests.gameObject.SetActive(false);
        questInfo.gameObject.SetActive(false);
    }

    // Open quests on npc interaction
    public void OnNpcInteract(NPC npc)
    {
        UpdateQuests(npc);
        quests.gameObject.SetActive(true);
        questInfo.gameObject.SetActive(false);
    }

    public void UpdateQuests(NPC npc)
    {
        // If quests objects don't exist- create them
        if (content.childCount == 0)
        {
            for (int i = 0; i < npc.quests.Count; i++)
            {
                GameObject questGo = Instantiate(questPrefab, content);
                questGo.name = "Quest" + i.ToString();

                Button questButton = content.transform.GetChild(i).GetComponent<Button>();
                Text questName = content.transform.GetChild(i).Find("QuestName").GetComponent<Text>();
                Quest quest = npc.quests[i];

                questName.text = quest.name;
                int temp = i;
                questButton.onClick.AddListener(() => OnQuestClick(npc.quests[temp], npc));

                if (player.Lvl < quest.requiredLvl)
                {
                    questName.color = Color.red;
                    questButton.onClick.RemoveAllListeners();
                }
            }
        }
        // Update quest objects
        else
        {
            for (int i = 0; i < npc.quests.Count; i++)
            {
                Button questButton = content.transform.GetChild(i).GetComponent<Button>();
                Text questName = content.transform.GetChild(i).Find("QuestName").GetComponent<Text>();
                Quest quest = npc.quests[i];
                if (player.Lvl < quest.requiredLvl)
                {
                    questName.color = Color.red;
                    questButton.onClick.RemoveAllListeners();
                }
                // Change quest name when in progress
                else if (isQuestInProgress(quest))
                    questName.text = quest.name + " - in progress";
                else questName.text = quest.name;
            }
        }
    }

    // Show quest info on quest click
    private void OnQuestClick(Quest quest, NPC npc)
    {
        questInfo.SetActive(true);

        QuestInfoNameText.text = quest.name + "\n";
        QuestInfoText.text = quest.description + "\n";


        // Create new listeners according to quest click
        if (!isQuestInProgress(quest)) // Not in progress: accept/decline
        {
            acceptButton.onClick.RemoveAllListeners(); // Clear other listeners on this button
            acceptButton.onClick.AddListener(() => AcceptQuest(quest, npc));
            declineButton.onClick.RemoveAllListeners();
            declineButton.onClick.AddListener(() => DeclineQuest());
        }
        else
        {
            if (quest.isCompleted()) // In progress and completed: back/turn in!/give up
            {
                turnInButton.gameObject.SetActive(true);
                giveUpButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                acceptButton.gameObject.SetActive(false);
                declineButton.gameObject.SetActive(false);
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(() => backFromQuest());
                turnInButton.onClick.RemoveAllListeners(); // Clear other listeners on this button
                turnInButton.onClick.AddListener(() => TurnInQuest(quest, npc));
                giveUpButton.onClick.RemoveAllListeners();
                giveUpButton.onClick.AddListener(() => GiveUpQuest(quest, npc));
            }
            else // In progress but not completed: back/give up
            {
                //turnInButton.gameObject.SetActive(false);
                giveUpButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                acceptButton.gameObject.SetActive(false);
                declineButton.gameObject.SetActive(false);
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(() => backFromQuest());
                giveUpButton.onClick.RemoveAllListeners();
                giveUpButton.onClick.AddListener(() => GiveUpQuest(quest, npc));
            }
        }
    }

    public void AcceptQuest(Quest quest, NPC npc)
    {
        player.ActiveQuests.Add(quest);
        UpdateQuests(npc);
        questInfo.gameObject.SetActive(false);
        npc.SetNpcFloatingText("?" , "QuestTaken");

        activeQuests.gameObject.SetActive(true);
        Vector3 pos = activeQuestPrefab.transform.position - new Vector3(0, 100, 0);
        Instantiate(activeQuestPrefab, pos, Quaternion.identity, activeQuests.transform);
        activeQuestName.text = quest.name;
        activeQuestName.color = new Color(0, 1, 0, 1);
        activeQuestName.fontSize = 10;
        pos = activeQuestPrefab.transform.position - new Vector3(0, 22, 0);
        // activeQuestProgressGo = Instantiate(activeQuestPrefab, pos, Quaternion.identity, activeQuests.transform);
        activeQuestProgress.text = "Little monsters killed: " + quest.currentAmount + "/" + quest.requiredAmount;
        activeQuestProgress.color = Color.white;
        activeQuestProgress.fontSize = 10;
    }

    public void GiveUpQuest(Quest quest, NPC npc)
    {
        player.ActiveQuests.Remove(quest);
        quest.currentAmount = 0;
        npc.SetNpcFloatingText("!", "NoQuest");
        questInfo.gameObject.SetActive(false);
        activeQuests.SetActive(false);
        turnInButton.gameObject.SetActive(false);
        giveUpButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        declineButton.gameObject.SetActive(true);
        UpdateQuests(npc);
    }

    public void backFromQuest()
    {
        questInfo.gameObject.SetActive(false);
    }

    public void DeclineQuest()
    {
        questInfo.gameObject.SetActive(false);
    }

    public bool isQuestInProgress(Quest quest)
    {
        foreach (Quest activeQuest in player.ActiveQuests)
        {
            if (activeQuest.id == quest.id)
                return true;
        }
        return false;
    }

    public void TurnInQuest(Quest quest, NPC npc)
    {
        player.GrantGold(quest.goldReward);
        player.GrantXp(quest.xpReward);
        player.ActiveQuests.Remove(quest);
        quests.gameObject.SetActive(false);
        activeQuests.SetActive(false);
        turnInButton.gameObject.SetActive(false);
        giveUpButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        declineButton.gameObject.SetActive(true);
        npc.SetNpcFloatingText("!", "NoQuest");
        quest.currentAmount = 0;
        UpdateQuests(npc);
    }

    public void UpdateActiveQuest(Quest quest)
    {
        quest.currentAmount++;     
        if (quest.isCompleted())
        {
            activeQuestProgress.text = "Monsters killed: " + quest.currentAmount + "/" + quest.requiredAmount + " (Completed!)";
            if (showFloatingText) // Don't show quest completion floating text more than once
                questCompletedFloatingText(quest);           
        }
        else activeQuestProgress.text = "Monsters killed: " + quest.currentAmount + "/" + quest.requiredAmount;

    }

    private void questCompletedFloatingText(Quest quest)
    {     
        string questCompletedText = "Quest Completed!";
        int fontSize = 12;
        float destroyTimer = 1.5f;
        float playerHieght = player.GetComponent<RectTransform>().rect.height;
        Vector3 position = player.transform.position + new Vector3(0, playerHieght);
        FloatingTextManager.instance.ShowFloatingText(questCompletedText, fontSize, Color.yellow, position, "GetResource", destroyTimer);
        showFloatingText = false;   
    }

    public void OnExitButton()
    {
        quests.gameObject.SetActive(false);
    }
}