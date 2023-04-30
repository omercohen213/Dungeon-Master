using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private List<NPC> npcs;

    [SerializeField] private GameObject quests;

    // QuestView objects
    [SerializeField] private GameObject QuestsExitButton;
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Transform content;

}
