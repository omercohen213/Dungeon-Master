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


    public void SetNpcFloatingText(string text, string animTrigger)
    {
        NpcFloatingText.text = text;
        NpcFloatingText.GetComponent<Animator>().SetTrigger(animTrigger);
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            if (Input.GetKeyDown(KeyCode.E))
                OnNpcInteract();
    }

    private void OnNpcInteract()
    {
        QuestManager.instance.OnNpcInteract(this);
    }
}
