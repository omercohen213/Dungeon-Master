using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : Collidable
{
    [SerializeField] private Text NpcFloatingText;
    [SerializeField] private Player player;

    // Quests objetcs
    [SerializeField] public List<Quest> quests;
    [SerializeField] public int nextQuestIndex;


    public void SetNpcFloatingText(string text)
    {
        NpcFloatingText.text = text;
    }

    protected override void Start()
    {
        base.Start();
        foreach (Quest quest in quests)
        {
            if (quest.isCompleted())
            {
                NpcFloatingText.text = "?";
                return;
            }
            else NpcFloatingText.text = "!";
        }
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            if (Input.GetKeyDown(KeyCode.E))
                QuestManager.instance.OnNpcInteract(this);
    }
}
